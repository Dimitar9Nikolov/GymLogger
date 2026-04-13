using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLogger.Models;

public class ProgramDay
{
    [Key]
    public int Id { get; set; }

    public int ProgramId { get; set; }
    [ForeignKey(nameof(ProgramId))]
    public TrainingProgram Program { get; set; } = null!;

    public int DayNumber { get; set; }
    public string Name { get; set; } = null!;
    public string? Notes { get; set; }

    public ICollection<ProgramDayExercise> ProgramDayExercises { get; set; } = new List<ProgramDayExercise>();
}
