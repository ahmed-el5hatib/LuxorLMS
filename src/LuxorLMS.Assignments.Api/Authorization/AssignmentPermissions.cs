namespace LuxorLMS.Assignments.Api.Authorization;

public static class AssignmentPermissions
{
    public const string Create = "assignment:create";
    public const string Edit = "assignment:edit";
    public const string Grade = "assignment:grade";
    public const string Submit = "assignment:submit";
    public const string View = "assignment:view";

    public static IReadOnlyDictionary<string, string> Descriptions { get; } = new Dictionary<string, string>
    {
        [Create] = "Create assignments and rubrics (instructor)",
        [Edit] = "Edit assignments and rubrics (instructor)",
        [Grade] = "Grade and return student submissions (instructor)",
        [Submit] = "Submit assignments (student self-service)",
        [View] = "View assignments, rubrics, and submissions"
    };
}
