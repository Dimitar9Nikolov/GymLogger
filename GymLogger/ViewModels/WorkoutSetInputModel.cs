using System.ComponentModel.DataAnnotations;

namespace GymLogger.ViewModels;

public class WorkoutSetInputModel
{
    [Required]
    [Range(1, 200, ErrorMessage = "Reps must be between 1 and 200.")]
    public int Reps { get; set; } = 10;

    [Required]
    [Range(0, 1000, ErrorMessage = "Weight must be between 0 and 1000 kg.")]
    public decimal WeightKg { get; set; } = 0;
}
