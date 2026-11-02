namespace CapstoneProjectPUSH.Services
{
    public class ExerciseService
    {
        private readonly DatabaseService _databaseService;
        private readonly CategoryService _categoryService;

        public ExerciseService(DatabaseService databaseService, CategoryService categoryService)
        {
            _databaseService = databaseService;
            _categoryService = categoryService;
        }

        public async Task<List<Exercise>> GetExercisesAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var exercises = await db.Table<Exercise>()
                .OrderBy(e => e.Title)
                .ToListAsync();


            var categoryNames = await _categoryService.GetCategoryNamesAsync();

            foreach (var exercise in exercises)
            {
                exercise.CategoryName = categoryNames.GetValueOrDefault(exercise.CategoryId, "Uncategorised");
            }

            return exercises;
        }

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

            var categoryNames = await _categoryService.GetCategoryNamesAsync();
            exercise.CategoryName = categoryNames.GetValueOrDefault(exercise.CategoryId, "Uncategorised");

            return exercise;
        }

        // Add other functions as needed, CRUD?
    }
}