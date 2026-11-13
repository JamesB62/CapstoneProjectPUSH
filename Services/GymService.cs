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

            return await db.Table<Gym>()
                .OrderBy(g => g.Name)
                .ThenBy(g => g.City)
                .ToListAsync();
        }

        // Get gym by Id
        public async Task<Gym?> GetGymByIdAsync(int gymId)
        {
            var db = await _databaseService.GetConnectionAsync();

            return await db.Table<Gym>()
                .Where(g => g.Id == gymId)
                .FirstOrDefaultAsync();
        }

        // Get all gyms for user Id, with the user's linked gyms first (every gym is still included)
        public async Task<List<Gym>> GetGymsForUserAsync(int userId)
        {
            var gyms = await GetGymsAsync();
            var linkedGymIds = await GetLinkedGymIdsAsync(userId);

            foreach (var gym in gyms)
            {
                gym.IsLinked = linkedGymIds.Contains(gym.Id);
            }

            // OrderBy is stable, so each group keeps the Name / City order from GetGymsAsync
            return gyms.OrderByDescending(g => g.IsLinked).ToList();
        }

        // Get Ids of the gyms linked to user Id
        public async Task<HashSet<int>> GetLinkedGymIdsAsync(int userId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var links = await db.Table<UserGym>()
                .Where(ug => ug.UserId == userId)
                .ToListAsync();

            return links.Select(ug => ug.GymId).ToHashSet();
        }

        // Link gym to user Id
        public async Task<(bool Success, string Message)> LinkGymAsync(int userId, int gymId)
        {
            var db = await _databaseService.GetConnectionAsync();

            var gym = await GetGymByIdAsync(gymId);

            if (gym is null)
            {
                return (false, "Gym could not be found.");
            }

            var existingLink = await db.Table<UserGym>()
                .Where(ug => ug.UserId == userId && ug.GymId == gymId)
                .FirstOrDefaultAsync();

            if (existingLink is not null)
            {
                return (true, $"{gym.Name} is already linked to your account.");
            }

            await db.InsertAsync(new UserGym { UserId = userId, GymId = gymId });

            return (true, $"{gym.Name} linked to your account.");
        }

        // Unlink gym from user Id
        public async Task<(bool Success, string Message)> UnlinkGymAsync(int userId, int gymId)
        {
            var db = await _databaseService.GetConnectionAsync();

            await db.Table<UserGym>().DeleteAsync(ug => ug.UserId == userId && ug.GymId == gymId);

            return (true, "Gym unlinked from your account.");
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

            // Remove any user links to this gym
            await db.Table<UserGym>().DeleteAsync(ug => ug.GymId == gymId);

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