namespace LuxorLMS.Grading.Application.DTOs;

public record GpaResultDto(
    Guid StudentId,
    decimal Gpa,
    int TotalCreditHours,
    int CourseCount);

public record SemesterGpaResultDto(
    Guid StudentId,
    Guid SemesterId,
    decimal SemesterGpa,
    int TotalCreditHours,
    int CourseCount);
