using LuxorLMS.Quizzes.Api.Authorization;
using LuxorLMS.Quizzes.Api.Filters;
using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Quizzes.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Quizzes.Api.Controllers;

[ApiController]
[Route("api/v1/quizzes/attempts")]
public class QuizAttemptsController : ControllerBase
{
    private readonly IQuizAttemptService _service;

    public QuizAttemptsController(IQuizAttemptService service)
    {
        _service = service;
    }

    [HttpGet("quiz/{quizId:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetByQuiz(Guid quizId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByQuizAsync(quizId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("student/{studentId:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetByStudent(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByStudentAsync(studentId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("start")]
    [RequirePermissionFilterFactory(QuizPermissions.Attempt)]
    public async Task<IActionResult> Start([FromBody] StartAttemptRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.StartAsync(request.QuizId, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("{id:guid}/submit")]
    [RequirePermissionFilterFactory(QuizPermissions.Attempt)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.SubmitAsync(id, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/auto-submit")]
    [RequirePermissionFilterFactory(QuizPermissions.Attempt)]
    public async Task<IActionResult> AutoSubmit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.AutoSubmitIfExpiredAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
