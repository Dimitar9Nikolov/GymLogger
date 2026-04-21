using System.ComponentModel.DataAnnotations;

namespace GymLogger.ViewModels;

public class WorkoutSetInputModel
{
    // Strength fields
    [Range(1, 200, ErrorMessage = "Reps must be between 1 and 200.")]
    public int? Reps { get; set; }

    [Range(0, 1000, ErrorMessage = "Weight must be between 0 and 1000 kg.")]
    public decimal? WeightKg { get; set; }

    // Cardio fields
    [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
    public int? DurationMinutes { get; set; }

    [Range(0, 1000, ErrorMessage = "Distance must be between 0 and 1000 km.")]
    public decimal? DistanceKm { get; set; }
}
