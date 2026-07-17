using LuxorLMS.Registration.Domain.Enums;
using LuxorLMS.Registration.Domain.Entities;
using LuxorLMS.Registration.Domain.Interfaces;
using LuxorLMS.Registration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Registration.Infrastructure.Repositories;

public class StudentProgramEnrollmentRepository : IStudentProgramEnrollmentRepository
{
    private readonly LuxorLMSRegistrationDbContext _context;

    public StudentProgramEnrollmentRepository(LuxorLMSRegistrationDbContext context)
    {
        _context = context;
    }

    public async Task<StudentProgramEnrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.StudentProgramEnrollments.FindAsync(new object[] { id }, cancellationToken);

    public async Task<StudentProgramEnrollment?> GetActiveByStudentAndProgramAsync(Guid studentId, Guid programId, CancellationToken cancellationToken = default)
        => await _context.StudentProgramEnrollments.AsNoTracking()
            .FirstOrDefaultAsync(spe => spe.StudentId == studentId && spe.ProgramId == programId && spe.Status == StudentProgramStatus.Active, cancellationToken);

    public async Task<IReadOnlyList<StudentProgramEnrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.StudentProgramEnrollments.AsNoTracking()
            .Where(spe => spe.StudentId == studentId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(StudentProgramEnrollment enrollment, CancellationToken cancellationToken = default)
        => await _context.StudentProgramEnrollments.AddAsync(enrollment, cancellationToken);

    public Task UpdateAsync(StudentProgramEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.StudentProgramEnrollments.Update(enrollment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(StudentProgramEnrollment enrollment, CancellationToken cancellationToken = default)
    {
        _context.StudentProgramEnrollments.Remove(enrollment);
        return Task.CompletedTask;
    }
}
