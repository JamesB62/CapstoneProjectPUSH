namespace CapstoneProjectPUSH.Services
{
    public class DatabaseService
    {
        // Admin seed -- for testing only, remove before release
        public const string SeedAdminDisplayName = "Administrator";
        public const string SeedAdminEmail = "admin@push.local";
        public const string SeedAdminPassword = "Admin123!";

        public const string DbFileName = "push.db3";
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
                    await connection.CreateTableAsync<WeightLog>();
                    await connection.CreateTableAsync<Workout>();
                    await connection.CreateTableAsync<WorkoutExercise>();
                    await connection.CreateTableAsync<WorkoutLog>();
                    await connection.CreateTableAsync<WorkoutFavourite>();


                    _connection = connection;

                    // Seeded data -- order specific
                    await SeedCategoriesAsync();
                    await SeedGymsAsync();
                    await SeedAdminAsync();
                    await SeedGuestAsync();
                    await SeedTrainersAsync();
                    await SeedExercisesAsync();
                    await SeedWorkoutsAsync();
                    await SeedWorkoutExercisesAsync();
                    await SeedNotesAsync();
                    await SeedWorkoutLogsAsync();
                    await SeedWeightLogsAsync();
                    await SeedArticlesAsync();
                }
            }
            finally
            {
                _initLock.Release();
            }

            return _connection;
        }

        // Guest seed
        private async Task SeedGuestAsync()
        {
            var db = await GetConnectionAsync();

            var guestExists = await db.Table<User>().Where(u => u.IsGuest == true).CountAsync() > 0;

            if (guestExists)
            {
                return;
            }

            var nameTaken = await db.Table<User>().Where(u => u.DisplayName == "Guest").CountAsync() > 0;

            if (nameTaken)
            {
                return;
            }

            var emailTaken = await db.Table<User>().Where(u => u.Email == "guest@push.local").CountAsync() > 0;

            if (emailTaken)
            {
                return;
            }

            await db.InsertAsync(new User
            {
                Email = "guest@push.local",
                DisplayName = "Guest",
                PasswordHash = string.Empty,
                IsGuest = true
            });
        }
        
        // Admin seed
        private async Task SeedAdminAsync()
        {
            var db = await GetConnectionAsync();

            var adminExists = await db.Table<User>().Where(u => u.IsAdmin == true).CountAsync() > 0;

            if (adminExists)
            {
                return;
            }

            var nameTaken = await db.Table<User>().Where(u => u.DisplayName == SeedAdminDisplayName).CountAsync() > 0;

            if (nameTaken)
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
                DisplayName = SeedAdminDisplayName,
                PasswordHash = UserService.HashPassword(SeedAdminPassword),
                IsAdmin = true
            });
        }

        // Categories seed
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
                "Core",                     // 6
                "Bodyweight",               // 7
                "Pilates",                  // 8
                "Recovery"                  // 9
            };

            await db.InsertAllAsync(categories.Select(c => new Category { Title = c }));
        }

        // Gyms seed
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
                    Name = "Iron Fitness",
                    Address = "14 Kauri Lane, Epsom",
                    City = "Auckland"
                },

                new Gym {
                    Name = "Harbour Strength Co.",
                    Address = "89 Wharf Street, Newmarket",
                    City = "Auckland"
                },

                new Gym {
                    Name = "Summit Lift Club",
                    Address = "5 Totara Avenue, Eden Terrace",
                    City = "Auckland"
                },

                // Hamilton
                new Gym {
                    Name = "Iron Fitness",
                    Address = "210 River Road, Hamilton Central",
                    City = "Hamilton"
                },

                // Wellington
                new Gym {
                    Name = "Peak Performance",
                    Address = "41 Rimu Street, Te Aro",
                    City = "Wellington"
                },

                new Gym {
                    Name = "Foundry Gym",
                    Address = "12 Harbour View Road, Te Aro",
                    City = "Wellington"
                },

                new Gym {
                    Name = "Iron Fitness",
                    Address = "73 Southern Cross Lane, Te Aro",
                    City = "Wellington"
                },

                // Christchurch
                new Gym {
                    Name = "Peak Performance",
                    Address = "9 Matai Crescent, Belfast",
                    City = "Christchurch"
                },

                new Gym {
                    Name = "Southern Core Studio",
                    Address = "150 Plains Road, Addington",
                    City = "Christchurch"
                }
            };

            await db.InsertAllAsync(gyms);
        }

        // Trainers seed
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
                    FirstName = "Liam",
                    LastName = "Walker",
                    Email = "liam.walker@example.com",
                    PhoneNumber = "123-456-7890",
                    CategoryId = 1,
                    Description = "Certified personal trainer with 8 years of experience in strength training. Liam focuses on proper technique and progressive overload, helping clients build lasting strength safely, whether they're picking up a barbell for the first time or working toward a new personal best.",
                    Available = true,
                    GymId = 1
                },

                new Trainer {
                    FirstName = "Aroha",
                    LastName = "Ngata",
                    Email = "aroha.ngata@example.com",
                    PhoneNumber = "123-456-7890",
                    CategoryId = 3,
                    Description = "Certified personal trainer with 5 years of experience specialising in high-intensity interval training. Aroha designs fast-paced, varied sessions that build endurance and burn calories efficiently, and scales every workout to suit beginners through to experienced athletes.",
                    Available = false,
                    GymId = 4
                },

                new Trainer {
                    FirstName = "Sam",
                    LastName = "Patel",
                    Email = string.Empty,
                    PhoneNumber = "123-456-7890",
                    CategoryId = 2,
                    Description = "Cardio and endurance coach with 3 years of experience. Sam helps clients improve their heart health and stamina through running, rowing and cycling plans, building up gradually so progress lasts.",
                    Available = true,
                    GymId = 6
                },

                new Trainer {
                    FirstName = "Mei",
                    LastName = "Chen",
                    Email = "mei.chen@example.com",
                    PhoneNumber = string.Empty,
                    CategoryId = 4,
                    Description = "Registered yoga instructor with 8 years of teaching experience. Mei runs calm, beginner-friendly classes that build flexibility, balance and breathing control, with options to progress for more experienced students.",
                    Available = true,
                    GymId = 8
                }
            };

            await db.InsertAllAsync(trainers);
        }

        // Exercises seed
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
                // 1 - Strength Training
                new Exercise
                {
                    Title = "Dumbbell Bench Press",
                    Description = "Lie on a bench with a dumbbell in each hand at chest level. Press the weights up until your arms are extended, then lower them under control.",
                    Difficulty = 3,
                    CategoryId = 1
                },
                new Exercise
                {
                    Title = "Barbell Back Squat",
                    Description = "Set the bar across your upper back and step out from the rack. Squat down until your thighs are at least parallel to the floor, keeping your chest up and knees tracking over your toes, then drive back up.",
                    Difficulty = 5,
                    CategoryId = 1
                },

                // 2 - Cardio
                new Exercise
                {
                    Title = "Brisk Walk",
                    Description = "Walk at a pace that raises your heart rate but still lets you hold a conversation. Keep your posture tall, shoulders relaxed, and swing your arms naturally.",
                    Difficulty = 1,
                    IsTimed = true,
                    CategoryId = 2
                },
                new Exercise
                {
                    Title = "Jump Rope",
                    Description = "Hold the handles at hip height and turn the rope with your wrists. Jump just high enough to clear the rope, landing softly on the balls of your feet.",
                    Difficulty = 3,
                    IsTimed = true,
                    CategoryId = 2
                },

                // 3 - HIIT
                new Exercise
                {
                    Title = "Burpee",
                    Description = "From standing, squat down and place your hands on the floor. Jump your feet back into a plank, lower your chest to the floor, push up, jump your feet forward, and explode upward into a jump.",
                    Difficulty = 4,
                    CategoryId = 3
                },

                // 4 - Yoga
                new Exercise
                {
                    Title = "Downward-Facing Dog",
                    Description = "Start on hands and knees, tuck your toes, and lift your hips up and back. Press your hands into the mat, keep your spine long, and let your head hang between your arms.",
                    Difficulty = 3,
                    IsTimed = true,
                    CategoryId = 4
                },

                // 5 - Flexibility & Mobility
                new Exercise
                {
                    Title = "Seated Hamstring Stretch",
                    Description = "Sit with one leg extended and the other bent. Hinge forward from the hips toward your extended foot until you feel a gentle stretch in the back of your thigh. Keep your back straight and breathe.",
                    Difficulty = 2,
                    IsTimed = true,
                    CategoryId = 5
                },

                // 6 - Core
                new Exercise
                {
                    Title = "Plank",
                    Description = "Rest on your forearms and toes with your elbows under your shoulders. Keep your body in a straight line from head to heels, brace your stomach and avoid letting your hips sag or lift.",
                    Difficulty = 2,
                    IsTimed = true,
                    CategoryId = 6
                },

                // 7 - Bodyweight
                new Exercise
                {
                    Title = "Bodyweight Squat",
                    Description = "Stand with feet shoulder-width apart. Push your hips back and bend your knees to lower down, keeping your chest up and heels flat. Drive through your feet to return to standing.",
                    Difficulty = 2,
                    CategoryId = 7
                },
                new Exercise
                {
                    Title = "Push-Up",
                    Description = "Start in a high plank with hands slightly wider than your shoulders. Lower your chest toward the floor while keeping your body in a straight line, then press back up.",
                    Difficulty = 3,
                    CategoryId = 7
                },

                // 8 - Pilates
                new Exercise
                {
                    Title = "Pilates Hundred",
                    Description = "Lie on your back with your legs raised and your head and shoulders lifted. Keep your arms straight by your sides and pump them up and down, breathing in for five pumps and out for five.",
                    Difficulty = 3,
                    CategoryId = 8
                },

                // 9 - Recovery
                new Exercise
                {
                    Title = "Foam Rolling",
                    Description = "Place the foam roller under the target muscle and use your body weight to roll slowly along it. Pause on tender spots and breathe deeply, avoiding joints and bones.",
                    Difficulty = 1,
                    IsTimed = true,
                    CategoryId = 9
                }
            };

            await db.InsertAllAsync(exercises);
        }

        // Articles seed
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
                    Title = "Meal Prep Basics for a Busy Week",
                    Content = "Spending an hour or two on the weekend preparing meals can make healthy eating much easier during a busy week. Start by choosing two or three simple recipes you enjoy and cook them in larger batches. A good meal prep container has a protein such as chicken, tofu or eggs, a carbohydrate such as rice, pasta or potatoes, and plenty of vegetables. Store meals in the fridge for up to three days and freeze the rest. Keep a few quick snacks on hand, like yoghurt, fruit or nuts, so you're not caught out between meals. Don't aim for perfect. A few prepared meals each week is a great start.",
                    Citation = "PUSH Team",
                    CitationUrl = string.Empty,
                    Image = "article_placeholder.png",
                    DateTimePublished = new DateTime(2026, 9, 14),
                    Topic = "Meal Prep"
                },

                new Article
                {
                    Title = "What to Eat Before and After a Workout",
                    Content = "What you eat around your workouts can help you train better and recover faster. Around one to three hours before training, have a light meal with carbohydrates for energy and a little protein, such as toast with peanut butter, a banana with yoghurt or a bowl of oats. Avoid heavy, greasy food right before exercising, as it can leave you feeling sluggish. After your workout, aim for a meal with protein to help your muscles recover and carbohydrates to refuel, such as a chicken wrap, eggs on toast or a smoothie with milk and fruit. Drink water throughout the day, not just during your session.",
                    Citation = "PUSH Team",
                    CitationUrl = string.Empty,
                    Image = "article_placeholder.png",
                    DateTimePublished = new DateTime(2026, 10, 5),
                    Topic = "Nutrition"
                },

                new Article
                {
                    Title = "Building a Balanced Plate",
                    Content = "A simple way to plan meals without counting anything is to picture your plate in sections. Fill about half with vegetables or salad, a quarter with a protein such as fish, meat, beans or tofu, and a quarter with a carbohydrate such as rice, wholegrain bread or kumara. Add a small amount of healthy fat, like olive oil, avocado or nuts. This approach works for breakfast, lunch and dinner, and it's easy to adjust. Add more carbohydrates on heavy training days and more vegetables on rest days. Try planning your meals a few days ahead so shopping is quicker and you waste less food.",
                    Citation = "PUSH Team",
                    CitationUrl = string.Empty,
                    Image = "article_placeholder.png",
                    DateTimePublished = new DateTime(2026, 10, 26),
                    Topic = "Meal Planning"
                }
            };

            await db.InsertAllAsync(articles);
        }

        // Workouts seed
        private async Task SeedWorkoutsAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<Workout>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var admin = await db.Table<User>().Where(u => u.Email == SeedAdminEmail).FirstOrDefaultAsync();
            var guest = await db.Table<User>().Where(u => u.IsGuest).FirstOrDefaultAsync();

            var workouts = new List<Workout>();

            if (admin is not null)
            {
                // Must stay private
                workouts.Add(new Workout { Title = "Strength Day", UserId = admin.Id, CategoryId = 1, IsPrivate = true });
                workouts.Add(new Workout { Title = "Morning HIIT", UserId = admin.Id, CategoryId = 3, IsPrivate = true });
                workouts.Add(new Workout { Title = "Yoga Flow", UserId = admin.Id, CategoryId = 4, IsPrivate = true });
            }

            if (guest is not null)
            {
                // Must stay private
                workouts.Add(new Workout { Title = "Beginner Full Body", UserId = guest.Id, CategoryId = 7, IsPrivate = true });
                workouts.Add(new Workout { Title = "Quick Cardio", UserId = guest.Id, CategoryId = 2, IsPrivate = true });
                workouts.Add(new Workout { Title = "Gentle Stretch", UserId = guest.Id, CategoryId = 5, IsPrivate = true });
            }

            await db.InsertAllAsync(workouts);
        }

        // Workout exercises seed
        private async Task SeedWorkoutExercisesAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<WorkoutExercise>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var workoutIds = new Dictionary<string, int>();
            foreach (var workout in await db.Table<Workout>().ToListAsync())
            {
                workoutIds[workout.Title] = workout.Id;
            }

            var exerciseIds = new Dictionary<string, int>();
            foreach (var exercise in await db.Table<Exercise>().ToListAsync())
            {
                exerciseIds[exercise.Title] = exercise.Id;
            }

            var workoutExercises = new List<WorkoutExercise>();

            void Add(string workoutTitle, string exerciseTitle, int sortOrder, int sets, int reps)
            {
                if (!workoutIds.TryGetValue(workoutTitle, out var workoutId) || !exerciseIds.TryGetValue(exerciseTitle, out var exerciseId))
                {
                    System.Diagnostics.Debug.WriteLine($"Seed skipped: {workoutTitle} / {exerciseTitle}");
                    return;
                }

                workoutExercises.Add(new WorkoutExercise
                {
                    WorkoutId = workoutId,
                    ExerciseId = exerciseId,
                    SortOrder = sortOrder,
                    Sets = sets,
                    Reps = reps
                });
            }

            void AddTimed(string workoutTitle, string exerciseTitle, int sortOrder, int minutes)
            {
                if (!workoutIds.TryGetValue(workoutTitle, out var workoutId) || !exerciseIds.TryGetValue(exerciseTitle, out var exerciseId))
                {
                    System.Diagnostics.Debug.WriteLine($"Seed skipped: {workoutTitle} / {exerciseTitle}");
                    return;
                }

                workoutExercises.Add(new WorkoutExercise
                {
                    WorkoutId = workoutId,
                    ExerciseId = exerciseId,
                    SortOrder = sortOrder,
                    DurationMinutes = minutes
                });
            }

            // Admin workouts
            Add("Strength Day", "Barbell Back Squat", 1, 4, 8);
            Add("Strength Day", "Dumbbell Bench Press", 2, 4, 8);
            Add("Strength Day", "Push-Up", 3, 3, 12);

            Add("Morning HIIT", "Burpee", 1, 4, 10);
            AddTimed("Morning HIIT", "Jump Rope", 2, 5);
            Add("Morning HIIT", "Bodyweight Squat", 3, 3, 20);

            AddTimed("Yoga Flow", "Downward-Facing Dog", 1, 5);
            Add("Yoga Flow", "Pilates Hundred", 2, 1, 100);
            AddTimed("Yoga Flow", "Seated Hamstring Stretch", 3, 5);

            // Guest workouts
            Add("Beginner Full Body", "Bodyweight Squat", 1, 3, 12);
            Add("Beginner Full Body", "Push-Up", 2, 3, 8);
            AddTimed("Beginner Full Body", "Plank", 3, 3);

            AddTimed("Quick Cardio", "Brisk Walk", 1, 10);
            AddTimed("Quick Cardio", "Jump Rope", 2, 5);
            Add("Quick Cardio", "Burpee", 3, 3, 8);

            AddTimed("Gentle Stretch", "Seated Hamstring Stretch", 1, 5);
            AddTimed("Gentle Stretch", "Downward-Facing Dog", 2, 5);
            AddTimed("Gentle Stretch", "Foam Rolling", 3, 10);

            if (workoutExercises.Count > 0)
            {
                await db.InsertAllAsync(workoutExercises);
            }
        }

        // Notes seed
        private async Task SeedNotesAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<Note>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var workouts = await db.Table<Workout>().ToListAsync();
            var notes = new List<Note>();

            void Add(string workoutTitle, string title, string content, int daysAgo, int hour)
            {
                var workout = workouts.FirstOrDefault(w => w.Title == workoutTitle);

                if (workout is null)
                {
                    System.Diagnostics.Debug.WriteLine($"Seed skipped: note {title} / {workoutTitle}");
                    return;
                }

                notes.Add(new Note
                {
                    Title = title,
                    Content = content,
                    DateTimeCreated = DateTime.Today.AddDays(-daysAgo).AddHours(hour),
                    UserId = workout.UserId,
                    WorkoutId = workout.Id
                });
            }

            // Admin notes
            Add("Strength Day", "New Squat Weight",
                "Moved up 2.5 kg on the back squat today and all four sets felt solid. My depth was good on every rep, but the last set slowed down, so I'll stay at this weight next session before going up again. Bench felt strong too.",
                12, 7);

            Add("Morning HIIT", "Energy Was High",
                "Felt great this morning and pushed harder than last time. My breathing recovered faster between the burpee sets, which tells me the earlier sessions are paying off. Remember to drink more water before the session.",
                6, 6);

            Add("Yoga Flow", "Yoga Reflection",
                "This session really helped me relax. My shoulders and lower back were tight when I started and felt looser by the end. Holding downward dog a little longer showed me where I'm stiffest. I'd like to make this a regular part of my week after harder workouts.",
                3, 18);

            // Guest notes
            Add("Beginner Full Body", "My First Workout",
                "Got through my first workout today and it felt good to finally get started. I took it slowly to focus on my form. The plank was harder than I expected, so I'll aim to hold it a little longer each time.",
                10, 17);

            Add("Quick Cardio", "Short but Tough",
                "Only twenty minutes but I was out of breath by the end of the burpees. The brisk walk was a good warm-up. Next time I'll try to keep the jump rope going without stopping.",
                5, 12);

            Add("Gentle Stretch", "Rest Day Stretch",
                "Did this on my rest day after my legs were sore from the full body workout. The hamstring stretch and foam rolling helped a lot, and I felt less stiff the next morning.",
                1, 20);

            if (notes.Count > 0)
            {
                await db.InsertAllAsync(notes);
            }
        }

        // Workout logs seed
        private async Task SeedWorkoutLogsAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<WorkoutLog>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var workouts = await db.Table<Workout>().ToListAsync();

            // Admin workouts
            var strengthDay = workouts.FirstOrDefault(w => w.Title == "Strength Day");
            var morningHiit = workouts.FirstOrDefault(w => w.Title == "Morning HIIT");
            var yogaFlow = workouts.FirstOrDefault(w => w.Title == "Yoga Flow");

            // Guest workouts
            var beginnerFullBody = workouts.FirstOrDefault(w => w.Title == "Beginner Full Body");
            var quickCardio = workouts.FirstOrDefault(w => w.Title == "Quick Cardio");
            var gentleStretch = workouts.FirstOrDefault(w => w.Title == "Gentle Stretch");

            if (strengthDay is null || morningHiit is null || yogaFlow is null
                || beginnerFullBody is null || quickCardio is null || gentleStretch is null)
            {
                System.Diagnostics.Debug.WriteLine("Workout log seed skipped: seeded workouts not found");
                return;
            }

            var workoutLogs = new List<WorkoutLog>
            {
                // Admin - Week
                CreateWorkoutLog(strengthDay, 0, 45),
                CreateWorkoutLog(morningHiit, 1, 30),
                CreateWorkoutLog(yogaFlow, 2, 60),
                CreateWorkoutLog(strengthDay, 3, 40),
                CreateWorkoutLog(morningHiit, 5, 35),

                // Admin - Month
                CreateWorkoutLog(strengthDay, 8, 45),
                CreateWorkoutLog(yogaFlow, 10, 30),
                CreateWorkoutLog(morningHiit, 13, 60),
                CreateWorkoutLog(strengthDay, 15, 40),
                CreateWorkoutLog(morningHiit, 20, 50),
                CreateWorkoutLog(yogaFlow, 24, 35),
                CreateWorkoutLog(strengthDay, 27, 45),

                // Admin - Year
                CreateWorkoutLog(strengthDay, 40, 50),
                CreateWorkoutLog(morningHiit, 70, 45),
                CreateWorkoutLog(yogaFlow, 100, 60),
                CreateWorkoutLog(strengthDay, 130, 30),
                CreateWorkoutLog(morningHiit, 160, 55),
                CreateWorkoutLog(yogaFlow, 200, 40),
                CreateWorkoutLog(strengthDay, 250, 70),
                CreateWorkoutLog(morningHiit, 300, 45),

                // Guest - Week
                CreateWorkoutLog(beginnerFullBody, 0, 30),
                CreateWorkoutLog(quickCardio, 1, 20),
                CreateWorkoutLog(gentleStretch, 2, 20),
                CreateWorkoutLog(beginnerFullBody, 3, 35),
                CreateWorkoutLog(quickCardio, 5, 25),

                // Guest - Month
                CreateWorkoutLog(beginnerFullBody, 8, 30),
                CreateWorkoutLog(gentleStretch, 10, 20),
                CreateWorkoutLog(quickCardio, 13, 25),
                CreateWorkoutLog(beginnerFullBody, 15, 35),
                CreateWorkoutLog(quickCardio, 20, 20),
                CreateWorkoutLog(gentleStretch, 24, 15),
                CreateWorkoutLog(beginnerFullBody, 27, 30),

                // Guest - Year
                CreateWorkoutLog(beginnerFullBody, 40, 30),
                CreateWorkoutLog(quickCardio, 70, 20),
                CreateWorkoutLog(gentleStretch, 100, 20),
                CreateWorkoutLog(beginnerFullBody, 130, 25),
                CreateWorkoutLog(quickCardio, 160, 25),
                CreateWorkoutLog(gentleStretch, 200, 15),
                CreateWorkoutLog(beginnerFullBody, 250, 30),
                CreateWorkoutLog(quickCardio, 300, 20)
            };

            await db.InsertAllAsync(workoutLogs);
        }

        // The log belongs to the workout's owner
        private static WorkoutLog CreateWorkoutLog(Workout workout, int daysAgo, int minutes)
        {
            return new WorkoutLog
            {
                WorkoutId = workout.Id,
                UserId = workout.UserId,
                DateTimeCompleted = DateTime.Today.AddDays(-daysAgo).AddHours(7),
                DurationSeconds = minutes * 60
            };
        }

        // Weight logs seed
        private async Task SeedWeightLogsAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<WeightLog>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var admin = await db.Table<User>().Where(u => u.Email == SeedAdminEmail).FirstOrDefaultAsync();
            var guest = await db.Table<User>().Where(u => u.IsGuest).FirstOrDefaultAsync();

            if (admin is null || guest is null)
            {
                System.Diagnostics.Debug.WriteLine("Seed skipped: weight logs (admin or guest not found)");
                return;
            }

            var weightLogs = new List<WeightLog>
            {
                // Admin - Week
                CreateWeightLog(admin.Id, 0, 80.5),
                CreateWeightLog(admin.Id, 1, 80.2),
                CreateWeightLog(admin.Id, 2, 80.0),
                CreateWeightLog(admin.Id, 3, 79.8),
                CreateWeightLog(admin.Id, 5, 79.5),

                // Admin - Month
                CreateWeightLog(admin.Id, 8, 79.3),
                CreateWeightLog(admin.Id, 10, 79.0),
                CreateWeightLog(admin.Id, 13, 78.8),
                CreateWeightLog(admin.Id, 15, 78.5),
                CreateWeightLog(admin.Id, 20, 78.3),
                CreateWeightLog(admin.Id, 24, 78.0),
                CreateWeightLog(admin.Id, 27, 77.8),

                // Admin - Year
                CreateWeightLog(admin.Id, 40, 77.5),
                CreateWeightLog(admin.Id, 70, 77.3),
                CreateWeightLog(admin.Id, 100, 77.0),
                CreateWeightLog(admin.Id, 130, 76.8),
                CreateWeightLog(admin.Id, 160, 76.5),
                CreateWeightLog(admin.Id, 200, 76.3),
                CreateWeightLog(admin.Id, 250, 76.0),
                CreateWeightLog(admin.Id, 300, 75.8),

                // Guest - Week
                CreateWeightLog(guest.Id, 0, 68.0),
                CreateWeightLog(guest.Id, 1, 68.2),
                CreateWeightLog(guest.Id, 2, 68.3),
                CreateWeightLog(guest.Id, 3, 68.5),
                CreateWeightLog(guest.Id, 5, 68.6),

                // Guest - Month
                CreateWeightLog(guest.Id, 8, 68.8),
                CreateWeightLog(guest.Id, 10, 69.0),
                CreateWeightLog(guest.Id, 13, 69.3),
                CreateWeightLog(guest.Id, 15, 69.5),
                CreateWeightLog(guest.Id, 20, 69.8),
                CreateWeightLog(guest.Id, 24, 70.0),
                CreateWeightLog(guest.Id, 27, 70.2),

                // Guest - Year
                CreateWeightLog(guest.Id, 40, 70.5),
                CreateWeightLog(guest.Id, 70, 71.0),
                CreateWeightLog(guest.Id, 100, 71.4),
                CreateWeightLog(guest.Id, 130, 71.8),
                CreateWeightLog(guest.Id, 160, 72.1),
                CreateWeightLog(guest.Id, 200, 72.5),
                CreateWeightLog(guest.Id, 250, 72.8),
                CreateWeightLog(guest.Id, 300, 73.0)
            };

            await db.InsertAllAsync(weightLogs);
        }

        private static WeightLog CreateWeightLog(int userId, int daysAgo, double weightKg)
        {
            return new WeightLog
            {
                UserId = userId,
                DateTimeLogged = DateTime.Today.AddDays(-daysAgo).AddHours(7),
                WeightKg = weightKg
            };
        }
    }
}