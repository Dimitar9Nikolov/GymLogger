using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymLogger.Migrations
{
    /// <inheritdoc />
    public partial class SeedAllExerciseImageUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2018/11/bankdruecken-flachbank-langhantel-800x448.png'
                WHERE Name = 'Barbell Bench Press' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2020/02/klimmzuege-800x448.png'
                WHERE Name = 'Pull-Up' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2020/03/kniebeugen-langhantel-800x448.png'
                WHERE Name = 'Barbell Squat' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2020/02/deadlift-kreuzheben-800x448.png'
                WHERE Name = 'Deadlift' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2020/03/schulterdruecken-langhantel-800x448.png'
                WHERE Name = 'Overhead Press' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2018/12/bizepscurls-800x448.png'
                WHERE Name = 'Dumbbell Curl' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://gymvisual.com/img/p/9/0/3/8/9038.gif'
                WHERE Name = 'Plank' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2020/03/kreuzheben-gestreckte-beine.png'
                WHERE Name = 'Romanian Deadlift' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2020/03/trizepsdruecken-kabelzug.png'
                WHERE Name = 'Tricep Pushdown' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2020/02/latzug-800x448.png'
                WHERE Name = 'Lat Pulldown' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://gymvisual.com/img/p/9/3/9/4/9394.gif'
                WHERE Name = 'Running' AND ImageUrl IS NULL;

                UPDATE Exercises SET ImageUrl = 'https://training.fit/wp-content/uploads/2020/03/ausfallschritt-kurzhanteln-1-800x448.png'
                WHERE Name = 'Dumbbell Lunges' AND ImageUrl IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Exercises SET ImageUrl = NULL
                WHERE Name IN (
                    'Pull-Up', 'Barbell Squat', 'Deadlift', 'Overhead Press',
                    'Dumbbell Curl', 'Plank', 'Romanian Deadlift', 'Tricep Pushdown',
                    'Lat Pulldown', 'Running', 'Dumbbell Lunges'
                );
            ");
        }
    }
}
