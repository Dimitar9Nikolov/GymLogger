using GymLogger.Models;
using GymLogger.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace GymLogger.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Seed Admin role
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        // Seed admin user
        var adminEmail = "admin@gymlogger.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                CreatedOn = DateTime.UtcNow,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Seed exercises
        if (!context.Exercises.Any())
        {
            var exercises = new List<Exercise>
            {
                new()
                {
                    Name = "Barbell Bench Press",
                    Description = "A compound push exercise targeting the chest, shoulders, and triceps.",
                    Instructions = "1. Lie flat on the bench.\n2. Grip the bar slightly wider than shoulder-width.\n3. Lower the bar to your mid-chest.\n4. Press back up to full extension.",
                    MuscleGroup = MuscleGroup.Chest,
                    Difficulty = Difficulty.Intermediate,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Pull-Up",
                    Description = "A bodyweight exercise targeting the back and biceps.",
                    Instructions = "1. Hang from the bar with an overhand grip.\n2. Pull yourself up until your chin clears the bar.\n3. Lower back down with control.",
                    MuscleGroup = MuscleGroup.Back,
                    Difficulty = Difficulty.Intermediate,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Barbell Squat",
                    Description = "The king of leg exercises. Targets quads, hamstrings, and glutes.",
                    Instructions = "1. Bar on upper traps, feet shoulder-width apart.\n2. Brace your core and descend until thighs are parallel.\n3. Drive through your heels back to standing.",
                    MuscleGroup = MuscleGroup.Legs,
                    Difficulty = Difficulty.Advanced,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Deadlift",
                    Description = "A full-body compound lift primarily targeting the posterior chain.",
                    Instructions = "1. Stand with feet hip-width, bar over mid-foot.\n2. Hinge at the hips, grip the bar just outside your legs.\n3. Drive through the floor and lock out at the top.\n4. Lower the bar with control.",
                    MuscleGroup = MuscleGroup.Back,
                    Difficulty = Difficulty.Advanced,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Overhead Press",
                    Description = "A compound shoulder press performed standing.",
                    Instructions = "1. Hold the bar at shoulder height, elbows slightly forward.\n2. Press the bar overhead to full extension.\n3. Lower back to shoulder height.",
                    MuscleGroup = MuscleGroup.Shoulders,
                    Difficulty = Difficulty.Intermediate,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Dumbbell Curl",
                    Description = "An isolation exercise for the biceps.",
                    Instructions = "1. Stand with a dumbbell in each hand, arms at your sides.\n2. Curl the weights up to shoulder height.\n3. Lower with control.",
                    MuscleGroup = MuscleGroup.Arms,
                    Difficulty = Difficulty.Beginner,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Plank",
                    Description = "An isometric core exercise that builds stability and endurance.",
                    Instructions = "1. Hold a push-up position with forearms on the floor.\n2. Keep your body in a straight line from head to heels.\n3. Hold for the target duration.",
                    MuscleGroup = MuscleGroup.Core,
                    Difficulty = Difficulty.Beginner,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Romanian Deadlift",
                    Description = "A hip-hinge movement targeting the hamstrings and glutes.",
                    Instructions = "1. Hold the bar at hip height, slight bend in knees.\n2. Hinge at the hips, lowering the bar along your legs.\n3. Drive your hips forward to return to standing.",
                    MuscleGroup = MuscleGroup.Legs,
                    Difficulty = Difficulty.Intermediate,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Tricep Pushdown",
                    Description = "A cable isolation exercise for the triceps.",
                    Instructions = "1. Attach a rope or bar to a high cable pulley.\n2. Keep elbows tucked at your sides.\n3. Push the weight down to full extension.\n4. Return slowly.",
                    MuscleGroup = MuscleGroup.Arms,
                    Difficulty = Difficulty.Beginner,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Lat Pulldown",
                    Description = "A cable exercise mimicking the pull-up, targeting the lats.",
                    Instructions = "1. Sit at the machine, grip the bar wide.\n2. Pull the bar down to your upper chest.\n3. Return with control.",
                    MuscleGroup = MuscleGroup.Back,
                    Difficulty = Difficulty.Beginner,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Running",
                    Description = "Cardiovascular exercise improving endurance and burning calories.",
                    Instructions = "1. Maintain an upright posture.\n2. Land mid-foot, not on your heel.\n3. Keep a steady breathing rhythm.",
                    MuscleGroup = MuscleGroup.Cardio,
                    Difficulty = Difficulty.Beginner,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "Dumbbell Lunges",
                    Description = "A unilateral leg exercise targeting quads, hamstrings, and glutes.",
                    Instructions = "1. Hold a dumbbell in each hand.\n2. Step forward and lower your back knee toward the floor.\n3. Push off the front foot to return.",
                    MuscleGroup = MuscleGroup.Legs,
                    Difficulty = Difficulty.Beginner,
                    IsApproved = true,
                    CreatedById = adminUser.Id
                },
            };

            context.Exercises.AddRange(exercises);
            await context.SaveChangesAsync();
        }
    }
}
