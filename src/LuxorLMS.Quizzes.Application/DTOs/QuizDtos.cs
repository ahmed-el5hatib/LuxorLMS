using LuxorLMS.Quizzes.Domain.Enums;

namespace LuxorLMS.Quizzes.Application.DTOs;

// ---------- Quiz ----------
public record QuizDto(
    Guid Id,
    Guid CourseOfferingId,
    string Title,
    string? Description,
    int TimeLimitMinutes,
    bool IsPublished,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    bool IsActive,
    DateTime CreatedAt);

public record CreateQuizRequest(
    Guid CourseOfferingId,
    string Title,
    string? Description,
    int TimeLimitMinutes,
    DateTime? AvailableFrom,
    DateTime? AvailableTo);

public record UpdateQuizRequest(
    string Title,
    string? Description,
    int TimeLimitMinutes,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    bool IsActive);

// ---------- Question ----------
public record QuizQuestionDto(
    Guid Id,
    Guid QuizId,
    QuestionType QuestionType,
    string Text,
    decimal Points,
    int DisplayOrder,
    bool IsActive,
    DateTime CreatedAt);

public record CreateQuizQuestionRequest(
    Guid QuizId,
    QuestionType QuestionType,
    string Text,
    decimal Points,
    int DisplayOrder);

public record UpdateQuizQuestionRequest(
    QuestionType QuestionType,
    string Text,
    decimal Points,
    int DisplayOrder,
    bool IsActive);

// ---------- Option ----------
public record QuizOptionDto(
    Guid Id,
    Guid QuizQuestionId,
    string Text,
    bool IsCorrect,
    int DisplayOrder,
    bool IsActive,
    DateTime CreatedAt);

public record CreateQuizOptionRequest(
    Guid QuizQuestionId,
    string Text,
    bool IsCorrect,
    int DisplayOrder);

public record UpdateQuizOptionRequest(
    string Text,
    bool IsCorrect,
    int DisplayOrder,
    bool IsActive);

// ---------- Visual Builder ----------
public record QuizBuilderOptionRequest(
    string Text,
    bool IsCorrect,
    int DisplayOrder);

public record QuizBuilderQuestionRequest(
    QuestionType QuestionType,
    string Text,
    decimal Points,
    int DisplayOrder,
    IReadOnlyList<QuizBuilderOptionRequest> Options);

public record QuizBuilderRequest(
    Guid CourseOfferingId,
    string Title,
    string? Description,
    int TimeLimitMinutes,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    IReadOnlyList<QuizBuilderQuestionRequest> Questions);

public record QuizBuilderQuestionDto(
    QuizQuestionDto Question,
    IReadOnlyList<QuizOptionDto> Options);

public record QuizDetailDto(
    QuizDto Quiz,
    IReadOnlyList<QuizBuilderQuestionDto> Questions);

// ---------- Attempt ----------
public record QuizAttemptDto(
    Guid Id,
    Guid QuizId,
    Guid StudentId,
    DateTime StartedAt,
    DateTime? SubmittedAt,
    decimal? Score,
    QuizAttemptStatus Status,
    bool IsActive,
    DateTime CreatedAt);

public record StartAttemptRequest(Guid QuizId);

// ---------- Answer ----------
public record QuizAnswerDto(
    Guid Id,
    Guid QuizAttemptId,
    Guid QuizQuestionId,
    Guid? SelectedOptionId,
    string? TextAnswer,
    bool IsCorrect,
    bool IsActive,
    DateTime CreatedAt);

public record SaveAnswerRequest(
    Guid QuizAttemptId,
    Guid QuizQuestionId,
    Guid? SelectedOptionId,
    string? TextAnswer);

public record GradeAnswerRequest(bool IsCorrect);
