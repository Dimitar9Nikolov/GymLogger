using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymLogger.Models.Enums;

namespace GymLogger.Models;

public class Exercise
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    [MaxLength(500)]
    public string? Description { get; set; }
    [MaxLength(2000)]
    public string? Instructions { get; set; }
    [MaxLength(2048)]
    public string? ImageUrl { get; set; }
    public MuscleGroup MuscleGroup { get; set; }
    public Difficulty Difficulty { get; set; }
    public bool IsApproved { get; set; }

    public string CreatedById { get; set; } = null!;
    [ForeignKey(nameof(CreatedById))]
    public ApplicationUser? CreatedBy { get; set; } = null!;

    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    public ICollection<PersonalRecord> PersonalRecords { get; set; } = new List<PersonalRecord>();
}
