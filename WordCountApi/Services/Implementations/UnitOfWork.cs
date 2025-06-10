using WordCountApi.Data;
using WordCountApi.Services.Interfaces;

namespace WordCountApi.Services.Implementations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly WordDbContext _context;

        public IWordRepository WordRepository { get; }

        public UnitOfWork(WordDbContext context, IWordRepository wordRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            WordRepository = wordRepository ?? throw new ArgumentNullException(nameof(wordRepository));
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
