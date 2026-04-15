using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymLogger.Models.Enums;
using GymLogger.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymLogger.Models;

public class TrainingProgram
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string Title { get; set; } = null!;
    [MaxLength(1000)]
    public string? Description { get; set; }
    public int DurationWeeks { get; set; }
    public Difficulty Difficulty { get; set; }
    public ProgramGoal Goal { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedOn { get; set; }

    public string CreatedById { get; set; } = null!;
    [ForeignKey(nameof(CreatedById))]
    public ApplicationUser CreatedBy { get; set; } = null!;

    public ICollection<ProgramDay> ProgramDays { get; set; } = new List<ProgramDay>();
    public ICollection<UserProgram> UserPrograms { get; set; } = new List<UserProgram>();
}
