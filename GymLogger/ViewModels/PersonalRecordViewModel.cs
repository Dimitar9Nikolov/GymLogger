namespace GymLogger.ViewModels;

public class PersonalRecordViewModel
{
    public string ExerciseName { get; set; } = string.Empty;
    public bool IsCardio { get; set; }

    // Strength
    public decimal WeightKg { get; set; }
    public int? Reps { get; set; }

    // Cardio
    public decimal? DistanceKm { get; set; }
    public int? DurationMinutes { get; set; }
    public double? SpeedKmh { get; set; }

    public DateTime AchievedOn { get; set; }
}
