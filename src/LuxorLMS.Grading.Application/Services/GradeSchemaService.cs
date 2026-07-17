using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Grading.Application.Services;

public class GradeSchemaService : IGradeSchemaService
{
    private readonly IGradeCategoryRepository _categoryRepository;
    private readonly IGradeComponentRepository _componentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GradeSchemaService(
        IGradeCategoryRepository categoryRepository,
        IGradeComponentRepository componentRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _componentRepository = componentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<GradeCategoryDto>>> GetCategoriesByOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default)
    {
        var items = await _categoryRepository.GetByCourseOfferingIdAsync(courseOfferingId, cancellationToken);
        return Result<IReadOnlyList<GradeCategoryDto>>.Success(items.Select(MapCategory).ToList());
    }

    public async Task<Result<GradeCategoryDto>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<GradeCategoryDto>.Failure(new Error("GradeCategory.NotFound", "Grade category not found."));
        return Result<GradeCategoryDto>.Success(MapCategory(entity));
    }

    public async Task<Result<GradeCategoryDto>> CreateCategoryAsync(CreateGradeCategoryRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        if (request.Weight is < 0 or > 1)
            return Result<GradeCategoryDto>.Failure(new Error("GradeCategory.InvalidWeight", "Weight must be between 0 and 1."));

        var existing = await _categoryRepository.GetByCourseOfferingIdAsync(request.CourseOfferingId, cancellationToken);
        var totalWeight = existing.Where(c => c.IsActive).Sum(c => c.Weight) + request.Weight;
        if (totalWeight > 1.0m)
            return Result<GradeCategoryDto>.Failure(new Error("GradeCategory.WeightOverflow", "Total category weights for the offering cannot exceed 1.0."));

        var entity = new GradeCategory
        {
            Id = Guid.NewGuid(),
            CourseOfferingId = request.CourseOfferingId,
            Name = request.Name,
            Weight = request.Weight,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _categoryRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GradeCategoryDto>.Success(MapCategory(entity));
    }

    public async Task<Result<GradeCategoryDto>> UpdateCategoryAsync(Guid id, UpdateGradeCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<GradeCategoryDto>.Failure(new Error("GradeCategory.NotFound", "Grade category not found."));

        if (request.Weight is < 0 or > 1)
            return Result<GradeCategoryDto>.Failure(new Error("GradeCategory.InvalidWeight", "Weight must be between 0 and 1."));

        var siblings = await _categoryRepository.GetByCourseOfferingIdAsync(entity.CourseOfferingId, cancellationToken);
        var totalWeight = siblings.Where(c => c.IsActive && c.Id != id).Sum(c => c.Weight) + (request.IsActive ? request.Weight : 0m);
        if (totalWeight > 1.0m)
            return Result<GradeCategoryDto>.Failure(new Error("GradeCategory.WeightOverflow", "Total category weights for the offering cannot exceed 1.0."));

        entity.Name = request.Name;
        entity.Weight = request.Weight;
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsActive = request.IsActive;

        await _categoryRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GradeCategoryDto>.Success(MapCategory(entity));
    }

    public async Task<Result> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("GradeCategory.NotFound", "Grade category not found."));

        await _categoryRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<GradeComponentDto>>> GetComponentsByCategoryAsync(Guid gradeCategoryId, CancellationToken cancellationToken = default)
    {
        var items = await _componentRepository.GetByCategoryIdAsync(gradeCategoryId, cancellationToken);
        return Result<IReadOnlyList<GradeComponentDto>>.Success(items.Select(MapComponent).ToList());
    }

    public async Task<Result<GradeComponentDto>> GetComponentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _componentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<GradeComponentDto>.Failure(new Error("GradeComponent.NotFound", "Grade component not found."));
        return Result<GradeComponentDto>.Success(MapComponent(entity));
    }

    public async Task<Result<GradeComponentDto>> CreateComponentAsync(CreateGradeComponentRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(request.GradeCategoryId, cancellationToken);
        if (category is null) return Result<GradeComponentDto>.Failure(new Error("GradeComponent.InvalidCategory", "Referenced grade category does not exist."));

        if (request.MaxPoints <= 0)
            return Result<GradeComponentDto>.Failure(new Error("GradeComponent.InvalidMaxPoints", "MaxPoints must be greater than 0."));

        var entity = new GradeComponent
        {
            Id = Guid.NewGuid(),
            GradeCategoryId = request.GradeCategoryId,
            Title = request.Title,
            MaxPoints = request.MaxPoints,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _componentRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GradeComponentDto>.Success(MapComponent(entity));
    }

    public async Task<Result<GradeComponentDto>> UpdateComponentAsync(Guid id, UpdateGradeComponentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _componentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result<GradeComponentDto>.Failure(new Error("GradeComponent.NotFound", "Grade component not found."));

        if (request.MaxPoints <= 0)
            return Result<GradeComponentDto>.Failure(new Error("GradeComponent.InvalidMaxPoints", "MaxPoints must be greater than 0."));

        entity.Title = request.Title;
        entity.MaxPoints = request.MaxPoints;
        entity.IsActive = request.IsActive;

        await _componentRepository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GradeComponentDto>.Success(MapComponent(entity));
    }

    public async Task<Result> DeleteComponentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _componentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return Result.Failure(new Error("GradeComponent.NotFound", "Grade component not found."));

        await _componentRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static GradeCategoryDto MapCategory(GradeCategory c) => new(
        c.Id, c.CourseOfferingId, c.Name, c.Weight, c.DisplayOrder, c.IsActive, c.CreatedAt);

    private static GradeComponentDto MapComponent(GradeComponent c) => new(
        c.Id, c.GradeCategoryId, c.Title, c.MaxPoints, c.IsActive, c.CreatedAt);
}
