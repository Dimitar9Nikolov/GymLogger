using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

    [MaxLength(500)]
    public string? Notes { get; set; }
    public int Order { get; set; }

    public ICollection<WorkoutSet> Sets { get; set; } = new List<WorkoutSet>();
}
