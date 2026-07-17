namespace LuxorLMS.Grading.Domain.Enums;

public enum GradePublishStatus
{
    Draft = 1,
    PendingDeptHead = 2,
    DeptHeadApproved = 3,
    Published = 4,
    Rejected = 5
}

public enum AppealStatus
{
    Open = 1,
    Approved = 2,
    Rejected = 3
}
