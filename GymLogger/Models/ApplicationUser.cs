using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GymLogger.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(2048)]
    public string? ProfilePictureUrl { get; set; }
    [MaxLength(500)]
    public string? Bio { get; set; }
    public DateTime CreatedOn { get; set; }

    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    public ICollection<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();
    public ICollection<PersonalRecord> PersonalRecords { get; set; } = new List<PersonalRecord>();
}
