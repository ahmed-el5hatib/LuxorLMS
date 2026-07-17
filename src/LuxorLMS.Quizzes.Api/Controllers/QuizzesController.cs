using LuxorLMS.Quizzes.Api.Authorization;
using LuxorLMS.Quizzes.Api.Filters;
using LuxorLMS.Quizzes.Application.DTOs;
using LuxorLMS.Quizzes.Application.Interfaces;
using LuxorLMS.Kernel;
using Microsoft.AspNetCore.Mvc;

namespace LuxorLMS.Quizzes.Api.Controllers;

[ApiController]
[Route("api/v1/quizzes")]
public class QuizzesController : ControllerBase
{
    private readonly IQuizService _service;

    public QuizzesController(IQuizService service)
    {
        _service = service;
    }

    // ---------------- Quizzes ----------------

    [HttpGet("offering/{courseOfferingId:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetByOffering(Guid courseOfferingId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByOfferingAsync(courseOfferingId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("{id:guid}/detail")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetDetail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetDetailAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost]
    [RequirePermissionFilterFactory(QuizPermissions.Create)]
    public async Task<IActionResult> Create([FromBody] CreateQuizRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("builder")]
    [RequirePermissionFilterFactory(QuizPermissions.Create)]
    public async Task<IActionResult> Build([FromBody] QuizBuilderRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.BuildAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetDetail), new { id = result.Value!.Quiz.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.Edit)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuizRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("{id:guid}/publish")]
    [RequirePermissionFilterFactory(QuizPermissions.Publish)]
    public async Task<IActionResult> Publish(Guid id, [FromBody] PublishQuizRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.PublishAsync(id, request.Publish, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.Edit)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    // ---------------- Questions ----------------

    [HttpGet("{quizId:guid}/questions")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetQuestions(Guid quizId, CancellationToken cancellationToken)
    {
        var result = await _service.GetQuestionsByQuizAsync(quizId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("questions/{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetQuestionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetQuestionByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("questions")]
    [RequirePermissionFilterFactory(QuizPermissions.Create)]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateQuizQuestionRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateQuestionAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetQuestionById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("questions/{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.Edit)]
    public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateQuizQuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateQuestionAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("questions/{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.Edit)]
    public async Task<IActionResult> DeleteQuestion(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteQuestionAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    // ---------------- Options ----------------

    [HttpGet("questions/{quizQuestionId:guid}/options")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetOptions(Guid quizQuestionId, CancellationToken cancellationToken)
    {
        var result = await _service.GetOptionsByQuestionAsync(quizQuestionId, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpGet("options/{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.View)]
    public async Task<IActionResult> GetOptionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.GetOptionByIdAsync(id, cancellationToken);
        return result.IsFailure ? NotFound(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpPost("options")]
    [RequirePermissionFilterFactory(QuizPermissions.Create)]
    public async Task<IActionResult> CreateOption([FromBody] CreateQuizOptionRequest request, CancellationToken cancellationToken)
    {
        var current = PermissionHelper.GetCurrentUserId(User);
        var result = await _service.CreateOptionAsync(request, current ?? Guid.Empty, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : CreatedAtAction(nameof(GetOptionById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("options/{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.Edit)]
    public async Task<IActionResult> UpdateOption(Guid id, [FromBody] UpdateQuizOptionRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateOptionAsync(id, request, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : Ok(result.Value);
    }

    [HttpDelete("options/{id:guid}")]
    [RequirePermissionFilterFactory(QuizPermissions.Edit)]
    public async Task<IActionResult> DeleteOption(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteOptionAsync(id, cancellationToken);
        return result.IsFailure ? BadRequest(ToProblem(result.Error)) : NoContent();
    }

    private static object ToProblem(Error error) => new { error = error.Code, description = error.Description };
}

public record PublishQuizRequest(bool Publish);
