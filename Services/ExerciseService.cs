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

            var exercises = await db.Table<Exercise>()
                .OrderBy(e => e.Title)
                .ToListAsync();

            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();

            foreach (var exercise in exercises)
            {
                exercise.CategoryTitle = categoryTitles.GetValueOrDefault(exercise.CategoryId, "Uncategorised");
            }

            return exercises;
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
    }
}