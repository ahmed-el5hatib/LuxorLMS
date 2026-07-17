namespace LuxorLMS.Quizzes.Domain.Enums;

public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    Essay = 3
}

public enum QuizAttemptStatus
{
    InProgress = 1,
    Submitted = 2,
    Graded = 3,
    Expired = 4
}
