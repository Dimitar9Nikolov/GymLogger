using GymLogger.Models.Enums;

namespace GymLogger.ViewModels;

public class ExercisePendingViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MuscleGroup MuscleGroup { get; set; }
    public Difficulty Difficulty { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
}
