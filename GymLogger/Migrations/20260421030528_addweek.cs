using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymLogger.Migrations
{
    /// <inheritdoc />
    public partial class addweek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // WeightKg, Reps, DistanceKm, DurationMinutes were already applied
            // to the database by a prior migration — only add what is missing.
            migrationBuilder.AddColumn<int>(
                name: "WeeklyWorkoutGoal",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeeklyWorkoutGoal",
                table: "AspNetUsers");
        }
    }
}
