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
                    CategoryId = 1
                },

                new Exercise
                {
                    Title = "Exercise 2",
                    Description = "This is a placeholder description for Exercise 2.",
                    CategoryId = 3
                },

                new Exercise
                {
                    Title = "Exercise 3",
                    Description = "This is a placeholder description for Exercise 3.",
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
    }
}