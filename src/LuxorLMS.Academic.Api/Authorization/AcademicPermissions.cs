namespace LuxorLMS.Academic.Api.Authorization;

public static class AcademicPermissions
{
    public const string FacultyRead = "faculty:read";
    public const string FacultyManage = "faculty:manage";
    public const string DepartmentRead = "department:read";
    public const string DepartmentManage = "department:manage";
    public const string ProgramRead = "program:read";
    public const string ProgramManage = "program:manage";
    public const string StudyPlanRead = "studyplan:read";
    public const string StudyPlanManage = "studyplan:manage";
    public const string CourseRead = "course:read";
    public const string CourseManage = "course:manage";
    public const string YearManage = "year:manage";
    public const string SemesterManage = "semester:manage";
    public const string OfferingManage = "offering:manage";
    public const string SectionManage = "section:manage";

    public static IReadOnlyDictionary<string, string> Descriptions { get; } = new Dictionary<string, string>
    {
        [FacultyRead] = "View faculty data",
        [FacultyManage] = "Manage faculties",
        [DepartmentRead] = "View department data",
        [DepartmentManage] = "Manage departments",
        [ProgramRead] = "View programs",
        [ProgramManage] = "Manage programs",
        [StudyPlanRead] = "View study plans",
        [StudyPlanManage] = "Manage study plans",
        [CourseRead] = "View courses",
        [CourseManage] = "Manage courses",
        [YearManage] = "Manage academic years",
        [SemesterManage] = "Manage semesters",
        [OfferingManage] = "Manage course offerings",
        [SectionManage] = "Manage sections"
    };
}
