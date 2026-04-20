namespace GymLogger.ViewModels;

public class WorkoutSetViewModel
{
    public int SetNumber { get; set; }
    public int Reps { get; set; }
    public decimal WeightKg { get; set; }
}

public class WorkoutExerciseViewModel
{
    public int Order { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<WorkoutSetViewModel> Sets { get; set; } = [];
}
