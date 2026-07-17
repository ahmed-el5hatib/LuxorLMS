namespace LuxorLMS.Registration.Domain.Enums;

public enum EnrollmentStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Withdrawn = 4
}

public enum StudentProgramStatus
{
    Active = 1,
    Graduated = 2,
    Withdrawn = 3,
    Suspended = 4
}

public enum EnrollmentType
{
    Regular = 1,
    Late = 2,
    Withdraw = 3
}
