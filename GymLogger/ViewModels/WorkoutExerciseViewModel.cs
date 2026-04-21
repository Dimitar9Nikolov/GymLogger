namespace GymLogger.ViewModels;

public class WorkoutSetViewModel
{
    public int SetNumber { get; set; }
    // Strength
    public int? Reps { get; set; }
    public decimal? WeightKg { get; set; }
    // Cardio
    public int? DurationMinutes { get; set; }
    public decimal? DistanceKm { get; set; }
}

public class WorkoutExerciseViewModel
{
    public int Order { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsCardio { get; set; }
    public List<WorkoutSetViewModel> Sets { get; set; } = [];
}
