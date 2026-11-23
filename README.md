# PUSH

> **Work in progress.** PUSH is a capstone project at Techtorium NZIIT and is still being built and tested. Features, data and setup steps may change.

PUSH is a fitness app for Android. Users build their own workouts from a library of exercises, run them with a built-in timer, keep notes, track their stats and weight over time, share workouts with the community, and find personal trainers at local gyms.

## Features

- **Accounts** – register, log in, guest access, update details, profile pictures, password reset by email, delete account
- **Workouts** – create, edit and delete workouts from the exercise library, set sets/reps or minutes per exercise, run a workout
- **Community** – share workouts publicly, favourite workouts, report inappropriate workouts
- **Notes** – add notes to your own workouts and to public workouts
- **Stats** – workout history and weight tracking with charts
- **Trainers** – browse trainers, with trainers at your favourite gym shown first
- **Articles** – fitness and nutrition articles on the home page
- **Admin** – manage users, gyms, categories, exercises and trainers, and review reported workouts

### Roles

| Role | Access |
|---|---|
| Guest | View seeded workouts, notes and stats; run workouts (not saved). No Trainers or Community. |
| Member | Everything for their own account, plus Community and Trainers |
| Admin | Everything a member can do, plus the admin pages and management |

## How it works

```
PUSH app (.NET MAUI, Android)
        │  HTTPS + JWT login token
        ▼
PUSH API (ASP.NET Core Web API)  ──►  Microsoft SQL Server database
        (both hosted on MonsterASP)
```

- The **app** never connects to the database directly. Every service in the app calls the API.
- The **API** checks every request: who is logged in (JWT), what their role allows, and all validation rules.

## Tech stack

**App:** .NET 10 MAUI (Android), MVVM with CommunityToolkit.Mvvm

**API:** ASP.NET Core Web API (.NET 10), Entity Framework Core (SQL Server), JWT bearer authentication, PBKDF2 password hashing, MailKit (Gmail SMTP) for emails

**Hosting:** MonsterASP (API + Microsoft SQL Server)

## Repositories

- **CapstoneProjectPUSH** – the MAUI app @ https://github.com/JamesB62/CapstoneProjectPUSH
- **CapstoneProjectPUSH_API** – the Web API @ https://github.com/JamesB62/CapstoneProjectPUSH_API

## Running the API locally

1. Open the API project in Visual Studio.
2. Right-click the project → **Manage User Secrets** and add:

   ```json
   {
     "ConnectionStrings": {
       "PushDb": "Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True;"
     },
     "Jwt": {
       "Key": "a long random key, at least 32 characters"
     },
     "SeedAdmin": {
       "Password": "the admin password to seed"
     },
     "Email": {
       "SenderName": "PUSH",
       "SenderEmail": "...",
       "AppPassword": "..."
     }
   }
   ```

   Ask the team for the real values. **Never commit these.** If `Email` is left out, emails are written to the output window instead of being sent.

3. Apply the database migrations in the **Package Manager Console**:

   ```
   Update-Database
   ```

4. Run the API. Empty tables are filled with seed data on startup.

## Running the app

1. Open the app project in Visual Studio.
2. Check `BaseUrl` in `Services/Api/ApiClient.cs` points at the API.
3. Run it on an Android emulator or device.

## Known limitations

- Android only (no iOS, Mac or Windows builds)
- No "remember me" yet – users log in again each time the app opens (login tokens last 7 days)
- Still being tested – expect bugs

## Student Contributions

- **[James]** – Web API, remote database and app migration, role-based access, reports
- **[Dylan]** – Admin features, page layout and app plumbing, Stats page
- **[Vison]** – Email features, guest account, workout feature
