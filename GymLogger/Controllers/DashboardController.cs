using GymLogger.Data;
using GymLogger.Models;
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

        var recentPRs = await _context.PersonalRecords
            .Where(pr => pr.UserId == userId)
            .OrderByDescending(pr => pr.AchievedOn)
            .Take(5)
            .Select(pr => new PersonalRecordViewModel
            {
                Id = pr.Id,
                ExerciseName = pr.Exercise.Name,
                WeightKg = pr.WeightKg,
                Reps = pr.Reps,
                AchievedOn = pr.AchievedOn
            })
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
            ActiveProgram = activeProgram
        };

        return View(viewModel);
    }
}
