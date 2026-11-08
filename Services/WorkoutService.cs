namespace CapstoneProjectPUSH.Services
{
    public class WorkoutService
    {
        private readonly DatabaseService _databaseService;
        private readonly CategoryService _categoryService;

        public WorkoutService(DatabaseService databaseService, CategoryService categoryService)
        {
            _databaseService = databaseService;
            _categoryService = categoryService;
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
