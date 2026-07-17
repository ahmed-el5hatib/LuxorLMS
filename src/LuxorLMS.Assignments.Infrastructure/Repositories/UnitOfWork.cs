using LuxorLMS.Assignments.Domain.Interfaces;
using LuxorLMS.Assignments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace LuxorLMS.Assignments.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LuxorLMSAssignmentsDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(LuxorLMSAssignmentsDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
