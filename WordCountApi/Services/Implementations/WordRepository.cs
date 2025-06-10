using Microsoft.EntityFrameworkCore;
using WordCountApi.Data;
using WordCountApi.Model;

namespace WordCountApi.Services.Interfaces
{
    public class WordRepository : IWordRepository
    {
        private readonly WordDbContext _context;

        public WordRepository(WordDbContext context) => _context = context;

        public async Task AddOrUpdateWordAsync(string word, int count)
        {
            var existing = await _context.Words.FirstOrDefaultAsync(w => w.Text == word);
            if (existing != null)
                existing.Count += count;
            else
                _context.Words.Add(new Word { Text = word, Count = count });
        }

        public async Task<List<WordCountResult>> GetWordCountsAsync()
        {
            return await _context.Words
                .OrderByDescending(w => w.Count)
                .Select(w => new WordCountResult { Word = w.Text, Count = w.Count })
                .ToListAsync();
        }
    }

}
