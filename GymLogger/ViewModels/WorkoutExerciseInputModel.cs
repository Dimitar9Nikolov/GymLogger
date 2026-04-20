using System.ComponentModel.DataAnnotations;

namespace GymLogger.ViewModels;

public class WorkoutExerciseInputModel
{
    [Required]
    public int ExerciseId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Each item is one set with its own reps and weight
    public List<WorkoutSetInputModel> Sets { get; set; } = [];
}
