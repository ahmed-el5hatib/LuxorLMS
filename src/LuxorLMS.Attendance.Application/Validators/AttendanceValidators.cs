using FluentValidation;
using LuxorLMS.Attendance.Application.DTOs;

namespace LuxorLMS.Attendance.Application.Validators;

public class CreateAttendanceSessionRequestValidator : AbstractValidator<CreateAttendanceSessionRequest>
{
    public CreateAttendanceSessionRequestValidator()
    {
        RuleFor(x => x.CourseOfferingId).NotEmpty();
        RuleFor(x => x.SessionDate).NotEmpty();
        RuleFor(x => x.SessionType).IsInEnum();
        RuleFor(x => x.TokenValidityMinutes).GreaterThan(0);
        When(x => x.Latitude.HasValue, () => RuleFor(x => x.Latitude).InclusiveBetween(-90.0, 90.0));
        When(x => x.Longitude.HasValue, () => RuleFor(x => x.Longitude).InclusiveBetween(-180.0, 180.0));
        When(x => x.GeofenceRadiusMeters.HasValue, () => RuleFor(x => x.GeofenceRadiusMeters).GreaterThan(0));
    }
}

public class UpdateAttendanceSessionRequestValidator : AbstractValidator<UpdateAttendanceSessionRequest>
{
    public UpdateAttendanceSessionRequestValidator()
    {
        RuleFor(x => x.SessionDate).NotEmpty();
        RuleFor(x => x.SessionType).IsInEnum();
        When(x => x.Latitude.HasValue, () => RuleFor(x => x.Latitude).InclusiveBetween(-90.0, 90.0));
        When(x => x.Longitude.HasValue, () => RuleFor(x => x.Longitude).InclusiveBetween(-180.0, 180.0));
        When(x => x.GeofenceRadiusMeters.HasValue, () => RuleFor(x => x.GeofenceRadiusMeters).GreaterThan(0));
    }
}

public class MarkAttendanceRequestValidator : AbstractValidator<MarkAttendanceRequest>
{
    public MarkAttendanceRequestValidator()
    {
        RuleFor(x => x.AttendanceSessionId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
        When(x => x.CheckedInLatitude.HasValue, () => RuleFor(x => x.CheckedInLatitude).InclusiveBetween(-90.0, 90.0));
        When(x => x.CheckedInLongitude.HasValue, () => RuleFor(x => x.CheckedInLongitude).InclusiveBetween(-180.0, 180.0));
        When(x => x.Notes != null, () => RuleFor(x => x.Notes).MaximumLength(2000));
    }
}
