namespace GymLogger.ViewModels;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public int TotalWorkouts { get; set; }
    public int TotalExercisesLogged { get; set; }
    public List<RecentWorkoutViewModel> RecentWorkouts { get; set; } = [];
    public List<PersonalRecordViewModel> RecentPersonalRecords { get; set; } = [];
    public ActiveProgramViewModel? ActiveProgram { get; set; }

    // Weekly stats
    public int? WeeklyWorkoutGoal { get; set; }
    public int WorkoutsThisWeek { get; set; }
    public int WorkoutsLastWeek { get; set; }

    public decimal TotalKgThisWeek { get; set; }
    public decimal TotalKgLastWeek { get; set; }

    public decimal CardioKmThisWeek { get; set; }
    public decimal CardioKmLastWeek { get; set; }
    public int CardioMinsThisWeek { get; set; }
    public int CardioMinsLastWeek { get; set; }
}
