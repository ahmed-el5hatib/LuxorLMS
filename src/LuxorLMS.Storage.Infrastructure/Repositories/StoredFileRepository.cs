using LuxorLMS.Storage.Domain.Entities;
using LuxorLMS.Storage.Domain.Interfaces;
using LuxorLMS.Storage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Storage.Infrastructure.Repositories;

public class StoredFileRepository : IStoredFileRepository
{
    private readonly LuxorLMSStorageDbContext _context;

    public StoredFileRepository(LuxorLMSStorageDbContext context)
    {
        _context = context;
    }

    public async Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StoredFiles
            .Include(x => x.Versions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<StoredFile>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.StoredFiles
            .Where(x => x.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StoredFile storedFile, CancellationToken cancellationToken = default)
    {
        await _context.StoredFiles.AddAsync(storedFile, cancellationToken);
    }

    public void Update(StoredFile storedFile)
    {
        _context.StoredFiles.Update(storedFile);
    }

    public void Delete(StoredFile storedFile)
    {
        _context.StoredFiles.Remove(storedFile);
    }
}
