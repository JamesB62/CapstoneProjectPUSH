namespace CapstoneProjectPUSH.Services
{
    public class WorkoutService
    {
        private readonly CategoryService _categoryService;
        private readonly DatabaseService _databaseService;
        private readonly UserService _userService;

        public WorkoutService(CategoryService categoryService, DatabaseService databaseService, UserService userService)
        {
            _categoryService = categoryService;
            _databaseService = databaseService;
            _userService = userService;
        }

        // Get a user's workouts
        public async Task<List<Workout>> GetWorkoutsForUserAsync(int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return new List<Workout>();
            }

            var db = await _databaseService.GetConnectionAsync();

            var workouts = await db.Table<Workout>()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.DateTimeCreated)
                .ToListAsync();

            if (workouts.Count == 0)
            {
                return workouts;
            }

            // Load every exercise for these workouts in one query, then group them by workout
            var workoutIds = workouts.Select(w => w.Id).ToList();

            var workoutExercises = await db.Table<WorkoutExercise>()
                .Where(we => workoutIds.Contains(we.WorkoutId))
                .ToListAsync();

            var exercisesByWorkout = workoutExercises.ToLookup(we => we.WorkoutId);
            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();
            var favouriteIds = await GetFavouriteWorkoutIdsAsync(userId);

            foreach (var workout in workouts)
            {
                var exercises = exercisesByWorkout[workout.Id];

                workout.CategoryTitle = categoryTitles.GetValueOrDefault(workout.CategoryId, "Uncategorised");
                workout.ExerciseCount = exercises.Count();
                workout.SetCount = exercises.Sum(e => e.Sets);
                workout.TotalMinutes = exercises.Sum(e => e.DurationMinutes);
                workout.IsFavourite = favouriteIds.Contains(workout.Id);
            }

            return workouts
                .OrderByDescending(w => w.IsFavourite)
                .ThenByDescending(w => w.DateTimeCreated)
                .ToList();
        }

        // Get one of the user's own workouts
        public async Task<Workout?> GetWorkoutByIdAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return null;
            }

            var db = await _databaseService.GetConnectionAsync();

            var workout = await db.Table<Workout>()
                .Where(w => w.Id == workoutId && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (workout is null)
            {
                return null;
            }

            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();
            workout.CategoryTitle = categoryTitles.GetValueOrDefault(workout.CategoryId, "Uncategorised");

            return workout;
        }

        // Gets every shared workout
        public async Task<List<Workout>> GetPublicWorkoutsAsync(int userId)
        {
            // Guests can't see community workouts
            if (!_userService.IsCurrentMember(userId))
            {
                return new List<Workout>();
            }

            var db = await _databaseService.GetConnectionAsync();

            var workouts = await db.Table<Workout>()
                .Where(w => w.IsPrivate == false)
                .OrderByDescending(w => w.DateTimeCreated)
                .ToListAsync();

            if (workouts.Count == 0)
            {
                return workouts;
            }

            var workoutIds = workouts.Select(w => w.Id).ToList();

            var workoutExercises = await db.Table<WorkoutExercise>()
                .Where(we => workoutIds.Contains(we.WorkoutId))
                .ToListAsync();

            var exercisesByWorkout = workoutExercises.ToLookup(we => we.WorkoutId);
            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();
            var displayNames = await _userService.GetDisplayNamesByIdAsync();
            var favouriteIds = await GetFavouriteWorkoutIdsAsync(userId);

            foreach (var workout in workouts)
            {
                var exercises = exercisesByWorkout[workout.Id];

                workout.CategoryTitle = categoryTitles.GetValueOrDefault(workout.CategoryId, "Uncategorised");
                workout.UserDisplayName = displayNames.GetValueOrDefault(workout.UserId, "Unknown");
                workout.ExerciseCount = exercises.Count();
                workout.SetCount = exercises.Sum(e => e.Sets);
                workout.TotalMinutes = exercises.Sum(e => e.DurationMinutes);
                workout.IsFavourite = favouriteIds.Contains(workout.Id);
            }

            return workouts.OrderByDescending(w => w.IsFavourite).ToList();
        }

        // Get one shared workout by Id
        public async Task<Workout?> GetPublicWorkoutByIdAsync(int workoutId)
        {
            if (_userService.GetCurrentMember() is null)
            {
                return null;
            }

            var db = await _databaseService.GetConnectionAsync();

            var workout = await db.Table<Workout>()
                .Where(w => w.Id == workoutId && w.IsPrivate == false)
                .FirstOrDefaultAsync();

            if (workout is null)
            {
                return null;
            }

            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();
            workout.CategoryTitle = categoryTitles.GetValueOrDefault(workout.CategoryId, "Uncategorised");

            var displayNames = await _userService.GetDisplayNamesByIdAsync();
            workout.UserDisplayName = displayNames.GetValueOrDefault(workout.UserId, "Unknown");

            return workout;
        }

        // Get the Ids of the workouts favourited by user Id
        public async Task<HashSet<int>> GetFavouriteWorkoutIdsAsync(int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return new HashSet<int>();
            }

            var db = await _databaseService.GetConnectionAsync();

            var favourites = await db.Table<WorkoutFavourite>()
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return favourites.Select(f => f.WorkoutId).ToHashSet();
        }

        // Check if a workout is a favourite of user Id
        public async Task<bool> IsFavouriteAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return false;
            }

            var db = await _databaseService.GetConnectionAsync();

            var favourite = await db.Table<WorkoutFavourite>()
                .Where(f => f.WorkoutId == workoutId && f.UserId == userId)
                .FirstOrDefaultAsync();

            return favourite is not null;
        }

        // Add the workout to the user's favourites, or remove it if it is already there
        public async Task<(bool Success, bool IsFavourite, string Message)> ToggleFavouriteAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return (false, false, "Register an account to favourite workouts.");
            }

            var db = await _databaseService.GetConnectionAsync();

            // Users can favourite their own workouts and shared workouts
            var workout = await db.Table<Workout>()
                .Where(w => w.Id == workoutId && (w.UserId == userId || w.IsPrivate == false))
                .FirstOrDefaultAsync();

            if (workout is null)
            {
                return (false, false, "Workout could not be found.");
            }

            var existing = await db.Table<WorkoutFavourite>()
                .Where(f => f.WorkoutId == workoutId && f.UserId == userId)
                .FirstOrDefaultAsync();

            if (existing is not null)
            {
                await db.DeleteAsync(existing);

                return (true, false, $"{workout.Title} removed from your favourites.");
            }

            await db.InsertAsync(new WorkoutFavourite { UserId = userId, WorkoutId = workoutId });

            return (true, true, $"{workout.Title} added to your favourites.");
        }

        // Update a workout and replace its exercises (all or nothing)
        public async Task<bool> UpdateWorkoutWithExercisesAsync(Workout workout, int userId, IEnumerable<WorkoutExercise> items)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return false;
            }

            var db = await _databaseService.GetConnectionAsync();

            var existing = await db.Table<Workout>()
                .Where(w => w.Id == workout.Id && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (existing is null)
            {
                return false;
            }

            // Copy the editable details across
            existing.Title = workout.Title;
            existing.CategoryId = workout.CategoryId;
            existing.IsPrivate = workout.IsPrivate;

            foreach (var item in items)
            {
                item.WorkoutId = existing.Id;
            }

            await db.RunInTransactionAsync(conn =>
            {
                conn.Update(existing);
                conn.Table<WorkoutExercise>().Delete(we => we.WorkoutId == existing.Id);
                conn.InsertAll(items);
            });

            return true;
        }

        // Gets a workout's exercises
        public async Task<List<WorkoutExercise>> GetExercisesForWorkoutAsync(int workoutId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var workout = await db.Table<Workout>().Where(w => w.Id == workoutId).FirstOrDefaultAsync();

            if (workout is null || !CanViewWorkout(workout))
            {
                return new List<WorkoutExercise>();
            }

            var items = await db.Table<WorkoutExercise>()
                .Where(we => we.WorkoutId == workoutId)
                .OrderBy(we => we.SortOrder)
                .ToListAsync();

            var exercises = await db.Table<Exercise>().ToListAsync();
            var titles = exercises.ToDictionary(e => e.Id, e => e.Title);

            foreach (var item in items)
            {
                item.ExerciseTitle = titles.GetValueOrDefault(item.ExerciseId, "Unknown exercise");
            }

            return items;
        }

        // Delete a workout
        public async Task DeleteWorkoutAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return;
            }

            var db = await _databaseService.GetConnectionAsync();

            var workout = await db.Table<Workout>()
                .Where(w => w.Id == workoutId && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (workout is null)
            {
                return;
            }

            await db.RunInTransactionAsync(conn =>
            {
                conn.Table<WorkoutExercise>().Delete(we => we.WorkoutId == workoutId);
                conn.Table<WorkoutFavourite>().Delete(f => f.WorkoutId == workoutId);
                conn.Table<Workout>().Delete(w => w.Id == workoutId && w.UserId == userId);
            });
        }

        // Add workout with exercises
        public async Task<int> AddWorkoutWithExercisesAsync(Workout workout, IEnumerable<WorkoutExercise> items)
        {
            if (!_userService.IsCurrentMember(workout.UserId))
            {
                return 0;
            }

            var db = await _databaseService.GetConnectionAsync();

            await db.RunInTransactionAsync(conn =>
            {
                conn.Insert(workout);

                foreach (var item in items)
                {
                    item.WorkoutId = workout.Id;
                    conn.Insert(item);
                }
            });

            return workout.Id;
        }

        // Helpers
        private bool CanViewWorkout(Workout workout)
        {
            if (_userService.IsCurrentUserAdmin())
            {
                return true;
            }

            if (_userService.IsCurrentUser(workout.UserId))
            {
                return true;
            }

            if (!workout.IsPrivate && _userService.GetCurrentMember() is not null)
            {
                return true;
            }

            return false;
        }
    }
}