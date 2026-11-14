namespace CapstoneProjectPUSH.Services
{
    public class GymService
    {
        private readonly DatabaseService _databaseService;

        public GymService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Get gym count
        public async Task<int> GetGymCountAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Gym>().CountAsync();
        }

        // Get all gyms
        public async Task<List<Gym>> GetGymsAsync()
        {
            var db = await _databaseService.GetConnectionAsync();

            var gyms = await db.Table<Gym>().ToListAsync();

            return gyms
                .OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(g => g.City, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Get gym by Id
        public async Task<Gym?> GetGymByIdAsync(int gymId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Gym>()
                .Where(g => g.Id == gymId)
                .FirstOrDefaultAsync();
        }

        // Add gym
        public async Task<(bool Success, string Message)> AddGymAsync(string name, string address, string city)
        {
            name = (name ?? string.Empty).Trim();
            address = (address ?? string.Empty).Trim();
            city = (city ?? string.Empty).Trim();

            var error = ValidateGym(name, address, city);

            if (error is not null)
            {
                return (false, error);
            }

            var db = await _databaseService.GetConnectionAsync();
            var gyms = await GetGymsAsync();

            if (gyms.Any(g => IsSameGym(g, name, address, city)))
            {
                return (false, "That gym already exists.");
            }

            await db.InsertAsync(new Gym { Name = name, Address = address, City = city });

            return (true, "Gym added.");
        }

        // Update gym
        public async Task<(bool Success, string Message)> UpdateGymAsync(int gymId, string name, string address, string city)
        {
            name = (name ?? string.Empty).Trim();
            address = (address ?? string.Empty).Trim();
            city = (city ?? string.Empty).Trim();

            var error = ValidateGym(name, address, city);

            if (error is not null)
            {
                return (false, error);
            }

            var db = await _databaseService.GetConnectionAsync();

            var gym = await GetGymByIdAsync(gymId);

            if (gym is null)
            {
                return (false, "Gym could not be found.");
            }

            var gyms = await GetGymsAsync();

            if (gyms.Any(g => g.Id != gymId && IsSameGym(g, name, address, city)))
            {
                return (false, "That gym already exists.");
            }

            gym.Name = name;
            gym.Address = address;
            gym.City = city;

            await db.UpdateAsync(gym);

            return (true, "Gym updated.");
        }

        // Delete gym
        public async Task<(bool Success, string Message)> DeleteGymAsync(int gymId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var gym = await GetGymByIdAsync(gymId);

            if (gym is null)
            {
                return (false, "Gym could not be found.");
            }

            // Trainers belong to a gym, so don't orphan them
            var trainerCount = await db.Table<Trainer>().Where(t => t.GymId == gymId).CountAsync();

            if (trainerCount > 0)
            {
                return (false, $"{gym.Name} can't be deleted because {trainerCount} trainer(s) are assigned to it.");
            }

            var users = await db.Table<User>().Where(u => u.GymId == gymId).ToListAsync();

            foreach (var user in users)
            {
                user.GymId = null;
                await db.UpdateAsync(user);
            }

            await db.DeleteAsync(gym);

            return (true, "Gym deleted.");
        }

        // Helpers
        // Validate gym details
        private static string? ValidateGym(string name, string address, string city)
        {
            if (name.Length == 0 || name.Length > 250)
            {
                return "Gym name is required (250 characters max).";
            }

            if (address.Length == 0 || address.Length > 250)
            {
                return "Address is required (250 characters max).";
            }

            if (city.Length == 0 || city.Length > 100)
            {
                return "City is required (100 characters max).";
            }

            return null;
        }

        // Compare gym details ignoring case
        private static bool IsSameGym(Gym gym, string name, string address, string city)
        {
            return string.Equals(gym.Name, name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(gym.Address, address, StringComparison.OrdinalIgnoreCase)
                && string.Equals(gym.City, city, StringComparison.OrdinalIgnoreCase);
        }
    }
}