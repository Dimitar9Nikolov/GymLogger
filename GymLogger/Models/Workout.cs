using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLogger.Models;

public class Workout
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public int DurationMinutes { get; set; }

    public string UserId { get; set; } = null!;
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}
