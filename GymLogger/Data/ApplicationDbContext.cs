using GymLogger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymLogger.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<PersonalRecord> PersonalRecords { get; set; }
    public DbSet<ProgramDay> ProgramsDay { get; set; }
    public DbSet<ProgramDayExercise> ProgramsDayExercises { get; set; }
    public DbSet<TrainingProgram> TrainingPrograms { get; set; }
    public DbSet<UserProgram> UserPrograms { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Restrict cascade delete on Exercise.CreatedById and TrainingProgram.CreatedById
        // to prevent multiple cascade paths to child tables (PersonalRecord, WorkoutExercise,
        // ProgramDayExercise) that also FK to the user directly or via Workout/Program.
        builder.Entity<Exercise>()
            .HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingProgram>()
            .HasOne(tp => tp.CreatedBy)
            .WithMany(u => u.TrainingPrograms)
            .HasForeignKey(tp => tp.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
