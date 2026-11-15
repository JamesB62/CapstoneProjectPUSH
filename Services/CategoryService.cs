namespace CapstoneProjectPUSH.Services
{
    public class CategoryService
    {
        private readonly DatabaseService _databaseService;

        public CategoryService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get category count
        public async Task<int> GetCategoryCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Category>().CountAsync();
        }

        // Get all categories
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var categories = await db.Table<Category>().ToListAsync();

            return categories.OrderBy(c => c.Title, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // Get category by Id
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Category>()
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();
        }

        // Get category titles
        public async Task<Dictionary<int, string>> GetCategoryTitlesAsync()
        {
            var categories = await GetCategoriesAsync();
            return categories.ToDictionary(c => c.Id, c => c.Title);
        }

        // Add category
        public async Task<(bool Success, string Message)> AddCategoryAsync(string title)
        {
            title = (title ?? string.Empty).Trim();

            if (title.Length < 2 || title.Length > 50)
            {
                return (false, "Category title must be between 2 and 50 characters.");
            }

            var db = await _databaseService.GetConnectionAsync();
            var categories = await GetCategoriesAsync();

            // Case-insensitive duplicate check (the Unique constraint is case-sensitive)
            if (categories.Any(c => string.Equals(c.Title, title, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "A category with that title already exists.");
            }

            await db.InsertAsync(new Category { Title = title });

            return (true, "Category added.");
        }

        // Update category
        public async Task<(bool Success, string Message)> UpdateCategoryAsync(int categoryId, string title)
        {
            title = (title ?? string.Empty).Trim();

            if (title.Length < 2 || title.Length > 50)
            {
                return (false, "Category title must be between 2 and 50 characters.");
            }

            var db = await _databaseService.GetConnectionAsync();

            var category = await GetCategoryByIdAsync(categoryId);

            if (category is null)
            {
                return (false, "Category could not be found.");
            }

            var categories = await GetCategoriesAsync();

            if (categories.Any(c => c.Id != categoryId && string.Equals(c.Title, title, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "A category with that title already exists.");
            }

            category.Title = title;

            await db.UpdateAsync(category);

            return (true, "Category updated.");
        }

        // Delete category
        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int categoryId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var category = await GetCategoryByIdAsync(categoryId);

            if (category is null)
            {
                return (false, "Category could not be found.");
            }

            var exercises = await db.Table<Exercise>().Where(e => e.CategoryId == categoryId).ToListAsync();

            foreach (var exercise in exercises)
            {
                exercise.CategoryId = 0;
                await db.UpdateAsync(exercise);
            }

            var trainers = await db.Table<Trainer>().Where(t => t.CategoryId == categoryId).ToListAsync();

            foreach (var trainer in trainers)
            {
                trainer.CategoryId = 0;
                await db.UpdateAsync(trainer);
            }

            var workouts = await db.Table<Workout>().Where(w => w.CategoryId == categoryId).ToListAsync();

            foreach (var workout in workouts)
            {
                workout.CategoryId = 0;
                await db.UpdateAsync(workout);
            }

            await db.DeleteAsync(category);

            return (true, "Category deleted.");
        }
    }
}