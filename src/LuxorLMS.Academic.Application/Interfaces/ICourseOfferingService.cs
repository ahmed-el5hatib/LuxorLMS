using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Interfaces;

public interface ICourseOfferingService
{
    Task<Result<IReadOnlyList<CourseOfferingDto>>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CourseOfferingDto>>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<Result<CourseOfferingDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<CourseOfferingDto>> CreateAsync(CreateCourseOfferingRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<CourseOfferingDto>> UpdateAsync(Guid id, UpdateCourseOfferingRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ISectionService
{
    Task<Result<IReadOnlyList<SectionDto>>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<SectionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SectionDto>> CreateAsync(CreateSectionRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<SectionDto>> UpdateAsync(Guid id, UpdateSectionRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SectionMemberDto>> AddMemberAsync(Guid sectionId, AddSectionMemberRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result> RemoveMemberAsync(Guid sectionId, Guid studentId, CancellationToken cancellationToken = default);
}
