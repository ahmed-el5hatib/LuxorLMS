using LuxorLMS.Storage.Domain.Entities;
using LuxorLMS.Storage.Domain.Interfaces;
using LuxorLMS.Storage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Storage.Infrastructure.Repositories;

public class FileVersionRepository : IFileVersionRepository
{
    private readonly LuxorLMSStorageDbContext _context;

    public FileVersionRepository(LuxorLMSStorageDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FileVersion>> GetByStoredFileIdAsync(Guid storedFileId, CancellationToken cancellationToken = default)
    {
        return await _context.FileVersions
            .Where(x => x.StoredFileId == storedFileId)
            .OrderBy(x => x.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FileVersion version, CancellationToken cancellationToken = default)
    {
        await _context.FileVersions.AddAsync(version, cancellationToken);
    }
}
