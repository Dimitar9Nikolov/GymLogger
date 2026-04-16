namespace GymLogger.ViewModels;

public class PersonalRecordViewModel
{
    public int Id { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public int Reps { get; set; }
    public DateTime AchievedOn { get; set; }
}
