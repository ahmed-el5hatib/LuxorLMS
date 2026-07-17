using LuxorLMS.Registration.Domain.Enums;

namespace LuxorLMS.Registration.Application.DTOs;

public record CourseEnrollmentDto(Guid Id, Guid StudentId, Guid CourseId, Guid SemesterId, Guid? RegistrationPeriodId, EnrollmentType EnrollmentType, EnrollmentStatus Status, int CreditHours, string? GradeLetter, bool IsPublished, Guid? ApprovedBy, DateTime? ApprovedAt, DateTime? WithdrawnAt, DateTime RequestedAt);
public record RegisterCourseRequest(Guid StudentId, Guid CourseId, Guid SemesterId, Guid? RegistrationPeriodId);
public record ApproveCourseEnrollmentRequest(bool Approve);
public record WithdrawCourseEnrollmentRequest;
