namespace LuxorLMS.Registration.Api.Authorization;

public static class RegistrationPermissions
{
    public const string PeriodManage = "registration:manage";
    public const string EnrollmentApprove = "enrollment:approve";
    public const string EnrollmentRegister = "course:register";

    public static IReadOnlyDictionary<string, string> Descriptions { get; } = new Dictionary<string, string>
    {
        [PeriodManage] = "Manage registration periods",
        [EnrollmentApprove] = "Approve/reject course enrollments",
        [EnrollmentRegister] = "Register for courses (student self-service)"
    };
}
