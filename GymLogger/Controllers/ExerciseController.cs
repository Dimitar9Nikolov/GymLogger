using GymLogger.Data;
using GymLogger.Models;
using GymLogger.Models.Enums;
using GymLogger.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        int pageSize = 10;
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
}