using LuxorLMS.Storage.Domain.Interfaces;
using LuxorLMS.Storage.Infrastructure.Persistence;

namespace LuxorLMS.Storage.Infrastructure.Repositories;

public class StorageUnitOfWork : IStorageUnitOfWork
{
    private readonly LuxorLMSStorageDbContext _context;

    public StorageUnitOfWork(LuxorLMSStorageDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
