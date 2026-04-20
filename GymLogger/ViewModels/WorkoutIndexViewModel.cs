namespace GymLogger.ViewModels;

public class WorkoutIndexViewModel
{
    public List<WorkoutSummaryViewModel> Workouts { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? Search { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
