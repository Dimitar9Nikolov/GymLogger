using System.ComponentModel.DataAnnotations;
using GymLogger.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymLogger.ViewModels;

public class ExerciseFormViewModel
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    [Display(Name = "Exercise Name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Instructions { get; set; }

    [Display(Name = "Image URL")]
    [MaxLength(2048)]
    public string? ImageUrl { get; set; }

    [Display(Name = "Muscle Group")]
    public MuscleGroup MuscleGroup { get; set; }

    public Difficulty Difficulty { get; set; }
    
    public SelectList? MuscleGroupOptions { get; set; }
    public SelectList? DifficultyOptions { get; set; }
}
