using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Academic.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly LuxorLMSAcademicDbContext _context;

    public StudentRepository(LuxorLMSAcademicDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Students.FindAsync(new object[] { id }, cancellationToken);

    public async Task<Student?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Students.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

    public async Task<decimal> GetGpaByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Students.AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => s.Gpa)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        => await _context.Students.AddAsync(student, cancellationToken);

    public Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        _context.Students.Update(student);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Student student, CancellationToken cancellationToken = default)
    {
        _context.Students.Remove(student);
        return Task.CompletedTask;
    }
}
