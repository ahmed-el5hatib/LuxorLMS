using LuxorLMS.Grading.Application.DTOs;
using LuxorLMS.Kernel;

namespace LuxorLMS.Grading.Application.Interfaces;

public interface IGradeSchemaService
{
    Task<Result<IReadOnlyList<GradeCategoryDto>>> GetCategoriesByOfferingAsync(Guid courseOfferingId, CancellationToken cancellationToken = default);
    Task<Result<GradeCategoryDto>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GradeCategoryDto>> CreateCategoryAsync(CreateGradeCategoryRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<GradeCategoryDto>> UpdateCategoryAsync(Guid id, UpdateGradeCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<GradeComponentDto>>> GetComponentsByCategoryAsync(Guid gradeCategoryId, CancellationToken cancellationToken = default);
    Task<Result<GradeComponentDto>> GetComponentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GradeComponentDto>> CreateComponentAsync(CreateGradeComponentRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Result<GradeComponentDto>> UpdateComponentAsync(Guid id, UpdateGradeComponentRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteComponentAsync(Guid id, CancellationToken cancellationToken = default);
}
