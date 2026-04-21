using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymLogger.Migrations
{
    /// <inheritdoc />
    public partial class SeedBenchPressImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2018/11/bankdruecken-flachbank-langhantel-800x448.png' WHERE Name = 'Barbell Bench Press' AND ImageUrl IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE Exercises SET ImageUrl = NULL WHERE Name = 'Barbell Bench Press'");
        }
    }
}
