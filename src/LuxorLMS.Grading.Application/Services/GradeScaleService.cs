using LuxorLMS.Grading.Application.Interfaces;

namespace LuxorLMS.Grading.Application.Services;

/// <summary>
/// Standard Egyptian-university-style 4.0 GPA scale. 'W' (withdrawal) is excluded from GPA.
/// </summary>
public class GradeScaleService : IGradeScaleService
{
    public string ToLetter(decimal rawScore)
    {
        return rawScore switch
        {
            >= 93 => "A",
            >= 90 => "A-",
            >= 87 => "B+",
            >= 83 => "B",
            >= 80 => "B-",
            >= 77 => "C+",
            >= 73 => "C",
            >= 70 => "C-",
            >= 67 => "D+",
            >= 60 => "D",
            _ => "F"
        };
    }

    public decimal ToGradePoints(string letter)
    {
        return letter switch
        {
            "A" => 4.0m,
            "A-" => 3.7m,
            "B+" => 3.3m,
            "B" => 3.0m,
            "B-" => 2.7m,
            "C+" => 2.3m,
            "C" => 2.0m,
            "C-" => 1.7m,
            "D+" => 1.3m,
            "D" => 1.0m,
            "F" => 0.0m,
            _ => 0.0m
        };
    }

    public bool CountsTowardGpa(string letter)
        => !string.Equals(letter, "W", StringComparison.OrdinalIgnoreCase);
}
