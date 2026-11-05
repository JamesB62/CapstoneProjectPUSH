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
                    await connection.CreateTableAsync<Workout>();
                    await connection.CreateTableAsync<WorkoutExercise>();
                    await connection.CreateTableAsync<WorkoutLog>();
                    await connection.CreateTableAsync<User>();
                    await connection.CreateTableAsync<WeightLog>();

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
                    await SeedWeightLogsAsync();
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

            var count = await db.Table<Exercise>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var exercises = new List<Exercise>
            {
                new Exercise
                {
                    Title = "Exercise 1",
                    Description = "This is a placeholder description for Exercise 1.",
                    Difficulty = "Easy",
                    CategoryId = 1
                },

                new Exercise
                {
                    Title = "Exercise 2",
                    Description = "This is a placeholder description for Exercise 2.",
                    Difficulty = "Medium",
                    CategoryId = 3
                },

                new Exercise
                {
                    Title = "Exercise 3",
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
                new Workout { Name = "Push Day", UserId = 1, CategoryId = 1 },
                new Workout { Name = "Morning HIIT", UserId = 1, CategoryId = 3 },
                new Workout { Name = "Yoga Flow", UserId = 1, CategoryId = 4 },

            });
        }

        private async Task SeedWorkoutExercisesAsync()
        {
            var db = await GetConnectionAsync();
            if (await db.Table<WorkoutExercise>().CountAsync() > -0) return;

            await db.InsertAllAsync(new[]
            {
                new WorkoutExercise { WorkoutId = 1, ExerciseId = 1, SortOrder = 1, Sets = 3, Reps = 10 },
                new WorkoutExercise { WorkoutId = 1, ExerciseId = 2, SortOrder = 2, Sets = 3, Reps = 12 },
                new WorkoutExercise { WorkoutId = 2, ExerciseId = 2, SortOrder = 1, Sets = 4, Reps = 15 },
                new WorkoutExercise { WorkoutId = 2, ExerciseId = 3, SortOrder = 2, Sets = 3, Reps = 20 },
                new WorkoutExercise { WorkoutId = 3, ExerciseId = 3, SortOrder = 1, Sets = 2, Reps = 30 },

            });
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

        private async Task SeedWeightLogsAsync()
        {
            var db = await GetConnectionAsync();
            if (await db.Table<WeightLog>().CountAsync() > 0) return;
            WeightLog Log(int daysAgo, double weight) => new()
            {
                UserId = 1,
                DateTimeLogged = DateTime.Today.AddDays(-daysAgo).AddHours(7),
                WeightKg = (int)weight
            };
            await db.InsertAllAsync(new[]
            {
                // Week
                Log(0, 80.5), Log(1, 80.2), Log(2, 80.0), Log(3, 79.8), Log(5, 79.5),
                // Month
                Log(8, 79.3), Log(10, 79.0), Log(13, 78.8), Log(15, 78.5),
                Log(20, 78.3), Log(24, 78.0), Log(27, 77.8),
                // Year
                Log(40, 77.5), Log(70, 77.3), Log(100, 77.0), Log(130, 76.8),
                Log(160, 76.5), Log(200, 76.3), Log(250, 76.0), Log(300, 75.8),
            });
        }
    }
}