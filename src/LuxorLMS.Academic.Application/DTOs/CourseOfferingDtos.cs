using LuxorLMS.Academic.Domain.Enums;

namespace LuxorLMS.Academic.Application.DTOs;

public record CourseOfferingDto(Guid Id, Guid CourseId, Guid SemesterId, Guid PrimaryTeacherId, int Capacity, DateTime RegistrationStart, DateTime RegistrationEnd, OfferingStatus Status, bool IsActive, DateTime CreatedAt);
public record CreateCourseOfferingRequest(Guid CourseId, Guid SemesterId, Guid PrimaryTeacherId, int Capacity, DateTime RegistrationStart, DateTime RegistrationEnd);
public record UpdateCourseOfferingRequest(int Capacity, DateTime RegistrationStart, DateTime RegistrationEnd, OfferingStatus Status, bool IsActive);

public record SectionDto(Guid Id, Guid CourseOfferingId, SectionType SectionType, Guid AssignedStaffId, int Capacity, bool IsActive, DateTime CreatedAt);
public record CreateSectionRequest(Guid CourseOfferingId, SectionType SectionType, Guid AssignedStaffId, int Capacity);
public record UpdateSectionRequest(SectionType SectionType, Guid AssignedStaffId, int Capacity, bool IsActive);

public record SectionMemberDto(Guid Id, Guid SectionId, Guid StudentId, DateTime EnrolledAt);
public record AddSectionMemberRequest(Guid StudentId);
