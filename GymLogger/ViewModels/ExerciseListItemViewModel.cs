using GymLogger.Models.Enums;

namespace GymLogger.ViewModels;

public class ExerciseListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public MuscleGroup MuscleGroup { get; set; }
    public Difficulty Difficulty { get; set; }
    public string? ImageUrl { get; set; }
}