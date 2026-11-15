namespace CapstoneProjectPUSH.Services
{
    public class ExerciseService
    {
        private readonly CategoryService _categoryService;
        private readonly DatabaseService _databaseService;

        public ExerciseService(CategoryService categoryService, DatabaseService databaseService)
        {
            _categoryService = categoryService;
            _databaseService = databaseService;
        }

        // Get exercise count
        public async Task<int> GetExerciseCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Exercise>().CountAsync();
        }

        // Get all exercises
        public async Task<List<Exercise>> GetExercisesAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var exercises = await db.Table<Exercise>().ToListAsync();

            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();

            foreach (var exercise in exercises)
            {
                exercise.CategoryTitle = categoryTitles.GetValueOrDefault(exercise.CategoryId, "Uncategorised");
            }

            return exercises.OrderBy(e => e.Title, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // Get exercise by Id
        public async Task<Exercise?> GetExerciseByIdAsync(int exerciseId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var exercise = await db.Table<Exercise>()
                .Where(e => e.Id == exerciseId)
                .FirstOrDefaultAsync();

            if (exercise is null)
            {
                return null;
            }

            var categoryTitle = await _categoryService.GetCategoryTitlesAsync();

            exercise.CategoryTitle = categoryTitle.GetValueOrDefault(exercise.CategoryId, "Uncategorised");

            return exercise;
        }

        // Add an exercise
        public async Task<(bool Success, string Message)> AddExerciseAsync(Exercise exercise)
        {
            var error = await NormaliseAndValidateAsync(exercise, null);

            if (error is not null)
            {
                return (false, error);
            }

            var db = await _databaseService.GetConnectionAsync();

            await db.InsertAsync(exercise);

            return (true, $"{exercise.Title} was added.");
        }

        // Update an exercise
        public async Task<(bool Success, string Message)> UpdateExerciseAsync(Exercise exercise)
        {
            var db = await _databaseService.GetConnectionAsync();

            var existing = await db.Table<Exercise>()
                .Where(e => e.Id == exercise.Id)
                .FirstOrDefaultAsync();

            if (existing is null)
            {
                return (false, "Exercise could not be found.");
            }

            var error = await NormaliseAndValidateAsync(exercise, exercise.Id);

            if (error is not null)
            {
                return (false, error);
            }

            existing.Title = exercise.Title;
            existing.Description = exercise.Description;
            existing.IsTimed = exercise.IsTimed;
            existing.Difficulty = exercise.Difficulty;
            existing.CategoryId = exercise.CategoryId;

            await db.UpdateAsync(existing);

            return (true, $"{existing.Title} was updated.");
        }

        // Delete an exercise
        public async Task<(bool Success, string Message)> DeleteExerciseAsync(int exerciseId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var exercise = await db.Table<Exercise>()
                .Where(e => e.Id == exerciseId)
                .FirstOrDefaultAsync();

            if (exercise is null)
            {
                return (false, "Exercise could not be found.");
            }

            // Remove it from workouts first
            var links = await db.Table<WorkoutExercise>()
                .Where(we => we.ExerciseId == exerciseId)
                .ToListAsync();

            foreach (var link in links)
            {
                await db.DeleteAsync(link);
            }

            await db.DeleteAsync(exercise);

            return (true, "Exercise deleted.");
        }

        // Helpers
        private async Task<string?> NormaliseAndValidateAsync(Exercise exercise, int? existingId)
        {
            exercise.Title = (exercise.Title ?? string.Empty).Trim();
            exercise.Description = (exercise.Description ?? string.Empty).Trim();

            if (exercise.Title.Length < 2 || exercise.Title.Length > 100)
            {
                return "Title must be between 2 and 100 characters.";
            }

            if (exercise.Description.Length == 0 || exercise.Description.Length > 1000)
            {
                return "Description is required (1000 characters max).";
            }

            if (exercise.Difficulty < 1 || exercise.Difficulty > 5)
            {
                return "Please choose a difficulty.";
            }

            var db = await _databaseService.GetConnectionAsync();

            var categoryExists = await db.Table<Category>()
                .Where(c => c.Id == exercise.CategoryId)
                .CountAsync() > 0;

            if (!categoryExists)
            {
                return "Please choose a category for this exercise.";
            }

            var exercises = await db.Table<Exercise>().ToListAsync();

            bool titleTaken = exercises.Any(e => e.Id != existingId
                && string.Equals(e.Title, exercise.Title, StringComparison.OrdinalIgnoreCase));

            if (titleTaken)
            {
                return $"An exercise called \"{exercise.Title}\" already exists.";
            }

            return null;
        }
    }
}