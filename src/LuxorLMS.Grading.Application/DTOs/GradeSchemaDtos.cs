namespace LuxorLMS.Grading.Application.DTOs;

public record GradeCategoryDto(
    Guid Id,
    Guid CourseOfferingId,
    string Name,
    decimal Weight,
    int DisplayOrder,
    bool IsActive,
    DateTime CreatedAt);

public record CreateGradeCategoryRequest(
    Guid CourseOfferingId,
    string Name,
    decimal Weight,
    int DisplayOrder);

public record UpdateGradeCategoryRequest(
    string Name,
    decimal Weight,
    int DisplayOrder,
    bool IsActive);

public record GradeComponentDto(
    Guid Id,
    Guid GradeCategoryId,
    string Title,
    decimal MaxPoints,
    bool IsActive,
    DateTime CreatedAt);

public record CreateGradeComponentRequest(
    Guid GradeCategoryId,
    string Title,
    decimal MaxPoints);

public record UpdateGradeComponentRequest(
    string Title,
    decimal MaxPoints,
    bool IsActive);
