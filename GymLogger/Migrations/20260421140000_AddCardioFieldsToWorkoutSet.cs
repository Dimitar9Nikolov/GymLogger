using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymLogger.Migrations
{
    /// <inheritdoc />
    public partial class AddCardioFieldsToWorkoutSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make Reps nullable
            migrationBuilder.AlterColumn<int>(
                name: "Reps",
                table: "WorkoutSets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Make WeightKg nullable
            migrationBuilder.AlterColumn<decimal>(
                name: "WeightKg",
                table: "WorkoutSets",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldPrecision: 6,
                oldScale: 2);

            // Add DurationMinutes for cardio
            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "WorkoutSets",
                type: "int",
                nullable: true);

            // Add DistanceKm for cardio
            migrationBuilder.AddColumn<decimal>(
                name: "DistanceKm",
                table: "WorkoutSets",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DurationMinutes", table: "WorkoutSets");
            migrationBuilder.DropColumn(name: "DistanceKm", table: "WorkoutSets");

            migrationBuilder.AlterColumn<int>(
                name: "Reps",
                table: "WorkoutSets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "WeightKg",
                table: "WorkoutSets",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldPrecision: 6,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
