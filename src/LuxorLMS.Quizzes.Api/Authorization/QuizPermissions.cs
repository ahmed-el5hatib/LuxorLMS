namespace LuxorLMS.Quizzes.Api.Authorization;

public static class QuizPermissions
{
    public const string Create = "quiz:create";
    public const string Edit = "quiz:edit";
    public const string Publish = "quiz:publish";
    public const string View = "quiz:view";
    public const string Attempt = "quiz:attempt";
    public const string Grade = "quiz:grade";

    public static IReadOnlyDictionary<string, string> Descriptions { get; } = new Dictionary<string, string>
    {
        [Create] = "Create quizzes, questions, and options",
        [Edit] = "Edit quizzes, questions, and options",
        [Publish] = "Publish or unpublish quizzes",
        [View] = "View quizzes, questions, attempts, and answers",
        [Attempt] = "Start, answer, and submit quiz attempts (student self-service)",
        [Grade] = "Manually grade essay answers"
    };
}
