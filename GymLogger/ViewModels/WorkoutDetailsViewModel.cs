namespace GymLogger.ViewModels;

public class WorkoutDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }
    public List<WorkoutExerciseViewModel> Exercises { get; set; } = [];
}
