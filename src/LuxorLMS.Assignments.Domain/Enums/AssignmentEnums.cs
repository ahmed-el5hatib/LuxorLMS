namespace LuxorLMS.Assignments.Domain.Enums;

public enum AssignmentStatus
{
    Draft = 1,
    Published = 2,
    Closed = 3,
    Archived = 4
}

public enum SubmissionStatus
{
    Pending = 1,
    Submitted = 2,
    Graded = 3,
    Late = 4,
    Returned = 5
}
