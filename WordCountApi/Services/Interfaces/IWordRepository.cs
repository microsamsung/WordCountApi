using WordCountApi.Model;

namespace WordCountApi.Services.Interfaces
{
    public interface IWordRepository
    {
        Task AddOrUpdateWordAsync(string word, int count);
        Task<List<WordCountResult>> GetWordCountsAsync();
    }
}
