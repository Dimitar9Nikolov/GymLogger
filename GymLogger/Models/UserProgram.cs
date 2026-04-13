using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymLogger.Models;

public class UserProgram
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;

    public int ProgramId { get; set; }
    [ForeignKey(nameof(ProgramId))]
    public TrainingProgram Program { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public bool IsActive { get; set; }
}
