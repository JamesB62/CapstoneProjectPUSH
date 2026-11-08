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

            return await db.Table<Category>()
                .OrderBy(c => c.Title)
                .ToListAsync();
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
    }
}