namespace GymLogger.ViewModels;

public class DashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public int TotalWorkouts { get; set; }
    public int TotalExercisesLogged { get; set; }
    public List<RecentWorkoutViewModel> RecentWorkouts { get; set; } = [];
    public List<PersonalRecordViewModel> RecentPersonalRecords { get; set; } = [];
    public ActiveProgramViewModel? ActiveProgram { get; set; }
}
