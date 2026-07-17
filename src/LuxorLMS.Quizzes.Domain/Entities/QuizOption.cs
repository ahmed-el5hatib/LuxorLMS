namespace LuxorLMS.Quizzes.Domain.Entities;

public class QuizOption
{
    public Guid Id { get; set; }
    public Guid QuizQuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
