using GymLogger.Data;
using GymLogger.Models;
using GymLogger.Models.Enums;
using GymLogger.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymLogger.Controllers;

[Authorize]
public class WorkoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public WorkoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search, DateTime? from, DateTime? to, int page = 1)
    {
        var userId = _userManager.GetUserId(User);

        var query = _context.Workouts
            .Where(w => w.UserId == userId);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(w => w.Title.Contains(search));

        if (from.HasValue)
            query = query.Where(w => w.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(w => w.Date <= to.Value);

        int pageSize = 9;
        int totalWorkouts = await query.CountAsync();

        var workouts = await query
            .OrderByDescending(w => w.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(w => new WorkoutSummaryViewModel
            {
                Id = w.Id,
                Title = w.Title,
                Date = w.Date,
                DurationMinutes = w.DurationMinutes,
                ExerciseCount = w.WorkoutExercises.Count
            })
            .ToListAsync();

        var viewModel = new WorkoutIndexViewModel
        {
            Workouts = workouts,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalWorkouts / (double)pageSize),
            Search = search,
            From = from,
            To = to
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);

        var workout = await _context.Workouts
            .Where(w => w.Id == id && w.UserId == userId)
            .Select(w => new WorkoutDetailsViewModel
            {
                Id = w.Id,
                Title = w.Title,
                Notes = w.Notes,
                Date = w.Date,
                DurationMinutes = w.DurationMinutes,
                Exercises = w.WorkoutExercises
                    .OrderBy(we => we.Order)
                    .Select(we => new WorkoutExerciseViewModel
                    {
                        Order = we.Order,
                        ExerciseName = we.Exercise.Name,
                        Notes = we.Notes,
                        IsCardio = we.Exercise.MuscleGroup == MuscleGroup.Cardio,
                        Sets = we.Sets
                            .OrderBy(s => s.SetNumber)
                            .Select(s => new WorkoutSetViewModel
                            {
                                SetNumber = s.SetNumber,
                                Reps = s.Reps,
                                WeightKg = s.WeightKg,
                                DurationMinutes = s.DurationMinutes,
                                DistanceKm = s.DistanceKm
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (workout == null)
            return NotFound();

        return View(workout);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new WorkoutFormViewModel
        {
            Date = DateTime.Today
        };

        var exercises = await _context.Exercises
            .Where(e => e.IsApproved)
            .OrderBy(e => e.Name)
            .ToListAsync();

        viewModel.ExerciseOptions = new SelectList(exercises, "Id", "Name");
        viewModel.CardioExerciseIds = exercises
            .Where(e => e.MuscleGroup == MuscleGroup.Cardio)
            .Select(e => e.Id)
            .ToHashSet();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutFormViewModel model)
    {
        if(!ModelState.IsValid)
        {
            var exercises = await _context.Exercises
                .Where(e => e.IsApproved)
                .OrderBy(e => e.Name)
                .ToListAsync();
            model.ExerciseOptions = new SelectList(exercises, "Id", "Name");
            model.CardioExerciseIds = exercises
                .Where(e => e.MuscleGroup == MuscleGroup.Cardio)
                .Select(e => e.Id)
                .ToHashSet();
            return View(model);
        }

        var workout = new Workout
        {
            Title = model.Title,
            Notes = model.Notes,
            Date = model.Date,
            DurationMinutes = model.DurationMinutes,
            UserId = _userManager.GetUserId(User)!
        };

        for (int i = 0; i < model.Exercises.Count; i++)
        {
            var exerciseInput = model.Exercises[i];
            var workoutExercise = new WorkoutExercise
            {
                ExerciseId = exerciseInput.ExerciseId,
                Notes = exerciseInput.Notes,
                Order = i + 1
            };

            for (int s = 0; s < exerciseInput.Sets.Count; s++)
            {
                workoutExercise.Sets.Add(new WorkoutSet
                {
                    SetNumber = s + 1,
                    Reps = exerciseInput.Sets[s].Reps,
                    WeightKg = exerciseInput.Sets[s].WeightKg,
                    DurationMinutes = exerciseInput.Sets[s].DurationMinutes,
                    DistanceKm = exerciseInput.Sets[s].DistanceKm
                });
            }

            workout.WorkoutExercises.Add(workoutExercise);
        }

        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Workout created successfully.";
        return RedirectToAction("Details", new { id = workout.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var workout = await _context.Workouts
            .Where(w => w.Id == id && w.UserId == _userManager.GetUserId(User))
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Sets)
            .FirstOrDefaultAsync();

            if (workout == null)
                return NotFound();

        var exercises = await _context.Exercises
            .Where(e => e.IsApproved)
            .OrderBy(e => e.Name)
            .ToListAsync();

        var cardioIds = exercises
            .Where(e => e.MuscleGroup == MuscleGroup.Cardio)
            .Select(e => e.Id)
            .ToHashSet();

        var viewModel = new WorkoutFormViewModel
        {
            Id = workout.Id,
            Title = workout.Title,
            Notes = workout.Notes,
            Date = workout.Date,
            DurationMinutes = workout.DurationMinutes,
            CardioExerciseIds = cardioIds,
            Exercises = workout.WorkoutExercises
                .OrderBy(we => we.Order)
                .Select(we => new WorkoutExerciseInputModel
                {
                    ExerciseId = we.ExerciseId,
                    Notes = we.Notes,
                    IsCardio = cardioIds.Contains(we.ExerciseId),
                    Sets = we.Sets
                        .OrderBy(s => s.SetNumber)
                        .Select(s => new WorkoutSetInputModel
                        {
                            Reps = s.Reps,
                            WeightKg = s.WeightKg,
                            DurationMinutes = s.DurationMinutes,
                            DistanceKm = s.DistanceKm
                        })
                        .ToList()
                })
                .ToList()
        };

        viewModel.ExerciseOptions = new SelectList(exercises, "Id", "Name");
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutFormViewModel model)
    {
        if(!ModelState.IsValid)
        {
            var exercises = await _context.Exercises
                .Where(e => e.IsApproved)
                .OrderBy(e => e.Name)
                .ToListAsync();
            model.ExerciseOptions = new SelectList(exercises, "Id", "Name");
            model.CardioExerciseIds = exercises
                .Where(e => e.MuscleGroup == MuscleGroup.Cardio)
                .Select(e => e.Id)
                .ToHashSet();
            return View(model);
        }

        var workout = await _context.Workouts
            .Where(w => w.Id == id && w.UserId == _userManager.GetUserId(User))
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Sets)
            .FirstOrDefaultAsync();

        if (workout == null)
            return NotFound();

        workout.Title = model.Title;
        workout.Notes = model.Notes;
        workout.Date = model.Date;
        workout.DurationMinutes = model.DurationMinutes;

        _context.WorkoutExercises.RemoveRange(workout.WorkoutExercises);

        for (int i = 0; i < model.Exercises.Count; i++)
        {
            var exerciseInput = model.Exercises[i];
            var workoutExercise = new WorkoutExercise
            {
                ExerciseId = exerciseInput.ExerciseId,
                Notes = exerciseInput.Notes,
                Order = i + 1
            };

            for (int s = 0; s < exerciseInput.Sets.Count; s++)
            {
                workoutExercise.Sets.Add(new WorkoutSet
                {
                    SetNumber = s + 1,
                    Reps = exerciseInput.Sets[s].Reps,
                    WeightKg = exerciseInput.Sets[s].WeightKg,
                    DurationMinutes = exerciseInput.Sets[s].DurationMinutes,
                    DistanceKm = exerciseInput.Sets[s].DistanceKm
                });
            }

            workout.WorkoutExercises.Add(workoutExercise);
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Workout updated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        var workout = await _context.Workouts
            .Where(w => w.Id == id && w.UserId == userId)
            .FirstOrDefaultAsync();

        if (workout == null)
            return NotFound();

        return View(workout);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = _userManager.GetUserId(User);
        var workout = await _context.Workouts
            .Where(w => w.Id == id && w.UserId == userId)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Sets)
            .FirstOrDefaultAsync();

        if (workout == null)
            return NotFound();

        _context.Workouts.Remove(workout);

        await _context.SaveChangesAsync();
        TempData["Success"] = "Workout deleted successfully.";
        
        return RedirectToAction(nameof(Index));
    }
}
