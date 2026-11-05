namespace CapstoneProjectPUSH.Services
{
    public class DatabaseService
    {
        public const string DbFileName = "push.db3";

        // Seeded admin login (testing only, change or remove before release)
        public const string SeedAdminEmail = "admin@push.local";
        public const string SeedAdminPassword = "Admin123!";
        // ==========

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

                    // SQLite tables
                    await connection.CreateTableAsync<Article>();
                    await connection.CreateTableAsync<Category>();
                    await connection.CreateTableAsync<Exercise>();
                    await connection.CreateTableAsync<Gym>();
                    await connection.CreateTableAsync<Note>();
                    await connection.CreateTableAsync<Trainer>();
                    await connection.CreateTableAsync<User>();
                    await connection.CreateTableAsync<Workout>();
                    await connection.CreateTableAsync<WorkoutExercise>();
                    await connection.CreateTableAsync<WorkoutLog>();

                    _connection = connection;

                    // Seeded data -- order specific
                    await SeedCategoriesAsync();
                    await SeedGymsAsync();
                    await SeedTrainersAsync();
                    await SeedExercisesAsync();
                    await SeedWorkoutsAsync();
                    await SeedWorkoutLogsAsync();
                    await SeedWorkoutExercisesAsync();
                    await SeedArticlesAsync();
                    await SeedNotesAsync();
                    //await SeedAdminAsync();
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
                "Strength Training",        // 1
                "Cardio",                   // 2
                "HIIT",                     // 3
                "Yoga",                     // 4
                "Flexibility & Mobility",   // 5
                "Weight Loss",              // 6
                "Muscle Building",          // 7
                "Nutrition",                // 8
                "Recovery"                  // 9
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
                // Auckland
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
                    Name = "Lift Club",
                    StreetAddress = "24 Minnie Street, Eden Terrace",
                    City = "Auckland"
                },

                // Hamilton
                new Gym {
                    Name = "Les Mills",
                    StreetAddress = "747 Victoria Street, Hamilton Central",
                    City = "Hamilton"
                },

                // Wellington
                new Gym
                {
                    Name = "Flex Fitness",
                    StreetAddress = "222a Willis Street, Te Aro",
                    City = "Wellington"
                },

                new Gym
                {
                    Name = "Jetts",
                    StreetAddress = "James Smith Corner 49 First Floor/61 Cuba Street, Te Aro",
                    City = "Wellington"
                },

                new Gym
                {
                    Name = "Les Mills",
                    StreetAddress = "52/70 Taranaki Street, Te Aro",
                    City = "Wellington"
                },

                // Christchurch
                new Gym
                {
                    Name = "Flex Fitness",
                    StreetAddress = "4 Bellewood Avenue, Belfast",
                    City = "Christchurch"
                },

                new Gym
                {
                    Name = "Inception Gym",
                    StreetAddress = "65 Blenheim Road, Addington",
                    City = "Christchurch"
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
                    CategoryId = 1,
                    Description = "Certified personal trainer with 8 years of experience in strength training. John focuses on proper technique and progressive overload, helping clients build lasting strength safely, whether they're picking up a barbell for the first time or working toward a new personal best.",
                    Available = true,
                    GymId = 1
                },

                new Trainer {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jdoe456@example.com",
                    PhoneNumber = "098-765-4321",
                    CategoryId = 3,
                    Description = "Certified personal trainer with 5 years of experience specialising in high-intensity interval training. Jane designs fast-paced, varied sessions that build endurance and burn calories efficiently, and scales every workout to suit beginners through to experienced athletes.",
                    Available = false,
                    GymId = 3
                }
            };

            await db.InsertAllAsync(trainers);
        }

        private async Task SeedExercisesAsync()
        {
            var db = await GetConnectionAsync();
            if (await db.Table<Exercise>().CountAsync() > 0) return;

            // CategoryId: 1 Strength, 2 Cardio, 3 HIIT, 4 Yoga, 5 Flexibility, 7 Muscle Building, 9 Recovery

            var exercises = new List<Exercise>
            {
                // 1 - Strength Training
                new Exercise
                {
                    Title = "Bodyweight Squat",
                    Description = "Stand with feet shoulder-width apart. Push your hips back and bend your knees to lower down, keeping your chest up and heels flat. Drive through your feet to return to standing.",
                    Difficulty = "Easy",
                    CategoryId = 1
                },
                new Exercise
                {
                    Title = "Push-Up",
                    Description = "Start in a high plank with hands slightly wider than your shoulders. Lower your chest toward the floor while keeping your body in a straight line, then press back up.",
                    Difficulty = "Medium",
                    CategoryId = 1
                },

                // 2 - Cardio
                new Exercise
                {
                    Title = "Brisk Walk",
                    Description = "Walk at a pace that raises your heart rate but still lets you hold a conversation. Keep your posture tall, shoulders relaxed, and swing your arms naturally.",
                    Difficulty = "Easy",
                    CategoryId = 2
                },
                new Exercise
                {
                    Title = "Jump Rope",
                    Description = "Hold the handles at hip height and turn the rope with your wrists. Jump just high enough to clear the rope, landing softly on the balls of your feet.",
                    Difficulty = "Medium",
                    CategoryId = 2
                },

                // 3 - HIIT
                new Exercise
                {
                    Title = "Burpee",
                    Description = "From standing, squat down and place your hands on the floor. Jump your feet back into a plank, lower your chest to the floor, push up, jump your feet forward, and explode upward into a jump.",
                    Difficulty = "Hard",
                    CategoryId = 3
                },

                // 4 - Yoga
                new Exercise
                {
                    Title = "Downward-Facing Dog",
                    Description = "Start on hands and knees, tuck your toes, and lift your hips up and back. Press your hands into the mat, keep your spine long, and let your head hang between your arms.",
                    Difficulty = "Medium",
                    CategoryId = 4
                },

                // 5 - Flexibility & Mobility
                new Exercise
                {
                    Title = "Seated Hamstring Stretch",
                    Description = "Sit with one leg extended and the other bent. Hinge forward from the hips toward your extended foot until you feel a gentle stretch in the back of your thigh. Keep your back straight and breathe.",
                    Difficulty = "Easy",
                    CategoryId = 5
                },

                // 7 - Muscle Building
                new Exercise
                {
                    Title = "Dumbbell Bench Press",
                    Description = "Lie on a bench with a dumbbell in each hand at chest level. Press the weights up until your arms are extended, then lower them under control.",
                    Difficulty = "Medium",
                    CategoryId = 7
                },
                new Exercise
                {
                    Title = "Barbell Back Squat",
                    Description = "Set the bar across your upper back and step out from the rack. Squat down until your thighs are at least parallel to the floor, keeping your chest up and knees tracking over your toes, then drive back up.",
                    Difficulty = "Hard",
                    CategoryId = 7
                },

                // 9 - Recovery
                new Exercise
                {
                    Title = "Foam Rolling",
                    Description = "Place the foam roller under the target muscle and use your body weight to roll slowly along it. Pause on tender spots and breathe deeply, avoiding joints and bones.",
                    Difficulty = "Easy",
                    CategoryId = 9
                }
            };

            await db.InsertAllAsync(exercises);
        }

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
                new Workout { Name = "Yoga Flow",    UserId = 1, CategoryId = 4 },
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
                    Title = "How Much Cardio Do You Need Each Week?",
                    Content = "Adults should aim for at least 150 minutes of moderate-intensity aerobic activity a week, or 75 minutes of vigorous activity, or a mix of both, ideally spread through the week. At a moderate pace your heart beats faster and you breathe harder, but you can still talk. Think brisk walking, water aerobics, dancing or easy cycling. At a vigorous pace you'll warm up, sweat and struggle to talk, as with running, swimming laps or jumping rope. If you can't reach 150 minutes yet, that's fine. Walking is the simplest way to start, and brisk five- or ten-minute walks a few times a day add up. Increase your time and intensity gradually, and know that reaching 300 minutes a week brings even more benefits.",
                    Citation = "American Heart Association",
                    Url = "https://www.heart.org/en/healthy-living/fitness/fitness-basics/aha-recs-for-physical-activity-in-adults",
                    Image = "american_heart_association.jpg",
                    DateTimePublished = new DateTime(2024, 1, 19),
                    DateTimeAdded = DateTime.Now.AddDays(-2),
                    CategoryId = 2
                },

                new Article
                {
                    Title = "Strength Training: How Often and How Hard?",
                    Content = "On top of your cardio, adults need muscle-strengthening activity on at least two days a week, working all the major muscle groups: legs, hips, back, chest, abdomen, shoulders and arms. To get the benefit, work each muscle until it's hard to complete another repetition without help. A good target is 8 to 12 repetitions per set. One set per exercise is a solid start, and two or three sets bring extra benefit. Lifting weights, using resistance bands and doing bodyweight exercises like push-ups and sit-ups all count.",
                    Citation = "Centers for Disease Control and Prevention",
                    Url = "https://www.cdc.gov/physical-activity-basics/adding-adults/what-counts.html",
                    Image = "centers_for_disease_control_and_prevention.jpg",
                    DateTimePublished = new DateTime(2023, 12, 6),
                    DateTimeAdded = DateTime.Now.AddDays(-1),
                    CategoryId = 1
                },

                new Article
                {
                    Title = "A Simple Guide to Stretching",
                    Content = "Stretching won't reliably prevent soreness or injury, but it does improve flexibility, the ability to move your joints and muscles through their full range, which makes everyday tasks easier. Before a workout, do a few minutes of dynamic stretching by gently mimicking your activity at lower intensity, such as marching in place before a run or arm circles before swimming. Save static stretches, where you hold a position, for after exercise when your muscles are warm. Hold each for 10 to 30 seconds, with the biggest gains around 30. Target the major muscle groups, with calves, hamstrings and chest and shoulders a good place to start. Stretch only to mild tension, never pain, and try to fit in a few minutes most days.",
                    Citation = "Julie Corliss, Harvard Health Publishing",
                    Url = "https://www.health.harvard.edu/healthy-aging-and-longevity/take-time-to-stretch",
                    Image = "harvard_health_publishing.jpg",
                    DateTimePublished = new DateTime(2024, 8, 1),
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

        private async Task SeedNotesAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<Note>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var notes = new List<Note>
            {
                new Note
                {
                    Title = "My First Note",
                    Content = "Got through my first logged workout today and it felt good to finally get started. I took it slowly to focus on my form, and a few of the movements felt awkward at first, but they got easier as I warmed up. I was out of breath sooner than I expected, so I'll keep the pace comfortable next time and build up gradually. Goal for the next session: stay consistent and pay attention to how my body feels the day after.",
                    UserId = 1,
                    WorkoutId = 1
                },
                new Note
                {
                    Title = "Workout Thoughts",
                    Content = "I felt great after today's workout! My energy was high from the start and I was able to push a bit harder than last time. My legs were tired by the end, but in a good way. I noticed my breathing recovered faster between efforts, which tells me the earlier sessions are paying off. I want to keep this momentum going, so next time I'll try adding a little more challenge while keeping my form clean. Remember to drink more water before the session.",
                    UserId = 1,
                    WorkoutId = 2
                },
                new Note
                {
                    Title = "Yoga Reflection",
                    Content = "The yoga session really helped me relax. My shoulders and lower back were tight when I started, and by the end they felt looser and my mind was a lot calmer. Slowing down and focusing on my breathing made the biggest difference, and holding the poses a little longer helped me notice where I'm stiffest. Balance poses were the hardest part, so I want to keep practising those. I'd like to make this a regular part of my week, especially after harder workouts.",
                    UserId = 1,
                    WorkoutId = 3
                }
            };

            await db.InsertAllAsync(notes);
        }
    }
}