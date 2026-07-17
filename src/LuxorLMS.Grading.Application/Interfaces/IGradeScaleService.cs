namespace LuxorLMS.Grading.Application.Interfaces;

/// <summary>
/// Maps a raw percentage score (0-100) to a standard 4.0-scale letter grade and grade points.
/// </summary>
public interface IGradeScaleService
{
    string ToLetter(decimal rawScore);
    decimal ToGradePoints(string letter);
    bool CountsTowardGpa(string letter);
}
