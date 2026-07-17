using LuxorLMS.Reporting.Application.DTOs;
using LuxorLMS.Reporting.Application.Interfaces;
using LuxorLMS.Reporting.Domain.Entities;
using LuxorLMS.Reporting.Domain.Enums;
using LuxorLMS.Reporting.Domain.Interfaces;
using LuxorLMS.Kernel;

namespace LuxorLMS.Reporting.Application.Services;

public class ReportingService : IReportingService
{
    private readonly IReportJobRepository _jobRepository;
    private readonly IReportTemplateRepository _templateRepository;
    private readonly IReportingUnitOfWork _unitOfWork;

    public ReportingService(
        IReportJobRepository jobRepository,
        IReportTemplateRepository templateRepository,
        IReportingUnitOfWork unitOfWork)
    {
        _jobRepository = jobRepository;
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReportJobDto>> CreateReportAsync(CreateReportRequest request, Guid requestedBy, CancellationToken cancellationToken = default)
    {
        var job = new ReportJob
        {
            ReportType = request.ReportType,
            Format = request.Format,
            RequestedBy = requestedBy,
            CourseOfferingId = request.CourseOfferingId,
            StudentId = request.StudentId,
            Status = ReportStatus.Queued,
            RequestedAt = DateTime.UtcNow,
            Parameters = request.Parameters
        };

        await _jobRepository.AddAsync(job, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ReportJobDto>.Success(MapJob(job));
    }

    public async Task<Result<ReportJobDto>> GetReportAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        var job = await _jobRepository.GetByIdAsync(reportId, cancellationToken);
        if (job is null)
            return Result<ReportJobDto>.Failure(new Error("Reporting.NotFound", "Report not found."));

        return Result<ReportJobDto>.Success(MapJob(job));
    }

    public async Task<Result<IReadOnlyList<ReportJobDto>>> GetUserReportsAsync(Guid userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var jobs = await _jobRepository.GetByUserAsync(userId, pageNumber, pageSize, cancellationToken);
        var result = jobs.Select(MapJob).ToList();
        return Result<IReadOnlyList<ReportJobDto>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<ReportTemplateDto>>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = await _templateRepository.GetAllAsync(cancellationToken);
        var result = templates.Select(MapTemplate).ToList();
        return Result<IReadOnlyList<ReportTemplateDto>>.Success(result);
    }

    public async Task<Result<ReportTemplateDto>> CreateTemplateAsync(string code, string name, ReportType reportType, ExportFormat format, string templatePath, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var template = new ReportTemplate
        {
            Code = code,
            Name = name,
            ReportType = reportType,
            Format = format,
            TemplatePath = templatePath,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _templateRepository.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ReportTemplateDto>.Success(MapTemplate(template));
    }

    private static ReportJobDto MapJob(ReportJob j) => new(
        j.Id,
        j.ReportType.ToString(),
        j.Format.ToString(),
        j.RequestedBy,
        j.CourseOfferingId,
        j.StudentId,
        j.Status.ToString(),
        j.FilePath,
        j.FileName,
        j.FileSizeBytes,
        j.Error,
        j.RequestedAt,
        j.CompletedAt
    );

    private static ReportTemplateDto MapTemplate(ReportTemplate t) => new(
        t.Id,
        t.Code,
        t.Name,
        t.ReportType.ToString(),
        t.Format.ToString(),
        t.TemplatePath,
        t.IsActive,
        t.CreatedAt
    );
}
