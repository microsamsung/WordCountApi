namespace WordCountApi.Services.Interfaces
{
    public interface IUnitOfWork
    {
        IWordRepository WordRepository { get; }
        Task CommitAsync();
    }
}
