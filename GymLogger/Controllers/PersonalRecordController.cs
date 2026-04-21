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
        // best distance per cardio exercise.
        var allSets = await _context.WorkoutSets
            .Where(s => s.WorkoutExercise.Workout.UserId == userId)
            .Select(s => new
            {
                ExerciseId    = s.WorkoutExercise.ExerciseId,
                ExerciseName  = s.WorkoutExercise.Exercise.Name,
                MuscleGroup   = s.WorkoutExercise.Exercise.MuscleGroup,
                IsCardio      = s.WorkoutExercise.Exercise.MuscleGroup == MuscleGroup.Cardio,
                WeightKg      = s.WeightKg,
                Reps          = s.Reps,
                DistanceKm    = s.DistanceKm,
                DurationMinutes = s.DurationMinutes,
                Date          = s.WorkoutExercise.Workout.Date
            })
            .ToListAsync();

        var strengthPRs = allSets
            .Where(s => !s.IsCardio && s.WeightKg.HasValue)
            .GroupBy(s => s.ExerciseId)
            .Select(g => g.OrderByDescending(s => s.WeightKg).ThenByDescending(s => s.Date).First())
            .Select(s => new PersonalRecordIndexViewModel
            {
                ExerciseName = s.ExerciseName,
                MuscleGroup  = s.MuscleGroup.ToString(),
                IsCardio     = false,
                WeightKg     = s.WeightKg,
                Reps         = s.Reps,
                Date         = s.Date
            })
            .OrderBy(s => s.ExerciseName)
            .ToList();

        var cardioPRs = allSets
            .Where(s => s.IsCardio && s.DistanceKm.HasValue)
            .GroupBy(s => s.ExerciseId)
            .Select(g => g.OrderByDescending(s => s.DistanceKm).ThenByDescending(s => s.Date).First())
            .Select(s => new PersonalRecordIndexViewModel
            {
                ExerciseName    = s.ExerciseName,
                MuscleGroup     = s.MuscleGroup.ToString(),
                IsCardio        = true,
                DistanceKm      = s.DistanceKm,
                DurationMinutes = s.DurationMinutes,
                Date            = s.Date
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
