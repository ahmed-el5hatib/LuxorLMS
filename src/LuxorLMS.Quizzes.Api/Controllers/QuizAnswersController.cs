using LuxorLMS.Quizzes.Api.Authorization;
using LuxorLMS.Quizzes.Api.Filters;
using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Quizzes.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Quizzes.Api.Controllers;

[ApiController]
[Route("api/v1/quizzes/answers")]
public class QuizAnswersController : ControllerBase
{
    private readonly IQuizAnswerService _service;

    public QuizAnswersController(IQuizAnswerService service)
    {
        _service = service;
    }

    [HttpGet("attempt/{quizAttemptId:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetByAttempt(Guid quizAttemptId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByAttemptAsync(quizAttemptId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(QuizPermissions.Attempt)]
    public async Task<IActionResult> Save([FromBody] SaveAnswerRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.SaveAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("{id:guid}/grade")]
    [RequirePermissionFilterFactory(QuizPermissions.Grade)]
    public async Task<IActionResult> Grade(Guid id, [FromBody] GradeAnswerRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.GradeAsync(id, request.IsCorrect, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.Attempt)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}
