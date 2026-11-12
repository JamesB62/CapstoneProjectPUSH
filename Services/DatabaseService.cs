using Android.Icu.Math;

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
                    await connection.CreateTableAsync<Stats>();
                    await connection.CreateTableAsync<Trainer>();
                    await connection.CreateTableAsync<User>();
                    await connection.CreateTableAsync<UserGym>();
                    await connection.CreateTableAsync<WeightLog>();
                    await connection.CreateTableAsync<Workout>();
                    await connection.CreateTableAsync<WorkoutExercise>();
                    await connection.CreateTableAsync<WorkoutLog>();

                    _connection = connection;

                    // Seeded data -- order specific
                    await SeedCategoriesAsync();
                    await SeedGymsAsync();
                    await SeedAdminAsync();
                    await SeedGuestAsync();
                    await SeedTrainersAsync();
                    await SeedWorkoutsAsync();
                    await SeedExercisesAsync();
                    await SeedArticlesAsync();
                    // await SeedWorkoutExercisesAsync();
                    await SeedWorkoutLogsAsync();
                    await SeedNotesAsync();
                    await SeedWeightLogsAsync();
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
                "Weight Loss",              // 6
                "Muscle Building",          // 7
                "Nutrition",                // 8
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
                    Name = "ALLFIT",
                    Address = "32 Manukau Road, Epsom",
                    City = "Auckland"
                },

                new Gym {
                    Name = "Les Mills",
                    Address = "269 Khyber Pass Road, Newmarket",
                    City = "Auckland"
                },

                new Gym {
                    Name = "Lift Club",
                    Address = "24 Minnie Street, Eden Terrace",
                    City = "Auckland"
                },

                // Hamilton
                new Gym {
                    Name = "Les Mills",
                    Address = "747 Victoria Street, Hamilton Central",
                    City = "Hamilton"
                },

                // Wellington
                new Gym {
                    Name = "Flex Fitness",
                    Address = "222a Willis Street, Te Aro",
                    City = "Wellington"
                },

                new Gym {
                    Name = "Jetts",
                    Address = "James Smith Corner 49 First Floor/61 Cuba Street, Te Aro",
                    City = "Wellington"
                },

                new Gym {
                    Name = "Les Mills",
                    Address = "52/70 Taranaki Street, Te Aro",
                    City = "Wellington"
                },

                // Christchurch
                new Gym {
                    Name = "Flex Fitness",
                    Address = "4 Bellewood Avenue, Belfast",
                    City = "Christchurch"
                },

                new Gym {
                    Name = "Inception Gym",
                    Address = "65 Blenheim Road, Addington",
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
                },

                new Trainer {
                    FirstName = "Daniel",
                    LastName = "Doe",
                    Email = "ddoe123@example.com",
                    PhoneNumber = "123-456-7890",
                    CategoryId = 2,
                    Description = "Certified personal trainer with 3 years of experience in cardio training. Daniel focuses on proper technique and progressive overload, helping clients build lasting strength safely, whether they're picking up a barbell for the first time or working toward a new personal best.",
                    Available = true,
                    GymId = 2
                },
                new Trainer {
                    FirstName = "Alex",
                    LastName = "Dane",
                    Email = "adane123@example.com",
                    PhoneNumber = "123-456-7890",
                    CategoryId = 4,
                    Description = "Certified personal trainer with 8 years of experience in yoga instructing. Alex focuses on proper technique and progressive overload, helping clients build lasting strength safely, whether they're picking up a barbell for the first time or working toward a new personal best.",
                    Available = true,
                    GymId = 5
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
                    Title = "Bodyweight Squat",
                    Description = "Stand with feet shoulder-width apart. Push your hips back and bend your knees to lower down, keeping your chest up and heels flat. Drive through your feet to return to standing.",
                    Difficulty = 2,
                    CategoryId = 1
                },
                new Exercise
                {
                    Title = "Push-Up",
                    Description = "Start in a high plank with hands slightly wider than your shoulders. Lower your chest toward the floor while keeping your body in a straight line, then press back up.",
                    Difficulty = 3,
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

                // 7 - Muscle Building
                new Exercise
                {
                    Title = "Dumbbell Bench Press",
                    Description = "Lie on a bench with a dumbbell in each hand at chest level. Press the weights up until your arms are extended, then lower them under control.",
                    Difficulty = 3,
                    CategoryId = 7
                },
                new Exercise
                {
                    Title = "Barbell Back Squat",
                    Description = "Set the bar across your upper back and step out from the rack. Squat down until your thighs are at least parallel to the floor, keeping your chest up and knees tracking over your toes, then drive back up.",
                    Difficulty = 5,
                    CategoryId = 7
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

            //line with workout 
            var exerciseIds = exercises.ToDictionary(e => e.Title, e => e.Id);

            var guest = await db.Table<User>().Where(u => u.IsGuest ==true).FirstOrDefaultAsync();
            var workouts = await db.Table<Workout>().ToListAsync();
            var items = new List<WorkoutExercise>();
            
            void Add(int userId, string workout, string exercise, int order, int sets, int reps)
            {
                var w = workouts.FirstOrDefault(x => x.UserId == userId && x.Title == workout);

                if (w is null || !exerciseIds.TryGetValue(exercise, out var eId))
                {
                    System.Diagnostics.Debug.WriteLine($"Seed skipped: {workout} / {exercise}");
                    return;
                }

                items.Add(new WorkoutExercise { WorkoutId = w.Id, ExerciseId = eId, SortOrder = order, Sets = sets, Reps = reps });
            }

            //amdin see workout
            const int demo = 1;
            Add(demo, "Push Day", "Dumbbell Bench Press", 1, 4, 8);
            Add(demo, "Push Day", "Push-Up",              2, 3, 15);

            Add(demo, "Leg Day", "Barbell Back Squat", 1, 4, 8);
            Add(demo, "Leg Day", "Bodyweight Squat",   2, 3, 15);

            Add(demo, "Yoga Flow", "Downward-Facing Dog",      1, 3, 8);
            Add(demo, "Yoga Flow", "Seated Hamstring Stretch", 2, 2, 3);

            Add(demo, "Morning HIIT", "Burpee",           1, 4, 15);
            Add(demo, "Morning HIIT", "Jump Rope",        2, 4, 50);
            Add(demo, "Morning HIIT", "Bodyweight Squat", 3, 3, 20);

            Add(demo, "Cardio Burn", "Jump Rope",  1, 3, 100);
            Add(demo, "Cardio Burn", "Brisk Walk", 2, 1, 20);

            Add(demo, "Cool Down", "Seated Hamstring Stretch", 1, 2, 3);
            Add(demo, "Cool Down", "Foam Rolling",             2, 1, 10);

            if (guest is not null)
            {
                var g = guest.Id;

                Add(g, "Beginner Full Body", "Bodyweight Squat", 1, 3, 12);
                Add(g, "Beginner Full Body", "Push-Up",          2, 3, 8);
                Add(g, "Beginner Full Body", "Foam Rolling",     3, 1, 10);

                Add(g, "Quick Cardio", "Jump Rope",  1, 3, 100);
                Add(g, "Quick Cardio", "Burpee",     2, 3, 10);
                Add(g, "Quick Cardio", "Brisk Walk", 3, 1, 20);

                Add(g, "Gentle Stretch", "Downward-Facing Dog",      1, 3, 8);
                Add(g, "Gentle Stretch", "Seated Hamstring Stretch", 2, 2, 3);
                Add(g, "Gentle Stretch", "Foam Rolling",             3, 1, 10);
            }
            
            if (items.Count > 0)
            {
                await db.InsertAllAsync(items);
            }

    
        }

        // Articles seed -- find better sources with usage rights
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
                    CitationUrl = "https://www.heart.org/en/healthy-living/fitness/fitness-basics/aha-recs-for-physical-activity-in-adults",
                    Image = "american_heart_association.jpg",
                    DateTimePublished = new DateTime(2024, 1, 19),
                    CategoryId = 2
                },

                new Article
                {
                    Title = "Strength Training: How Often and How Hard?",
                    Content = "On top of your cardio, adults need muscle-strengthening activity on at least two days a week, working all the major muscle groups: legs, hips, back, chest, abdomen, shoulders and arms. To get the benefit, work each muscle until it's hard to complete another repetition without help. A good target is 8 to 12 repetitions per set. One set per exercise is a solid start, and two or three sets bring extra benefit. Lifting weights, using resistance bands and doing bodyweight exercises like push-ups and sit-ups all count.",
                    Citation = "Centers for Disease Control and Prevention",
                    CitationUrl = "https://www.cdc.gov/physical-activity-basics/adding-adults/what-counts.html",
                    Image = "centers_for_disease_control_and_prevention.jpg",
                    DateTimePublished = new DateTime(2023, 12, 6),
                    CategoryId = 1
                },

                new Article
                {
                    Title = "A Simple Guide to Stretching",
                    Content = "Stretching won't reliably prevent soreness or injury, but it does improve flexibility, the ability to move your joints and muscles through their full range, which makes everyday tasks easier. Before a workout, do a few minutes of dynamic stretching by gently mimicking your activity at lower intensity, such as marching in place before a run or arm circles before swimming. Save static stretches, where you hold a position, for after exercise when your muscles are warm. Hold each for 10 to 30 seconds, with the biggest gains around 30. Target the major muscle groups, with calves, hamstrings and chest and shoulders a good place to start. Stretch only to mild tension, never pain, and try to fit in a few minutes most days.",
                    Citation = "Julie Corliss, Harvard Health Publishing",
                    CitationUrl = "https://www.health.harvard.edu/healthy-aging-and-longevity/take-time-to-stretch",
                    Image = "harvard_health_publishing.jpg",
                    DateTimePublished = new DateTime(2024, 8, 1),
                    CategoryId = 5
                }
            };

            await db.InsertAllAsync(articles);
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

            var notes = new List<Note>
            {
                new Note {
                    Title = "My First Note",
                    Content = "Got through my first logged workout today and it felt good to finally get started. I took it slowly to focus on my form, and a few of the movements felt awkward at first, but they got easier as I warmed up. I was out of breath sooner than I expected, so I'll keep the pace comfortable next time and build up gradually. Goal for the next session: stay consistent and pay attention to how my body feels the day after.",
                    DateTimeCreated = DateTime.Today.AddDays(-12).AddHours(19),
                    UserId = 1,
                    WorkoutId = 1
                },
                new Note {
                    Title = "Workout Thoughts",
                    Content = "I felt great after today's workout! My energy was high from the start and I was able to push a bit harder than last time. My legs were tired by the end, but in a good way. I noticed my breathing recovered faster between efforts, which tells me the earlier sessions are paying off. I want to keep this momentum going, so next time I'll try adding a little more challenge while keeping my form clean. Remember to drink more water before the session.",
                    DateTimeCreated = DateTime.Today.AddDays(-8).AddHours(3),
                    UserId = 1,
                    WorkoutId = 2
                },
                new Note {
                    Title = "Yoga Reflection",
                    Content = "The yoga session really helped me relax. My shoulders and lower back were tight when I started, and by the end they felt looser and my mind was a lot calmer. Slowing down and focusing on my breathing made the biggest difference, and holding the poses a little longer helped me notice where I'm stiffest. Balance poses were the hardest part, so I want to keep practising those. I'd like to make this a regular part of my week, especially after harder workouts.",
                    DateTimeCreated = DateTime.Today.AddDays(-4).AddHours(12),
                    UserId = 1,
                    WorkoutId = 3
                },
                new Note {
                    Title = "Tough Workout",
                    Content = "This is a test note.",
                    DateTimeCreated = DateTime.Today.AddDays(-2).AddHours(4),
                    UserId = 2,
                    WorkoutId = 2
                }
            };

            await db.InsertAllAsync(notes);
        }

        // ================================================ DOUBLE CHECK =================================
        // Workouts seed
        private async Task SeedWorkoutsAsync()
        {
            var db = await GetConnectionAsync();
            if(await db.Table<Workout>().CountAsync() > 0) return;

            await db.InsertAllAsync(new[]
            {
                
                new Workout { Title = "Push Day",     UserId = 1, CategoryId = 1 },
                new Workout { Title = "Leg Day",      UserId = 1, CategoryId = 1 },
                new Workout { Title = "Morning HIIT", UserId = 1, CategoryId = 3 },
                new Workout { Title = "Cardio Burn",  UserId = 1, CategoryId = 2 },
                new Workout { Title = "Yoga Flow",    UserId = 1, CategoryId = 4 },
                new Workout { Title = "Cool Down",    UserId = 1, CategoryId = 9 },
            });

            var guest = await db.Table<User>().Where(u => u.IsGuest == true).FirstOrDefaultAsync();
            if(guest is null) return;
            if(await db.Table<Workout>().Where(w => w.UserId == guest.Id).CountAsync() > 0) return;

            var exerciseIds = (await db.Table<Exercise>().ToListAsync())
                .GroupBy(e => e.Title).ToDictionary(e => e.Key, e => e.First().Id);
            var workoutIds = (await db.Table<Workout>().ToListAsync())
            .GroupBy(w => w.Title).ToDictionary(w => w.Key, w => w.First().Id);

            var items = new List<WorkoutExercise>();

            await db.InsertAllAsync(new[]
            {
                new Workout {Title ="Beginner Full Boday", UserId = guest.Id, CategoryId = 1},
                new Workout { Title = "Quick Cardio",       UserId = guest.Id, CategoryId = 2 },
                new Workout { Title = "Gentle Stretch",     UserId = guest.Id, CategoryId = 5 },
            });

            var count = await db.Table<Workout>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var workouts = new List<Workout>
            {
                new Workout { Title = "Full Body Strength", UserId = 1, CategoryId = 1, IsPrivate = true },
                new Workout { Title = "Cardio Burn", UserId = 1, CategoryId = 2, IsPrivate = false },
                new Workout { Title = "Yoga & Recovery", UserId = 1, CategoryId = 4, IsPrivate = true }
            };

            await db.InsertAllAsync(workouts);


        }

        //put those data in the *SeedWorkoutsAsync* 
        // Workout exercises seed
//         private async Task SeedWorkoutExercisesAsync()
//         {
//             var db = await GetConnectionAsync();

//             var count = await db.Table<WorkoutExercise>().CountAsync();
//             if (count > 0)
//             {
//                 return;
//             }

//             var workoutExercises = new List<WorkoutExercise>
//             {
//                 // Full Body Strength
//                 new WorkoutExercise { WorkoutId = 1, ExerciseId = 9, SortOrder = 1, Sets = 4, Reps = 8 },
//                 new WorkoutExercise { WorkoutId = 1, ExerciseId = 8, SortOrder = 2, Sets = 4, Reps = 8 },
//                 new WorkoutExercise { WorkoutId = 1, ExerciseId = 2, SortOrder = 3, Sets = 3, Reps = 15 },

//                 // Cardio Burn
//                 new WorkoutExercise { WorkoutId = 2, ExerciseId = 3, SortOrder = 1, DurationMinutes = 20 },
//                 new WorkoutExercise { WorkoutId = 2, ExerciseId = 4, SortOrder = 2, DurationMinutes = 10 },
//                 new WorkoutExercise { WorkoutId = 2, ExerciseId = 5, SortOrder = 3, Sets = 3, Reps = 10 },

//                 // Yoga & Recovery
//                 new WorkoutExercise { WorkoutId = 3, ExerciseId = 6, SortOrder = 1, DurationMinutes = 5 },
//                 new WorkoutExercise { WorkoutId = 3, ExerciseId = 7, SortOrder = 2, DurationMinutes = 5 },
//                 new WorkoutExercise { WorkoutId = 3, ExerciseId = 10, SortOrder = 3, DurationMinutes = 10 }
//             };

//             await db.InsertAllAsync(workoutExercises);
//         }

        // Workout logs seed
        private async Task SeedWorkoutLogsAsync()
        {
            var db = await GetConnectionAsync();

            var count = await db.Table<WorkoutLog>().CountAsync();
            if (count > 0)
            {
                return;
            }

            var workoutLogs = new List<WorkoutLog>
            {
                // Week
                CreateWorkoutLog(1, 0, 45),
                CreateWorkoutLog(2, 1, 30),
                CreateWorkoutLog(3, 2, 60),
                CreateWorkoutLog(1, 3, 40),
                CreateWorkoutLog(2, 5, 35),

                // Month
                CreateWorkoutLog(1, 8, 45),
                CreateWorkoutLog(3, 10, 30),
                CreateWorkoutLog(2, 13, 60),
                CreateWorkoutLog(1, 15, 40),
                CreateWorkoutLog(2, 20, 50),
                CreateWorkoutLog(3, 24, 35),
                CreateWorkoutLog(1, 27, 45),

                // Year
                CreateWorkoutLog(1, 40, 50),
                CreateWorkoutLog(2, 70, 45),
                CreateWorkoutLog(3, 100, 60),
                CreateWorkoutLog(1, 130, 30),
                CreateWorkoutLog(2, 160, 55),
                CreateWorkoutLog(3, 200, 40),
                CreateWorkoutLog(1, 250, 70),
                CreateWorkoutLog(2, 300, 45)
            };

            await db.InsertAllAsync(workoutLogs);
            

        }

        private static WorkoutLog CreateWorkoutLog(int workoutId, int daysAgo, int minutes)
        {
            return new WorkoutLog
            {
                WorkoutId = workoutId,
                UserId = 1,
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

            var weightLogs = new List<WeightLog>
            {
                // Week
                CreateWeightLog(0, 80.5),
                CreateWeightLog(1, 80.2),
                CreateWeightLog(2, 80.0),
                CreateWeightLog(3, 79.8),
                CreateWeightLog(5, 79.5),

                // Month
                CreateWeightLog(8, 79.3),
                CreateWeightLog(10, 79.0),
                CreateWeightLog(13, 78.8),
                CreateWeightLog(15, 78.5),
                CreateWeightLog(20, 78.3),
                CreateWeightLog(24, 78.0),
                CreateWeightLog(27, 77.8),

                // Year
                CreateWeightLog(40, 77.5),
                CreateWeightLog(70, 77.3),
                CreateWeightLog(100, 77.0),
                CreateWeightLog(130, 76.8),
                CreateWeightLog(160, 76.5),
                CreateWeightLog(200, 76.3),
                CreateWeightLog(250, 76.0),
                CreateWeightLog(300, 75.8)
            };

            await db.InsertAllAsync(weightLogs);
        }

        private static WeightLog CreateWeightLog(int daysAgo, double weightKg)
        {
            return new WeightLog
            {
                UserId = 1,
                DateTimeLogged = DateTime.Today.AddDays(-daysAgo).AddHours(7),
                WeightKg = weightKg
            };
        }
    }
}