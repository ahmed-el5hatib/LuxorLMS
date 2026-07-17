using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Enums;
using LuxorLMS.Grading.Domain.Interfaces;
using LuxorLMS.Grading.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Grading.Infrastructure.Repositories;

public class StudentGradeRepository : IStudentGradeRepository
{
    private readonly LuxorLMSGradingDbContext _context;

    public StudentGradeRepository(LuxorLMSGradingDbContext context)
    {
        _context = context;
    }

    public async Task<StudentGrade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.StudentGrades.FindAsync(new object[] { id }, cancellationToken);

    public async Task<StudentGrade?> GetByOfferingAndStudentAsync(Guid courseOfferingId, Guid studentId, CancellationToken cancellationToken = default)
        => await _context.StudentGrades.AsNoTracking()
            .FirstOrDefaultAsync(sg => sg.CourseOfferingId == courseOfferingId && sg.StudentId == studentId, cancellationToken);

    public async Task<IReadOnlyList<StudentGrade>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
        => await _context.StudentGrades.AsNoTracking()
            .Where(sg => sg.CourseOfferingId == courseOfferingId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudentGrade>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.StudentGrades.AsNoTracking()
            .Where(sg => sg.StudentId == studentId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudentGrade>> GetPublishedByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
        => await _context.StudentGrades.AsNoTracking()
            .Where(sg => sg.StudentId == studentId && sg.PublishStatus == GradePublishStatus.Published)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudentGrade>> GetPublishedByStudentAndSemesterAsync(Guid studentId, Guid semesterId, CancellationToken cancellationToken = default)
        => await _context.StudentGrades.AsNoTracking()
            .Where(sg => sg.StudentId == studentId && sg.SemesterId == semesterId && sg.PublishStatus == GradePublishStatus.Published)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(StudentGrade grade, CancellationToken cancellationToken = default)
        => await _context.StudentGrades.AddAsync(grade, cancellationToken);

    public Task UpdateAsync(StudentGrade grade, CancellationToken cancellationToken = default)
    {
        _context.StudentGrades.Update(grade);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(StudentGrade grade, CancellationToken cancellationToken = default)
    {
        _context.StudentGrades.Remove(grade);
        return Task.CompletedTask;
    }
}
