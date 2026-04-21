using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GymLogger.Models;

public class WorkoutSet
{
    [Key]
    public int Id { get; set; }

    public int WorkoutExerciseId { get; set; }
    [ForeignKey(nameof(WorkoutExerciseId))]
    public WorkoutExercise WorkoutExercise { get; set; } = null!;

    public int SetNumber { get; set; }

    // Strength fields (null for cardio exercises)
    public int? Reps { get; set; }

    [Precision(6, 2)]
    public decimal? WeightKg { get; set; }

    // Cardio fields (null for strength exercises)
    public int? DurationMinutes { get; set; }

    [Precision(6, 2)]
    public decimal? DistanceKm { get; set; }
}
