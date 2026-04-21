using System.ComponentModel.DataAnnotations;

namespace GymLogger.ViewModels;

public class WorkoutExerciseInputModel
{
    [Required]
    public int ExerciseId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Not bound from form — set by controller for view rendering in Edit
    public bool IsCardio { get; set; }

    public List<WorkoutSetInputModel> Sets { get; set; } = [];
}
