# GymLogger

A workout tracking web application built with ASP.NET Core MVC. Log workouts, track personal records, and browse an exercise library — all in one place.

---

## Features

- **Workout Logging** — Create and edit workouts with multiple exercises and sets
- **Strength & Cardio Support** — Strength exercises track reps and weight; cardio exercises track duration and distance
- **Exercise Library** — Browse approved exercises with images, muscle group filters, and difficulty ratings
- **Personal Records** — Automatically derived from your best logged sets per exercise
- **Dashboard** — Overview of total workouts, sets logged, and recent personal bests
- **User Submissions** — Registered users can submit new exercises for admin approval
- **Admin Panel** — Approve or reject user-submitted exercises
- **Role-based Access** — Admin and regular user roles via ASP.NET Core Identity

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core MVC (.NET 10) |
| ORM | Entity Framework Core 10 |
| Database | SQL Server (LocalDB for development) |
| Auth | ASP.NET Core Identity |
| UI | Bootstrap 5 |

---

## Local Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server LocalDB (included with Visual Studio)

### 1. Clone the repository

```bash
git clone https://github.com/your-username/GymLogger.git
cd GymLogger
```

### 2. Set the connection string

The connection string is **not stored in the repository**. Create a local `appsettings.Development.json` file inside the `GymLogger/` project folder:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GymLogger;Integrated Security=True;TrustServerCertificate=True;Encrypt=False"
  }
}
```

> Adjust the `Server` value to match your local SQL Server instance.

### 3. Apply migrations & seed data

```bash
cd GymLogger
dotnet ef database update
```

This creates the database and seeds:
- An admin user (`admin@gymlogger.com` / `Admin123!`)
- 12 approved exercises with images

### 4. Run the app

```bash
dotnet run
```

Open `https://localhost:5001` in your browser.

---

## Project Structure

```
GymLogger/
├── Controllers/          # MVC controllers
├── Data/                 # DbContext and database seeder
├── Migrations/           # EF Core migrations
├── Models/               # Entity models and enums
├── ViewModels/           # View-specific data models
└── Views/                # Razor views
```

---

## Admin Account

After running migrations the following admin account is seeded automatically:

| Field | Value |
|-------|-------|
| Email | `admin@gymlogger.com` |
| Password | `Admin123!` |

---

## Security

- Connection strings are excluded from source control via `.gitignore`
- All write actions are protected with `[ValidateAntiForgeryToken]`
- All pages require authentication via `[Authorize]`
- User data is strictly scoped — users can only access their own workouts and records
