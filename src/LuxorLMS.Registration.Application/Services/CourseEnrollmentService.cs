using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Application.Services;
using LuxorLMS.Kernel;
using LuxorLMS.Registration.Application.Concurrency;
using LuxorLMS.Registration.Application.DTOs;
using LuxorLMS.Registration.Application.Interfaces;
using LuxorLMS.Registration.Domain.Entities;
using LuxorLMS.Registration.Domain.Enums;
using LuxorLMS.Registration.Domain.Interfaces;

namespace LuxorLMS.Registration.Application.Services;

public class CourseEnrollmentService : ICourseEnrollmentService
{
    private readonly ICourseEnrollmentRepository _repository;
    private readonly IRegistrationPeriodRepository _periodRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourseCatalogService _catalog;
    private readonly IStudentService _studentService;
    private readonly ISeatBookingLockFactory _lockFactory;

    public CourseEnrollmentService(
        ICourseEnrollmentRepository repository,
        IRegistrationPeriodRepository periodRepository,
        IUnitOfWork unitOfWork,
        ICourseCatalogService catalog,
        IStudentService studentService,
        ISeatBookingLockFactory lockFactory)
    {
        _repository = repository;
        _periodRepository = periodRepository;
        _unitOfWork = unitOfWork;
        _catalog = catalog;
        _studentService = studentService;
        _lockFactory = lockFactory;
    }

    public async Task<Result<IReadOnlyList<CourseEnrollmentDto>>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetByStudentIdAsync(studentId, cancellationToken);
        return Result<IReadOnlyList<CourseEnrollmentDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<IReadOnlyList<CourseEnrollmentDto>>> GetBySemesterIdAsync(Guid semesterId, CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetBySemesterIdAsync(semesterId, cancellationToken);
        return Result<IReadOnlyList<CourseEnrollmentDto>>.Success(items.Select(Map).ToList());
    }

    public async Task<Result<CourseEnrollmentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.NotFound", "Course enrollment not found."));
        return Result<CourseEnrollmentDto>.Success(Map(entity));
    }

    public async Task<Result<CourseEnrollmentDto>> RegisterAsync(RegisterCourseRequest request, Guid requestedBy, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var catalog = await _catalog.GetCourseAsync(request.CourseId, cancellationToken);
        if (catalog is null)
            return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.InvalidCourse", "Referenced course does not exist."));

        if (await _repository.GetByStudentCourseSemesterAsync(request.StudentId, request.CourseId, request.SemesterId, cancellationToken) is not null)
            return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.Duplicate", "Student is already enrolled in this course for the semester."));

        var period = request.RegistrationPeriodId.HasValue
            ? await _periodRepository.GetByIdAsync(request.RegistrationPeriodId.Value, cancellationToken)
            : (await _periodRepository.GetActiveAsync(now, cancellationToken))
                .FirstOrDefault(p => p.SemesterId == request.SemesterId);

        if (period is null)
            return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.NoOpenPeriod", "No active registration period for this semester."));

        var isLate = period.LateRegistrationStart.HasValue && period.LateRegistrationEnd.HasValue
                    && now >= period.LateRegistrationStart.Value && now <= period.LateRegistrationEnd.Value;
        var inRegularWindow = now >= period.StartDate && now <= period.EndDate;

        if (!inRegularWindow && !isLate)
            return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.OutsideWindow", "Registration is outside the allowed period."));

        // Prerequisite check
        var completed = await _repository.GetCompletedCourseIdsAsync(request.StudentId, cancellationToken);
        var missing = catalog.PrerequisiteCourseIds
            .Where(p => !completed.Contains(p))
            .ToList();
        if (missing.Count != 0)
            return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.PrerequisiteNotMet", "Prerequisite courses have not been completed."));

        // Distributed lock around capacity/credit-hour booking to prevent race conditions (M3.4).
        var resource = $"registration:{request.StudentId}:{request.SemesterId}";
        await using var seatLock = await _lockFactory.AcquireAsync(resource, TimeSpan.FromSeconds(15), cancellationToken);

        var studentGpa = (await _studentService.GetGpaByUserIdAsync(request.StudentId, cancellationToken)).Value;

        // Credit-hour limit check
        var currentCredits = await _repository.SumApprovedCreditHoursAsync(request.StudentId, request.SemesterId, cancellationToken);
        var effectiveMax = studentGpa < period.GpaCapForMax ? period.MinCreditHours : period.MaxCreditHours;
        if (currentCredits + catalog.CreditHours > effectiveMax)
            return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.CreditLimitExceeded", $"Enrolling would exceed the credit-hour limit of {effectiveMax}."));
        if (currentCredits + catalog.CreditHours < period.MinCreditHours && !isLate)
        {
        }

        var entity = new CourseEnrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            SemesterId = request.SemesterId,
            RegistrationPeriodId = period.Id,
            EnrollmentType = isLate ? EnrollmentType.Late : EnrollmentType.Regular,
            Status = EnrollmentStatus.Pending,
            CreditHours = catalog.CreditHours,
            RequestedAt = now,
            CreatedAt = now,
            CreatedBy = requestedBy
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CourseEnrollmentDto>.Success(Map(entity));
    }

    public async Task<Result<CourseEnrollmentDto>> ApproveAsync(Guid id, bool approve, Guid approvedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.NotFound", "Course enrollment not found."));
        if (entity.Status == EnrollmentStatus.Withdrawn)
            return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.Withdrawn", "Cannot approve a withdrawn enrollment."));

        entity.Status = approve ? EnrollmentStatus.Approved : EnrollmentStatus.Rejected;
        entity.ApprovedBy = approvedBy;
        entity.ApprovedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CourseEnrollmentDto>.Success(Map(entity));
    }

    public async Task<Result<CourseEnrollmentDto>> WithdrawAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.NotFound", "Course enrollment not found."));
        if (entity.Status == EnrollmentStatus.Withdrawn)
            return Result<CourseEnrollmentDto>.Failure(new Error("CourseEnrollment.AlreadyWithdrawn", "Enrollment is already withdrawn."));

        entity.Status = EnrollmentStatus.Withdrawn;
        entity.EnrollmentType = EnrollmentType.Withdraw;
        entity.GradeLetter = "W";
        entity.WithdrawnAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CourseEnrollmentDto>.Success(Map(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("CourseEnrollment.NotFound", "Course enrollment not found."));

        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static CourseEnrollmentDto Map(CourseEnrollment e) => new(
        e.Id, e.StudentId, e.CourseId, e.SemesterId, e.RegistrationPeriodId, e.EnrollmentType, e.Status,
        e.CreditHours, e.GradeLetter, e.IsPublished, e.ApprovedBy, e.ApprovedAt, e.WithdrawnAt, e.RequestedAt);
}
