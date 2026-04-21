namespace GymLogger.ViewModels;

public class PersonalRecordIndexViewModel
{
    public string ExerciseName { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public bool IsCardio { get; set; }

    // Strength
    public decimal WeightKg { get; set; }
    public int? Reps { get; set; }

    // Cardio
    public decimal? DistanceKm { get; set; }
    public int? DurationMinutes { get; set; }
    public double? SpeedKmh { get; set; }   // calculated: km/h

    public DateTime Date { get; set; }
}

public class PersonalRecordListViewModel
{
    public List<PersonalRecordIndexViewModel> StrengthRecords { get; set; } = [];
    public List<PersonalRecordIndexViewModel> CardioRecords { get; set; } = [];
}
