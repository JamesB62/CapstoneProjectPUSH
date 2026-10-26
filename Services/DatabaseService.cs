using static Android.Provider.SyncStateContract;

namespace CapstoneProjectPUSH.Services
{
    public class DatabaseService
    {
        public const string DbFileName = "push.db3";
        public static string DbPath => Path.Combine(FileSystem.AppDataDirectory, DbFileName);

        private SQLiteAsyncConnection? _connection;

        private readonly SemaphoreSlim _initLock = new(1, 1);

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_connection is not null)
            {
                return _connection;
            }

            await _initLock.WaitAsync();

            try
            {
                if (_connection is null)
                {
                    var connection = new SQLiteAsyncConnection(DbPath);

                    // Add future tables here
                    await connection.CreateTableAsync<User>();

                    _connection = connection;
                }
            }
            finally
            {
                _initLock.Release();
            }

            return _connection;
        }


    }
}