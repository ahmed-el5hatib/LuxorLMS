using LuxorLMS.Academic.Application.DTOs;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Academic.Application.Services;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;
    private readonly ISectionMemberRepository _memberRepository;
    private readonly ICourseOfferingRepository _offeringRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SectionService(ISectionRepository sectionRepository, ISectionMemberRepository memberRepository, ICourseOfferingRepository offeringRepository, IUnitOfWork unitOfWork)
    {
        _sectionRepository = sectionRepository;
        _memberRepository = memberRepository;
        _offeringRepository = offeringRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<SectionDto>>> GetByCourseOfferingIdAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var items = await _sectionRepository.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken);
        return Result<IReadOnlyList<SectionDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<SectionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _sectionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<SectionDto>.Failure(new Error("Section.NotFound", "Section not found."));
        return Result<SectionDto>.Success(Map(entity));
    }

    public async Task<Result<SectionDto>> CreateAsync(CreateSectionRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (await _offeringRepository.GetByIdAsync(request.CourseOfferingId, cancellationToken) is null)
            return Result<SectionDto>.Failure(new Error("Section.InvalidOffering", "Referenced course offering does not exist."));

        var entity = new Section
        {
            Id = Guid.NewGuid(),
            CourseOfferingId = request.CourseOfferingId,
            SectionType = request.SectionType,
            AssignedStaffId = request.AssignedStaffId,
            Capacity = request.Capacity,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _sectionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<SectionDto>.Success(Map(entity));
    }

    public async Task<Result<SectionDto>> UpdateAsync(Guid id, UpdateSectionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _sectionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<SectionDto>.Failure(new Error("Section.NotFound", "Section not found."));

        entity.SectionType = request.SectionType;
        entity.AssignedStaffId = request.AssignedStaffId;
        entity.Capacity = request.Capacity;
        entity.IsActive = request.IsActive;

        await _sectionRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<SectionDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _sectionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("Section.NotFound", "Section not found."));

        await _sectionRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<SectionMemberDto>> AddMemberAsync(Guid sectionId, AddSectionMemberRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var section = await _sectionRepository.GetByIdAsync(sectionId, cancellationToken);
        if (section is null) return Result<SectionMemberDto>.Failure(new Error("Section.NotFound", "Section not found."));

        if (await _memberRepository.GetAsync(sectionId, request.StudentId, cancellationToken) is not null)
            return Result<SectionMemberDto>.Failure(new Error("SectionMember.Duplicate", "Student is already a member of this section."));

        var memberCount = await _sectionRepository.CountMembersAsync(sectionId, cancellationToken);
        if (memberCount >= section.Capacity)
            return Result<SectionMemberDto>.Failure(new Error("SectionMember.CapacityExceeded", "Section capacity has been reached."));

        var member = new SectionMember
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            StudentId = request.StudentId,
            EnrolledAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _memberRepository.AddAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<SectionMemberDto>.Success(new SectionMemberDto(member.Id, member.SectionId, member.StudentId, member.EnrolledAt));
    }

    public async Task<Result> RemoveMemberAsync(Guid sectionId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetAsync(sectionId, studentId, cancellationToken);
        if (member is null) return Result.Failure(new Error("SectionMember.NotFound", "Section membership not found."));

        await _memberRepository.DeleteAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static SectionDto Map(Section s) => new(s.Id, s.CourseOfferingId, s.SectionType, s.AssignedStaffId, s.Capacity, s.IsActive, s.CreatedAt);
}
