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

        public async Task<List<Workout>> GetWorkoutsForUserAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var workouts = await db.Table<Workout>()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.DateTimeCreated)
                .ToListAsync();

            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();

            foreach (var workout in workouts)
            {
                workout.CategoryTitle = categoryTitles.GetValueOrDefault(workout.CategoryId, "Uncategorised");
            }

            return workouts;
        }

        public async Task<Workout?> GetWorkoutByIdAsync(int workoutId, int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var workout = await db.Table<Workout>()
                .Where(w => w.Id == workoutId && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (workout is null)
            {
                return null;
            }

            var categoryTitle = await _categoryService.GetCategoryTitlesAsync();
            workout.CategoryTitle = categoryTitle.GetValueOrDefault(workout.CategoryId, "Uncategorised");

            return workout;
        }

        // TEST ==================================================================================
        public async Task<List<Workout>> GetPublicWorkoutsAsync()
        {
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

            foreach (var workout in workouts)
            {
                var exercises = exercisesByWorkout[workout.Id];

                workout.CategoryTitle = categoryTitles.GetValueOrDefault(workout.CategoryId, "Uncategorised");
                workout.UserDisplayName = displayNames.GetValueOrDefault(workout.UserId, "Unknown");
                workout.ExerciseCount = exercises.Count();
                workout.SetCount = exercises.Sum(e => e.Sets);
                workout.TotalMinutes = exercises.Sum(e => e.DurationMinutes);
            }

            return workouts;
        }

        public async Task<Workout?> GetPublicWorkoutByIdAsync(int workoutId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var workout = await db.Table<Workout>()
                .Where(w => w.Id == workoutId && w.IsPrivate == false)
                .FirstOrDefaultAsync();

            if (workout is null)
            {
                return null;
            }

            var categoryTitle = await _categoryService.GetCategoryTitlesAsync();
            workout.CategoryTitle = categoryTitle.GetValueOrDefault(workout.CategoryId, "Uncategorised");

            var displayNames = await _userService.GetDisplayNamesByIdAsync();
            workout.UserDisplayName = displayNames.GetValueOrDefault(workout.UserId, "Unknown");

            return workout;
        }

        public async Task<bool> UpdateWorkoutWithExercisesAsync(Workout workout, int userId, IEnumerable<WorkoutExercise> items)
        {
            var db = await _databaseService.GetConnectionAsync();

            var existing = await db.Table<Workout>()
                .Where(w => w.Id == workout.Id && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (existing is null)
            {
                return false;
            }

            existing.Title = workout.Title;
            existing.CategoryId = workout.CategoryId;
            existing.IsPrivate = workout.IsPrivate;

            await db.UpdateAsync(existing);

            await db.Table<WorkoutExercise>().DeleteAsync(we => we.WorkoutId == workout.Id);

            foreach (var item in items)
            {
                item.WorkoutId = workout.Id;
            }

            await db.InsertAllAsync(items);

            return true;
        }

        // TEST ==================================================================================

        //workout exercisese 
        public async Task<List<WorkoutExercise>> GetExercisesForWorkoutAsync(int workoutId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var items = await db.Table<WorkoutExercise>()
                .Where(we => we.WorkoutId == workoutId)
                .OrderBy(we => we.SortOrder)
                .ToListAsync();

                var exercises = await db.Table<Exercise>().ToListAsync();
                var titles = exercises.ToDictionary(e => e.Id, e => e.Title);

                foreach(var item in items)
                {
                    item.ExerciseTitle = titles.GetValueOrDefault(item.ExerciseId, "Unknown exercise");

                }
                return items;
        }

        // Add other functions as needed, CRUD
        //------------------------------------//

        // Add
        public async Task<int> AddWorkoutAsync(Workout workout)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.InsertAsync(workout);
        }

        // Delete
        public async Task DeleteWorkoutAsync(int workoutId, int userId)
        {
            var db = await _databaseService.GetConnectionAsync();
            await db.Table<WorkoutExercise>().DeleteAsync(we => we.WorkoutId == workoutId);
            await db.Table<Workout>().DeleteAsync(w => w.Id == workoutId && w.UserId == userId);
        }
        // Update
        public async Task<int> UpdateWorkoutAsync(Workout workout)
        {
            var db = await _databaseService.GetConnectionAsync();
            return await db.UpdateAsync(workout);
        }

        //add workout with exercises
        public async Task<int> AddWorkoutWithExercisesAsync(Workout workout, IEnumerable<WorkoutExercise> items)
        {
            var db = await _databaseService.GetConnectionAsync();

            await db.RunInTransactionAsync(conn =>
            {
                conn.Insert(workout);

                foreach(var item in items)
                {
                    item.WorkoutId = workout.Id;
                    conn.Insert(item);
                }
            });

            return workout.Id;
        }
    }
}
