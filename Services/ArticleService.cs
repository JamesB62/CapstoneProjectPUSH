namespace CapstoneProjectPUSH.Services
{
    public class ArticleService
    {
        private readonly DatabaseService _databaseService;
        private readonly CategoryService _categoryService;

        public ArticleService(DatabaseService databaseService, CategoryService categoryService)
        {
            _databaseService = databaseService;
            _categoryService = categoryService;
        }

        public async Task<List<Article>> GetArticlesAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var articles = await db.Table<Article>()
                .OrderBy(a => a.DateTimeAdded)
                .ToListAsync();

            var categoryNames = await _categoryService.GetCategoryNamesAsync();

            foreach (var article in articles)
            {
                article.CategoryName = categoryNames.GetValueOrDefault(article.CategoryId, "Uncategorised");
            }

            return articles;
        }

        public async Task<Article?> GetArticleByIdAsync(int articleId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var article = await db.Table<Article>()
                .Where(e => e.Id == articleId)
                .FirstOrDefaultAsync();

            if (article is null)
            {
                return null;
            }

            var categoryNames = await _categoryService.GetCategoryNamesAsync();
            article.CategoryName = categoryNames.GetValueOrDefault(article.CategoryId, "Uncategorised");

            return article;
        }

        // Add other functions as needed, CRUD?
    }
}