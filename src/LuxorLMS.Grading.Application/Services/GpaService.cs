using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Grading.Application.Services;

public class GpaService : IGpaService
{
    private readonly IStudentGradeRepository _repository;
    private readonly IGradeScaleService _scale;
    private readonly IAcademicGradingGateway _academic;

    public GpaService(
        IStudentGradeRepository repository,
        IGradeScaleService scale,
        IAcademicGradingGateway academic)
    {
        _repository = repository;
        _scale = scale;
        _academic = academic;
    }

    public async Task<Result<SemesterGpaResultDto>> GetSemesterGpaAsync(Guid studentId, Guid semesterId, CancellationToken cancellationToken = default)
    {
        var grades = await _repository.GetPublishedByStudentAndSemesterAsync(studentId, semesterId, cancellationToken);
        var (gpa, credits, count) = Compute(grades);
        return Result<SemesterGpaResultDto>.Success(new SemesterGpaResultDto(studentId, semesterId, gpa, credits, count));
    }

    public async Task<Result<GpaResultDto>> GetCumulativeGpaAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var grades = await _repository.GetPublishedByStudentAsync(studentId, cancellationToken);
        var (gpa, credits, count) = Compute(grades);
        return Result<GpaResultDto>.Success(new GpaResultDto(studentId, gpa, credits, count));
    }

    public async Task<Result<GpaResultDto>> RecalculateAndPersistCgpaAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var grades = await _repository.GetPublishedByStudentAsync(studentId, cancellationToken);
        var (gpa, credits, count) = Compute(grades);

        // StudentId in Grading is the Identity User.Id, which Academic.Student.UserId references.
        await _academic.UpdateStudentCgpaAsync(studentId, gpa, cancellationToken);

        return Result<GpaResultDto>.Success(new GpaResultDto(studentId, gpa, credits, count));
    }

    private (decimal Gpa, int Credits, int Count) Compute(IEnumerable<StudentGrade> grades)
    {
        var counted = grades.Where(g => _scale.CountsTowardGpa(g.GradeLetter)).ToList();
        var totalCredits = counted.Sum(g => g.CreditHours);
        if (totalCredits == 0)
            return (0m, 0, counted.Count);

        var weightedPoints = counted.Sum(g => g.GradePoints * g.CreditHours);
        var gpa = Math.Round(weightedPoints / totalCredits, 2, MidpointRounding.AwayFromZero);
        return (gpa, totalCredits, counted.Count);
    }
}
