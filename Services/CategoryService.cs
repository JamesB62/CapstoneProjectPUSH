namespace CapstoneProjectPUSH.Services
{
    public class CategoryService
    {
        private readonly DatabaseService _databaseService;

        public CategoryService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Category>()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Category>()
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();
        }

        // Add other functions as needed, CRUD?

        // Helpers
        public async Task<Dictionary<int, string>> GetCategoryNamesAsync()
        {
            var categories = await GetCategoriesAsync();
            return categories.ToDictionary(c => c.Id, c => c.Name);
        }
    }
}