using LuxorLMS.Storage.Domain.Entities;
using LuxorLMS.Storage.Domain.Interfaces;
using LuxorLMS.Storage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Storage.Infrastructure.Repositories;

public class StorageProviderConfigRepository : IStorageProviderConfigRepository
{
    private readonly LuxorLMSStorageDbContext _context;

    public StorageProviderConfigRepository(LuxorLMSStorageDbContext context)
    {
        _context = context;
    }

    public async Task<StorageProviderConfig?> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StorageProviderConfigs.FirstOrDefaultAsync(x => x.IsActive, cancellationToken);
    }

    public async Task<StorageProviderConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StorageProviderConfigs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<StorageProviderConfig>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StorageProviderConfigs.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StorageProviderConfig config, CancellationToken cancellationToken = default)
    {
        await _context.StorageProviderConfigs.AddAsync(config, cancellationToken);
    }

    public void Update(StorageProviderConfig config)
    {
        _context.StorageProviderConfigs.Update(config);
    }
}
