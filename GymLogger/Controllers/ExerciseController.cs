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

public class ExerciseController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public ExerciseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search, MuscleGroup? muscleGroup, int page = 1)
    {
        var query = _context.Exercises.Where(e => e.IsApproved);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Name.Contains(search));
        
        if(muscleGroup.HasValue)
            query = query.Where(e => e.MuscleGroup == muscleGroup.Value);

        int pageSize = 9;
        int totalExercises = await query.CountAsync();
        
        var exercises = await query
            .OrderBy(e => e.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = new ExerciseIndexViewModel
        {
            Exercises = exercises.Select(e => new ExerciseListItemViewModel
            {
                Id = e.Id,
                Name = e.Name,
                MuscleGroup = e.MuscleGroup,
                Difficulty = e.Difficulty,
                ImageUrl = e.ImageUrl
            }).ToList(),
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalExercises / (double)pageSize),
            Search = search,
            SelectedMuscleGroup = muscleGroup
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        
        var exercise = await _context.Exercises
            .Include(e => e.CreatedBy)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (exercise == null)
            return NotFound();
        
        var userId = _userManager.GetUserId(User);
        
        bool isOwner = exercise.CreatedById == userId;
        bool isAdmin = User.IsInRole("Admin");
        
        if (!exercise.IsApproved && !isOwner && !isAdmin)
            return Forbid();
        
        var viewModel = new ExerciseDetailsViewModel
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            Instructions = exercise.Instructions,
            ImageUrl = exercise.ImageUrl,
            MuscleGroup = exercise.MuscleGroup,
            Difficulty = exercise.Difficulty,
            IsApproved = exercise.IsApproved,
            CreatedByName = $"{exercise.CreatedBy.FirstName} {exercise.CreatedBy.LastName}",
            CanEdit = isOwner || isAdmin,
            CanApprove = isAdmin && !exercise.IsApproved
        };
        
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var exerciseViewModel = new ExerciseFormViewModel();
        
        RepopulateSelectLists(exerciseViewModel);
        
        return View(exerciseViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExerciseFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var exercise = new Exercise
        {
            Name = model.Name,
            Description = model.Description,
            Instructions = model.Instructions,
            ImageUrl = model.ImageUrl,
            MuscleGroup = model.MuscleGroup,
            Difficulty = model.Difficulty,
            CreatedBy = await _userManager.GetUserAsync(User) ?? throw new UnauthorizedAccessException(),
            IsApproved = false
        };
        
        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Exercise submitted! It will appear publicly once approved.";
        return RedirectToAction(nameof(Details), new { id = exercise.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)
            return NotFound();


        var userId = _userManager.GetUserId(User);
        if (exercise.CreatedById != userId)
            return Forbid();

      
        var viewModel = new ExerciseFormViewModel
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            Instructions = exercise.Instructions,
            ImageUrl = exercise.ImageUrl,
            MuscleGroup = exercise.MuscleGroup,
            Difficulty = exercise.Difficulty
        };

        RepopulateSelectLists(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExerciseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            RepopulateSelectLists(model);
            return View(model);
        }

        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)
            return NotFound();

        var userId = _userManager.GetUserId(User);
        if (exercise.CreatedById != userId)
            return Forbid();

        exercise.Name = model.Name;
        exercise.Description = model.Description;
        exercise.Instructions = model.Instructions;
        exercise.ImageUrl = model.ImageUrl;
        exercise.MuscleGroup = model.MuscleGroup;
        exercise.Difficulty = model.Difficulty;

        exercise.IsApproved = false;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Your changes have been submitted and are pending admin approval.";
        return RedirectToAction(nameof(Details), new { id = exercise.Id });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Pending()
    {
        var pending = await _context.Exercises
            .Include(e => e.CreatedBy)
            .Where(e => !e.IsApproved)
            .OrderBy(e => e.Name)
            .Select(e => new ExercisePendingViewModel
            {
                Id = e.Id,
                Name = e.Name,
                MuscleGroup = e.MuscleGroup,
                Difficulty = e.Difficulty,
                SubmittedBy = e.CreatedBy.FirstName + " " + e.CreatedBy.LastName
            })
            .ToListAsync();

        return View(pending);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)
            return NotFound();

        exercise.IsApproved = true;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"{exercise.Name} has been approved and is now public.";
        return RedirectToAction(nameof(Pending));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)
            return NotFound();

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"{exercise.Name} has been rejected and removed.";
        return RedirectToAction(nameof(Pending));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)            
            return NotFound();

        var userId = _userManager.GetUserId(User);
        bool isOwner = exercise.CreatedById == userId;
        bool isAdmin = User.IsInRole("Admin");

        if (!isOwner && !isAdmin)
            return Forbid();

        return View(exercise);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)
            return NotFound();

        var userId = _userManager.GetUserId(User);
        bool isOwner = exercise.CreatedById == userId;
        bool isAdmin = User.IsInRole("Admin");
        if (!isOwner && !isAdmin)
            return Forbid();

        bool isReferenced = await _context.WorkoutExercises.AnyAsync(we => we.ExerciseId == id);
        if (isReferenced)
        {
            TempData["Error"] = "Cannot delete this exercise because it is used in one or more workouts. Please remove it from those workouts first.";
            return RedirectToAction(nameof(Details), new { id });
        }
        
        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = $"{exercise.Name} has been deleted.";
        return RedirectToAction(nameof(Index));
    }

    private void RepopulateSelectLists(ExerciseFormViewModel model)
    {
        model.MuscleGroupOptions = new SelectList(
                Enum.GetValues(typeof(MuscleGroup)),
                model.MuscleGroup
            );
        model.DifficultyOptions = new SelectList(
                Enum.GetValues(typeof(Difficulty)),
                model.Difficulty
            );
    }
}