namespace GymLogger.ViewModels;

public class WorkoutSummaryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }
    public int ExerciseCount { get; set; }
}
