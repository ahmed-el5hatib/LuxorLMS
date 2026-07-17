namespace LuxorLMS.Grading.Api.Authorization;

public static class GradingPermissions
{
    public const string SchemaManage = "grading:schema:manage";
    public const string GradeEnter = "grading:grade:enter";
    public const string GradeApprove = "grading:grade:approve";
    public const string GradePublish = "grading:grade:publish";
    public const string GradeView = "grading:grade:view";
    public const string AppealSubmit = "grading:appeal:submit";
    public const string AppealResolve = "grading:appeal:resolve";

    public static IReadOnlyDictionary<string, string> Descriptions { get; } = new Dictionary<string, string>
    {
        [SchemaManage] = "Manage grade categories/components (weighted schemas)",
        [GradeEnter] = "Enter/edit student grades (teacher)",
        [GradeApprove] = "Department Head approval of grades",
        [GradePublish] = "Academic Affairs publishing of grades",
        [GradeView] = "View student grades",
        [AppealSubmit] = "Submit a grade appeal (student self-service)",
        [AppealResolve] = "Resolve grade appeals"
    };
}
