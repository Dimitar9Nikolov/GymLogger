using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLogger.Models;

public class WorkoutExercise
{
    [Key]
    public int Id { get; set; }

    public int WorkoutId { get; set; }
    [ForeignKey(nameof(WorkoutId))]
    public Workout Workout { get; set; } = null!;

    public int ExerciseId { get; set; }
    [ForeignKey(nameof(ExerciseId))]
    public Exercise Exercise { get; set; } = null!;

    public int Sets { get; set; }
    public int Reps { get; set; }
    public decimal WeightKg { get; set; }
    public string? Notes { get; set; }
    public int Order { get; set; }
}
