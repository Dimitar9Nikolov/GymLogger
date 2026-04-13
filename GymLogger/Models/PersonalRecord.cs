using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLogger.Models;

public class PersonalRecord
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    public int ExerciseId { get; set; }
    [ForeignKey(nameof(ExerciseId))]
    public Exercise Exercise { get; set; } = null!;

    public decimal WeightKg { get; set; }
    public int Reps { get; set; }
    public DateTime AchievedOn { get; set; }
}
