using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using WordCountApi.Model;
using WordCountApi.Services.Interfaces;

namespace WordCountApi.Services.Implementations
{
    public class WordCountService : IWordCountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WordCountService> _logger;

        //  Lock to protect DB writes
        private static readonly SemaphoreSlim _saveLock = new(1, 1);

        public WordCountService(IUnitOfWork unitOfWork, ILogger<WordCountService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Dictionary<string, int>> CountWordsAsync(Stream stream)
        {
            try
            {
                using var reader = new StreamReader(stream);
                string text = await reader.ReadToEndAsync();

                var matches = Regex.Matches(text.ToLower(), @"\b[\w']+\b");

                var wordCounts = new ConcurrentDictionary<string, int>();

                foreach (Match match in matches)
                {
                    string word = match.Value;
                    wordCounts.AddOrUpdate(word, 1, (_, existing) => existing + 1);
                }

                //var wordEntities = wordCounts.Select(kvp => new WordCountResult
                //{
                //    Word = kvp.Key,
                //    Count = kvp.Value
                //}).ToList();

                var wordEntities = wordCounts
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => new WordCountResult
                {
                    Word = kvp.Key,
                    Count = kvp.Value
                }).ToList();

                // Critical section: DB insert/update
                await _saveLock.WaitAsync();
                try
                {
                    foreach (var entity in wordEntities)
                    {
                        await _unitOfWork.WordRepository.AddOrUpdateWordAsync(entity.Word, entity.Count);
                    }

                    await _unitOfWork.CommitAsync();
                }
                finally
                {
                    _saveLock.Release();
                }

                _logger.LogInformation("Processed and saved {Count} words.", wordEntities.Count);

                return wordCounts
                    .OrderByDescending(kvp => kvp.Value)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in WordCountService.");
                throw;
            }
        }
    }
}
