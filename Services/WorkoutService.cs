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

            var categoryNames = await _categoryService.GetCategoryNamesAsync();

            foreach (var workout in workouts)
            {
                workout.CategoryName = categoryNames.GetValueOrDefault(workout.CategoryId, "Uncategorised");
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

            var categoryNames = await _categoryService.GetCategoryNamesAsync();
            workout.CategoryName = categoryNames.GetValueOrDefault(workout.CategoryId, "Uncategorised");

            return workout;
        }

        // Add other functions as needed, CRUD?
    }
}