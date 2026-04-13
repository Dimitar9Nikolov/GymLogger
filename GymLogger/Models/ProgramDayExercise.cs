using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLogger.Models;

public class ProgramDayExercise
{
    [Key]
    public int Id { get; set; }

    public int ProgramDayId { get; set; }
    [ForeignKey(nameof(ProgramDayId))]
    public ProgramDay ProgramDay { get; set; } = null!;

    public int ExerciseId { get; set; }
    [ForeignKey(nameof(ExerciseId))]
    public Exercise Exercise { get; set; } = null!;

    public int Sets { get; set; }
    public int Reps { get; set; }
    public int RestSeconds { get; set; }
    public int Order { get; set; }
}
