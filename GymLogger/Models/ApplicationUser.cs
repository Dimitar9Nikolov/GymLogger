using Microsoft.AspNetCore.Identity;

namespace GymLogger.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedOn { get; set; }

    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    public ICollection<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();
    public ICollection<PersonalRecord> PersonalRecords { get; set; } = new List<PersonalRecord>();
}
