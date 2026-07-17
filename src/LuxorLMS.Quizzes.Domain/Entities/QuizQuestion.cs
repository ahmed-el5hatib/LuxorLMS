using LuxorLMS.Quizzes.Domain.Enums;

namespace LuxorLMS.Quizzes.Domain.Entities;

public class QuizQuestion
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public QuestionType QuestionType { get; set; }
    public string Text { get; set; } = string.Empty;
    public decimal Points { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
