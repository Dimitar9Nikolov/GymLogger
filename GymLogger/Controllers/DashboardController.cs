using GymLogger.Data;
using GymLogger.Models;
using GymLogger.Models.Enums;
using GymLogger.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymLogger.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var totalWorkouts = await _context.Workouts
            .CountAsync(w => w.UserId == userId);

        var totalExercisesLogged = await _context.WorkoutExercises
            .CountAsync(we => we.Workout.UserId == userId);

        var recentWorkouts = await _context.Workouts
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.Date)
            .Take(5)
            .Select(w => new RecentWorkoutViewModel
            {
                Id = w.Id,
                Title = w.Title,
                Date = w.Date,
                DurationMinutes = w.DurationMinutes,
                ExerciseCount = w.WorkoutExercises.Count
            })
            .ToListAsync();

        // Derive PRs from WorkoutSets (PersonalRecords table is not auto-populated)
        var allSets = await _context.WorkoutSets
            .Where(s => s.WorkoutExercise.Workout.UserId == userId)
            .Select(s => new
            {
                ExerciseId      = s.WorkoutExercise.ExerciseId,
                ExerciseName    = s.WorkoutExercise.Exercise.Name,
                IsCardio        = s.WorkoutExercise.Exercise.MuscleGroup == MuscleGroup.Cardio,
                WeightKg        = s.WeightKg,
                Reps            = s.Reps,
                DistanceKm      = s.DistanceKm,
                DurationMinutes = s.DurationMinutes,
                Date            = s.WorkoutExercise.Workout.Date
            })
            .ToListAsync();

        // Best weight per strength exercise
        var strengthPRs = allSets
            .Where(s => !s.IsCardio)
            .GroupBy(s => s.ExerciseId)
            .Select(g =>
            {
                var best = g.OrderByDescending(s => s.WeightKg ?? 0m).ThenByDescending(s => s.Date).First();
                return new PersonalRecordViewModel
                {
                    ExerciseName = best.ExerciseName,
                    IsCardio     = false,
                    WeightKg     = best.WeightKg ?? 0m,
                    Reps         = best.Reps,
                    AchievedOn   = best.Date
                };
            });

        // Best speed (or distance) per cardio exercise
        var cardioPRs = allSets
            .Where(s => s.IsCardio)
            .GroupBy(s => s.ExerciseId)
            .Select(g =>
            {
                var withSpeed = g.Where(s => s.DistanceKm.HasValue && s.DurationMinutes is > 0).ToList();
                var best = withSpeed.Any()
                    ? withSpeed.OrderByDescending(s => s.DistanceKm!.Value / (s.DurationMinutes!.Value / 60m)).First()
                    : g.OrderByDescending(s => s.DistanceKm).ThenByDescending(s => s.Date).First();

                double? speed = (best.DistanceKm.HasValue && best.DurationMinutes is > 0)
                    ? Math.Round((double)(best.DistanceKm!.Value / (best.DurationMinutes!.Value / 60m)), 2)
                    : null;

                return new PersonalRecordViewModel
                {
                    ExerciseName    = best.ExerciseName,
                    IsCardio        = true,
                    DistanceKm      = best.DistanceKm,
                    DurationMinutes = best.DurationMinutes,
                    SpeedKmh        = speed,
                    AchievedOn      = best.Date
                };
            });

        var recentPRs = strengthPRs.Concat(cardioPRs)
            .OrderByDescending(pr => pr.AchievedOn)
            .Take(5)
            .ToList();

        var activeProgram = await _context.UserPrograms
            .Where(up => up.UserId == userId && up.IsActive)
            .Select(up => new ActiveProgramViewModel
            {
                Id = up.Program.Id,
                Title = up.Program.Title,
                Description = up.Program.Description ?? string.Empty,
                DurationWeeks = up.Program.DurationWeeks,
                StartDate = up.StartDate,
                Difficulty = up.Program.Difficulty,
                Goal = up.Program.Goal
            })
            .FirstOrDefaultAsync();

        var user = await _userManager.GetUserAsync(User);

        var viewModel = new DashboardViewModel
        {
            UserName = user?.FirstName ?? user?.UserName ?? "Athlete",
            TotalWorkouts = totalWorkouts,
            TotalExercisesLogged = totalExercisesLogged,
            RecentWorkouts = recentWorkouts,
            RecentPersonalRecords = recentPRs,
            ActiveProgram = activeProgram
        };

        return View(viewModel);
    }
}
