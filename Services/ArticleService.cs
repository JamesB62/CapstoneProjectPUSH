namespace CapstoneProjectPUSH.Services
{
    public class ArticleService
    {
        private readonly CategoryService _categoryService;
        private readonly DatabaseService _databaseService;

        public ArticleService(CategoryService categoryService, DatabaseService databaseService)
        {
            _categoryService = categoryService;
            _databaseService = databaseService;
        }

        // Get article count
        public async Task<int> GetArticleCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Article>().CountAsync();
        }

        // Get all articles
        public async Task<List<Article>> GetArticlesAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var articles = await db.Table<Article>()
                .OrderByDescending(a => a.DateTimePublished)
                .ToListAsync();

            var categoryTitles = await _categoryService.GetCategoryTitlesAsync();

            foreach (var article in articles)
            {
                article.CategoryTitle = categoryTitles.GetValueOrDefault(article.CategoryId, "Uncategorised");
            }

            return articles;
        }

        // Get article by Id
        public async Task<Article?> GetArticleByIdAsync(int articleId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var article = await db.Table<Article>()
                .Where(a => a.Id == articleId)
                .FirstOrDefaultAsync();

            if (article is null)
            {
                return null;
            }

            var categoryTitle = await _categoryService.GetCategoryTitlesAsync();

            article.CategoryTitle = categoryTitle.GetValueOrDefault(article.CategoryId, "Uncategorised");

            return article;
        }
    }
}