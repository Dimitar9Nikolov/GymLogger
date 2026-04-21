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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetWeeklyGoal(int goal)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            user.WeeklyWorkoutGoal = Math.Max(1, Math.Min(goal, 30));
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction(nameof(Index));
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

        // Weekly date ranges (Mon–Sun)
        var today = DateTime.Today;
        int daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var thisWeekStart = today.AddDays(-daysFromMonday);
        var lastWeekStart = thisWeekStart.AddDays(-7);
        var lastWeekEnd   = thisWeekStart;
        var nextWeekStart = thisWeekStart.AddDays(7);

        // Workouts this week / last week
        var workoutsThisWeek = await _context.Workouts
            .CountAsync(w => w.UserId == userId && w.Date >= thisWeekStart && w.Date < nextWeekStart);
        var workoutsLastWeek = await _context.Workouts
            .CountAsync(w => w.UserId == userId && w.Date >= lastWeekStart && w.Date < lastWeekEnd);

        // Strength sets this week / last week (non-cardio with WeightKg)
        var setsThisWeek = await _context.WorkoutSets
            .Where(s => s.WorkoutExercise.Workout.UserId == userId
                     && s.WorkoutExercise.Exercise.MuscleGroup != MuscleGroup.Cardio
                     && s.WorkoutExercise.Workout.Date >= thisWeekStart
                     && s.WorkoutExercise.Workout.Date < nextWeekStart
                     && s.WeightKg != null && s.Reps != null)
            .Select(s => new { WeightKg = s.WeightKg!.Value, Reps = s.Reps!.Value })
            .ToListAsync();

        var setsLastWeek = await _context.WorkoutSets
            .Where(s => s.WorkoutExercise.Workout.UserId == userId
                     && s.WorkoutExercise.Exercise.MuscleGroup != MuscleGroup.Cardio
                     && s.WorkoutExercise.Workout.Date >= lastWeekStart
                     && s.WorkoutExercise.Workout.Date < lastWeekEnd
                     && s.WeightKg != null && s.Reps != null)
            .Select(s => new { WeightKg = s.WeightKg!.Value, Reps = s.Reps!.Value })
            .ToListAsync();

        var totalKgThisWeek = setsThisWeek.Sum(s => s.WeightKg * s.Reps);
        var totalKgLastWeek = setsLastWeek.Sum(s => s.WeightKg * s.Reps);

        // Cardio sets this week / last week
        var cardioThisWeek = await _context.WorkoutSets
            .Where(s => s.WorkoutExercise.Workout.UserId == userId
                     && s.WorkoutExercise.Exercise.MuscleGroup == MuscleGroup.Cardio
                     && s.WorkoutExercise.Workout.Date >= thisWeekStart
                     && s.WorkoutExercise.Workout.Date < nextWeekStart)
            .Select(s => new { DistanceKm = s.DistanceKm ?? 0m, DurationMinutes = s.DurationMinutes ?? 0 })
            .ToListAsync();

        var cardioLastWeek = await _context.WorkoutSets
            .Where(s => s.WorkoutExercise.Workout.UserId == userId
                     && s.WorkoutExercise.Exercise.MuscleGroup == MuscleGroup.Cardio
                     && s.WorkoutExercise.Workout.Date >= lastWeekStart
                     && s.WorkoutExercise.Workout.Date < lastWeekEnd)
            .Select(s => new { DistanceKm = s.DistanceKm ?? 0m, DurationMinutes = s.DurationMinutes ?? 0 })
            .ToListAsync();

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
            ActiveProgram = activeProgram,

            WeeklyWorkoutGoal   = user?.WeeklyWorkoutGoal,
            WorkoutsThisWeek    = workoutsThisWeek,
            WorkoutsLastWeek    = workoutsLastWeek,

            TotalKgThisWeek     = totalKgThisWeek,
            TotalKgLastWeek     = totalKgLastWeek,

            CardioKmThisWeek    = cardioThisWeek.Sum(s => s.DistanceKm),
            CardioKmLastWeek    = cardioLastWeek.Sum(s => s.DistanceKm),
            CardioMinsThisWeek  = cardioThisWeek.Sum(s => s.DurationMinutes),
            CardioMinsLastWeek  = cardioLastWeek.Sum(s => s.DurationMinutes)
        };

        return View(viewModel);
    }
}
