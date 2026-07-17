using LuxorLMS.Kernel;
using LuxorLMS.Reporting.Application.DTOs;
using LuxorLMS.Reporting.Domain.Entities;
using LuxorLMS.Reporting.Domain.Enums;
using LuxorLMS.Reporting.Domain.Interfaces;

namespace LuxorLMS.Reporting.Application.Interfaces;

public interface IReportingService
{
    Task<Result<ReportJobDto>> CreateReportAsync(CreateReportRequest request, Guid requestedBy, CancellationToken cancellationToken = default);
    Task<Result<ReportJobDto>> GetReportAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ReportJobDto>>> GetUserReportsAsync(Guid userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ReportTemplateDto>>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<Result<ReportTemplateDto>> CreateTemplateAsync(string code, string name, ReportType reportType, ExportFormat format, string templatePath, Guid createdBy, CancellationToken cancellationToken = default);
}
