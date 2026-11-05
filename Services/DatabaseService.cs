namespace CapstoneProjectPUSH.Services
{
    public class DatabaseService
    {
        public const string DbFileName = "push.db3";

        // Seeded admin login (testing only, change or remove before release)
        public const string SeedAdminEmail = "admin@push.local";
        public const string SeedAdminPassword = "Admin123!";
        public static string DbPath => Path.Combine(FileSystem.AppDataDirectory, DbFileName);

        private SQLiteAsyncConnection? _connection;

        private readonly SemaphoreSlim _initLock = new(1, 1);

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_connection is not null)
            {
                return _connection;
            }

            await _initLock.WaitAsync();

            try
            {
                if (_connection is null)
                {
                    var connection = new SQLiteAsyncConnection(DbPath);

                    // Add SQLite tables here
                    await connection.CreateTableAsync<Article>();
                    await connection.CreateTableAsync<Category>();
                    await connection.CreateTableAsync<Exercise>();
                    await connection.CreateTableAsync<Gym>();
                    await connection.CreateTableAsync<Trainer>();
                    await connection.CreateTableAsync<User>();
                    await connection.CreateTableAsync<Workout>();
                    await connection.CreateTableAsync<WorkoutExercise>();
                    await connection.CreateTableAsync<WorkoutLog>();

                    _connection = connection;

                    // For testing purposes -- order specific
                    await SeedCategoriesAsync();
                    await SeedGymsAsync();
                    await SeedTrainersAsync();
                    await SeedExercisesAsync();
                    await SeedWorkoutsAsync();
                    await SeedWorkoutLogsAsync();
                    await SeedWorkoutExercisesAsync();
                    await SeedArticlesAsync();
                    await SeedAdminAsync();
                }
            }
            finally
            {
                _initLock.Release();
            }

            return _connection;
        }

        private async Task SeedAdminAsync()
        {
            var db = await GetConnectionAsync();

            var adminExists = await db.Table<User>().Where(u => u.IsAdmin == true).CountAsync() > 0;
            if (adminExists)
            {
                return;
            }

            // If a regular user already took this email, leave it alone rather than promoting them
            var emailTaken = await db.Table<User>().Where(u => u.Email == SeedAdminEmail).CountAsync() > 0;
            if (emailTaken)
            {
                return;
            }

            await db.InsertAsync(new User
            {
                Email = SeedAdminEmail,
                DisplayName = "Administrator",
                PasswordHash = UserService.HashPassword(SeedAdminPassword),
                IsAdmin = true,
                DateTimeCreated = DateTime.Now
            });
        }

        private async Task SeedCategoriesAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<Category>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var categories = new[]
            {
                "Strength Training", "Cardio", "HIIT", "Yoga",
                "Flexibility & Mobility", "Weight Loss", "Muscle Building",
                "Nutrition", "Recovery"
            };

            await db.InsertAllAsync(categories.Select(c => new Category { Name = c }));
        }

        private async Task SeedGymsAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<Gym>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var gyms = new List<Gym>
            {
                new Gym {
                    Name = "ALLFIT",
                    StreetAddress = "32 Manukau Road, Epsom",
                    City = "Auckland"
                },

                new Gym {
                    Name = "Les Mills",
                    StreetAddress = "269 Khyber Pass Road, Newmarket",
                    City = "Auckland"
                },

                new Gym {
                    Name = "Les Mills",
                    StreetAddress = "747 Victoria Street, Hamilton Central",
                    City = "Hamilton"
                },

                new Gym {
                    Name = "Lift Club",
                    StreetAddress = "24 Minnie Street, Eden Terrace",
                    City = "Auckland"
                }
            };

            await db.InsertAllAsync(gyms);
        }

        private async Task SeedTrainersAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<Trainer>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var trainers = new List<Trainer>
            {
                new Trainer {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "jdoe123@example.com",
                    PhoneNumber = "123-456-7890",
                    CategoryId = 3,
                    Description = "Certified personal trainer with 5 years of experience",
                    Available = true,
                    GymId = 1
                },

                new Trainer {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jdoe456@example.com",
                    PhoneNumber = "098-765-4321",
                    CategoryId = 1,
                    Description = "Certified personal trainer with 8 years of experience",
                    Available = false,
                    GymId = 3
                }
            };

            await db.InsertAllAsync(trainers);
        }

        //Update with better data
        private async Task SeedExercisesAsync()
        {
            var db = await GetConnectionAsync();
            if (await db.Table<Exercise>().CountAsync() > 0) return;

            // CategoryId: 1 Strength, 2 Cardio, 3 HIIT, 4 Yoga, 5 Flexibility, 7 Muscle Building, 9 Recovery

            var exercises = new List<Exercise>
            {
                new Exercise
                {
                    Title = "Barbell Bench Press",
                    Description = "Lie on a bench, lower the bar to mid-chest, then press it back up. Works chest, shoulders and triceps.",
                    Difficulty = "Easy",
                    CategoryId = 1
                },

                new Exercise
                {
                    Title = "Burpees",
                    Description = "Drop to a push-up, jump your feet in, then jump up with your arms overhead.",
                    Difficulty = "Medium",
                    CategoryId = 3
                },

                new Exercise
                {
                    Title = "Hamstring Stretch",
                    Description = "This is a placeholder description for Exercise 3.",
                    Difficulty = "Hard",
                    CategoryId = 5
                }
            };

            await db.InsertAllAsync(exercises);
        }

        //seed data for workout plan
        private async Task SeedWorkoutsAsync()
        {
            var db = await GetConnectionAsync();
            if (await db.Table<Workout>().CountAsync() > 0) return;

            await db.InsertAllAsync(new[]
            {
                new Workout { Name = "Push Day",     UserId = 1, CategoryId = 1 },
                new Workout { Name = "Pull Day",     UserId = 1, CategoryId = 1 },
                new Workout { Name = "Leg Day",      UserId = 1, CategoryId = 1 },
                new Workout { Name = "Morning HIIT", UserId = 1, CategoryId = 3 },
                new Workout { Name = "Cardio Burn",  UserId = 1, CategoryId = 2 },
                new Workout { Name = "Yoga Flow",    UserId = 1, CategoryId = 4 },
                new Workout { Name = "Cool Down",    UserId = 1, CategoryId = 9 },
            });
        }

        private async Task SeedWorkoutExercisesAsync()
        {
            var db = await GetConnectionAsync();
            if (await db.Table<WorkoutExercise>().CountAsync() > 0) return;

            var exerciseIds = (await db.Table<Exercise>().ToListAsync())
                .GroupBy(e => e.Title).ToDictionary(e => e.Key, e => e.First().Id);
            var workoutIds = (await db.Table<Workout>().ToListAsync())
            .GroupBy(w => w.Name).ToDictionary(w => w.Key, w => w.First().Id);
            
            var items = new List<WorkoutExercise>();

            void Add(string workout, string exercise, int order, int sets, int reps)
            {
                if (workoutIds.TryGetValue(workout, out var wId) && exerciseIds.TryGetValue(exercise, out var eId))
                    items.Add(new WorkoutExercise { WorkoutId = wId, ExerciseId = eId, SortOrder = order, Sets = sets, Reps = reps });
                else
                    System.Diagnostics.Debug.WriteLine($"Seed skipped: {workout} / {exercise}");
            }

            Add("Push Day", "Barbell Bench Press", 1, 4, 8);
            Add("Push Day", "Overhead Press",      2, 3, 10);
            Add("Push Day", "Push-Up",             3, 3, 15);

            Add("Pull Day", "Deadlift",     1, 3, 5);
            Add("Pull Day", "Pull-Up",      2, 4, 8);
            Add("Pull Day", "Dumbbell Row", 3, 3, 10);

            Add("Leg Day", "Back Squat",  1, 4, 8);
            Add("Leg Day", "Deadlift",    2, 3, 5);
            Add("Leg Day", "Jump Squats", 3, 3, 12);

            Add("Morning HIIT", "Burpees",           1, 4, 15);
            Add("Morning HIIT", "Mountain Climbers", 2, 4, 30);
            Add("Morning HIIT", "Jump Squats",       3, 3, 20);

            Add("Cardio Burn", "Jump Rope",         1, 3, 100);
            Add("Cardio Burn", "Mountain Climbers", 2, 3, 30);
            Add("Cardio Burn", "Burpees",           3, 3, 10);

            Add("Yoga Flow", "Sun Salutation", 1, 3, 5);
            Add("Yoga Flow", "Downward Dog",   2, 3, 8);

            Add("Cool Down", "Hamstring Stretch", 1, 2, 3);
            Add("Cool Down", "Downward Dog",      2, 2, 5);
            Add("Cool Down", "Foam Rolling",      3, 1, 10);

            await db.InsertAllAsync(items);



            // await db.InsertAllAsync(new[]
            // {
            //     new WorkoutExercise { WorkoutId = 1, ExerciseId = 1, SortOrder = 1, Sets = 3, Reps = 10 },
            //     new WorkoutExercise { WorkoutId = 1, ExerciseId = 2, SortOrder = 2, Sets = 3, Reps = 12 },
            //     new WorkoutExercise { WorkoutId = 2, ExerciseId = 2, SortOrder = 1, Sets = 4, Reps = 15 },
            //     new WorkoutExercise { WorkoutId = 2, ExerciseId = 3, SortOrder = 2, Sets = 3, Reps = 20 },
            //     new WorkoutExercise { WorkoutId = 3, ExerciseId = 3, SortOrder = 1, Sets = 2, Reps = 30 },
            // });
        }

        private async Task SeedArticlesAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<Article>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var articles = new List<Article>
            {
                new Article
                {
                    Title = "Article 1",
                    Content = "This is placeholder content for Article 1.",
                    Citation = "This is a placeholder citation for Article 1.",
                    DateTimeAdded = DateTime.Now.AddDays(-2),
                    CategoryId = 2
                },

                new Article
                {
                    Title = "Article 2",
                    Content = "This is placeholder content for Article 2.",
                    Citation = "This is a placeholder citation for Article 2.",
                    DateTimeAdded = DateTime.Now.AddDays(-1),
                    CategoryId = 1
                },

                new Article
                {
                    Title = "Article 3",
                    Content = "This is placeholder content for Article 3.",
                    Citation = "This is a placeholder citation for Article 3.",
                    DateTimeAdded = DateTime.Now,
                    CategoryId = 5
                }
            };

            await db.InsertAllAsync(articles);
        }

        private async Task SeedWorkoutLogsAsync()
        {
            var db = await GetConnectionAsync();
            if (await db.Table<WorkoutLog>().CountAsync() > 0) return;

            WorkoutLog Log(int workoutId, int daysAgo, int minutes) => new()
            {
                WorkoutId = workoutId,
                UserId = 1,
                DateTimeCompleted = DateTime.Today.AddDays(-daysAgo).AddHours(7),
                DurationMinutes = minutes,
                DurationSeconds = minutes * 60
            };

            await db.InsertAllAsync(new[]
            {
                // Week
                Log(1, 0, 45), Log(2, 1, 30), Log(3, 2, 60), Log(1, 3, 40), Log(2, 5, 35),

                // Month
                Log(1, 8, 45), Log(3, 10, 30), Log(2, 13, 60), Log(1, 15, 40),
                Log(2, 20, 50), Log(3, 24, 35), Log(1, 27, 45),

                // Year
                Log(1, 40, 50), Log(2, 70, 45), Log(3, 100, 60), Log(1, 130, 30),
                Log(2, 160, 55), Log(3, 200, 40), Log(1, 250, 70), Log(2, 300, 45),
            });
        }
    }
}