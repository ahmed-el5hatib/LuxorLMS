namespace LuxorLMS.Storage.Domain.Interfaces;

public interface IStorageUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
