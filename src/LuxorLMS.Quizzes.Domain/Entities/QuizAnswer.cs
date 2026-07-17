namespace LuxorLMS.Quizzes.Domain.Entities;

public class QuizAnswer
{
    public Guid Id { get; set; }
    public Guid QuizAttemptId { get; set; }
    public Guid QuizQuestionId { get; set; }
    public Guid? SelectedOptionId { get; set; }
    public string? TextAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
