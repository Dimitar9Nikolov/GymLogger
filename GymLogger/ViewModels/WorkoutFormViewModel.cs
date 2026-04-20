using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymLogger.ViewModels;

public class WorkoutFormViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    
    [Required]
    [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
    [Display(Name = "Duration (minutes)")]
    public int DurationMinutes { get; set; }
    public List<WorkoutExerciseInputModel> Exercises { get; set; } = [];
    public SelectList? ExerciseOptions { get; set; }
}
