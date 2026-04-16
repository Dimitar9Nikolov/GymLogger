using GymLogger.Models.Enums;

namespace GymLogger.ViewModels;

public class ActiveProgramViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationWeeks { get; set; }
    public DateTime StartDate { get; set; }
    public Difficulty Difficulty { get; set; }
    public ProgramGoal Goal { get; set; }
}
