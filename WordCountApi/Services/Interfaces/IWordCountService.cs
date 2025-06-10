using WordCountApi.Model;

namespace WordCountApi.Services.Interfaces
{
    public interface IWordCountService
    {
        Task<Dictionary<string, int>> CountWordsAsync(Stream stream);
    }
}
