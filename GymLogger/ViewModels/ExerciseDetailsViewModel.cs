using GymLogger.Models.Enums;

namespace GymLogger.ViewModels;

public class ExerciseDetailsViewModel
{
    // Display fields
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public string? ImageUrl { get; set; }
    public MuscleGroup MuscleGroup { get; set; }
    public Difficulty Difficulty { get; set; }
    public bool IsApproved { get; set; }
    public required string CreatedByName { get; set; }  

    // Controls what the view renders
    public bool CanEdit { get; set; }    // true if owner OR Admin
    public bool CanApprove { get; set; } // true if Admin AND !IsApproved
}