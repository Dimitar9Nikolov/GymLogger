using GymLogger.Models.Enums;

namespace GymLogger.ViewModels;

public class ExerciseIndexViewModel
{
    // The results
    public List<ExerciseListItemViewModel> Exercises { get; set; } = [];

    // Pagination
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    // Filter state — needed to repopulate the filter bar and build pagination links
    public string? Search { get; set; }
    public MuscleGroup? SelectedMuscleGroup { get; set; }
}