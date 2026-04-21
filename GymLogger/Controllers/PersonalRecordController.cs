using GymLogger.Data;
using GymLogger.Models.Enums;
using GymLogger.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymLogger.Controllers;

[Authorize]
public class PersonalRecordController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Models.ApplicationUser> _userManager;

    public PersonalRecordController(ApplicationDbContext context, UserManager<Models.ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        // Derive PRs from WorkoutSets: best weight per strength exercise,
        // best speed per cardio exercise.
        var allSets = await _context.WorkoutSets
            .Where(s => s.WorkoutExercise.Workout.UserId == userId)
            .Select(s => new
            {
                ExerciseId      = s.WorkoutExercise.ExerciseId,
                ExerciseName    = s.WorkoutExercise.Exercise.Name,
                MuscleGroup     = s.WorkoutExercise.Exercise.MuscleGroup,
                IsCardio        = s.WorkoutExercise.Exercise.MuscleGroup == MuscleGroup.Cardio,
                WeightKg        = s.WeightKg,
                Reps            = s.Reps,
                DistanceKm      = s.DistanceKm,
                DurationMinutes = s.DurationMinutes,
                Date            = s.WorkoutExercise.Workout.Date
            })
            .ToListAsync();

        var strengthPRs = allSets
            .Where(s => !s.IsCardio)
            .GroupBy(s => s.ExerciseId)
            .Select(g =>
            {
                var best = g.OrderByDescending(s => s.WeightKg ?? 0m).ThenByDescending(s => s.Date).First();
                return new PersonalRecordIndexViewModel
                {
                    ExerciseName = best.ExerciseName,
                    MuscleGroup  = best.MuscleGroup.ToString(),
                    IsCardio     = false,
                    WeightKg     = best.WeightKg ?? 0m,
                    Reps         = best.Reps,
                    Date         = best.Date
                };
            })
            .OrderBy(s => s.ExerciseName)
            .ToList();

        var cardioPRs = allSets
            .Where(s => s.IsCardio)
            .GroupBy(s => s.ExerciseId)
            .Select(g =>
            {
                // Prefer sets with both distance and duration (so we can calc speed)
                var withSpeed = g.Where(s => s.DistanceKm.HasValue && s.DurationMinutes is > 0).ToList();
                var best = withSpeed.Any()
                    ? withSpeed.OrderByDescending(s => s.DistanceKm!.Value / (s.DurationMinutes!.Value / 60m)).First()
                    : g.OrderByDescending(s => s.DistanceKm)
                        .ThenByDescending(s => s.DurationMinutes)
                        .ThenByDescending(s => s.Date)
                        .First();

                double? speed = (best.DistanceKm.HasValue && best.DurationMinutes is > 0)
                    ? Math.Round((double)(best.DistanceKm!.Value / (best.DurationMinutes!.Value / 60m)), 2)
                    : null;

                return new PersonalRecordIndexViewModel
                {
                    ExerciseName    = best.ExerciseName,
                    MuscleGroup     = best.MuscleGroup.ToString(),
                    IsCardio        = true,
                    DistanceKm      = best.DistanceKm,
                    DurationMinutes = best.DurationMinutes,
                    SpeedKmh        = speed,
                    Date            = best.Date
                };
            })
            .OrderBy(s => s.ExerciseName)
            .ToList();

        var vm = new PersonalRecordListViewModel
        {
            StrengthRecords = strengthPRs,
            CardioRecords   = cardioPRs
        };

        return View(vm);
    }
}
