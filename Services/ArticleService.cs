namespace CapstoneProjectPUSH.Services
{
    public class ArticleService
    {
        private readonly DatabaseService _databaseService;

        public ArticleService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get article count -- add to admin dashboard?
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

            return articles;
        }

        // Get article by Id
        public async Task<Article?> GetArticleByIdAsync(int articleId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Article>()
                .Where(a => a.Id == articleId)
                .FirstOrDefaultAsync();
        }
    }
}