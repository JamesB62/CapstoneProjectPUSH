namespace CapstoneProjectPUSH.Services
{
    public class WorkoutService
    {
        private readonly ApiClient _apiClient;
        private readonly UserService _userService;

        public WorkoutService(ApiClient apiClient, UserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;
        }

        // Get a user's workouts
        public async Task<List<Workout>> GetWorkoutsForUserAsync(int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return new List<Workout>();
            }

            ApiResult<List<Workout>> result = await _apiClient.GetAsync<List<Workout>>("api/workouts/mine");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Workout>();
        }

        // Get one of the user's own workouts
        public async Task<Workout?> GetWorkoutByIdAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return null;
            }

            ApiResult<Workout> result = await _apiClient.GetAsync<Workout>($"api/workouts/mine/{workoutId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Gets every shared workout
        public async Task<List<Workout>> GetPublicWorkoutsAsync(int userId)
        {
            // Guests can't see community workouts
            if (!_userService.IsCurrentMember(userId))
            {
                return new List<Workout>();
            }

            ApiResult<List<Workout>> result = await _apiClient.GetAsync<List<Workout>>("api/workouts/community");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<Workout>();
        }

        // Get one shared workout by Id
        public async Task<Workout?> GetPublicWorkoutByIdAsync(int workoutId)
        {
            if (_userService.GetCurrentMember() is null)
            {
                return null;
            }

            ApiResult<Workout> result = await _apiClient.GetAsync<Workout>($"api/workouts/community/{workoutId}");

            if (result.Success)
            {
                return result.Data;
            }

            return null;
        }

        // Get the Ids of the workouts favourited by user Id
        public async Task<HashSet<int>> GetFavouriteWorkoutIdsAsync(int userId)
        {
            HashSet<int> ids = new HashSet<int>();

            if (!_userService.IsCurrentUser(userId))
            {
                return ids;
            }

            ApiResult<List<int>> result = await _apiClient.GetAsync<List<int>>("api/workouts/favourite-ids");

            if (result.Success && result.Data != null)
            {
                foreach (int id in result.Data)
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

        // Check if a workout is a favourite of user Id
        public async Task<bool> IsFavouriteAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentUser(userId))
            {
                return false;
            }

            ApiResult<bool> result = await _apiClient.GetAsync<bool>($"api/workouts/{workoutId}/favourite");

            if (result.Success)
            {
                return result.Data;
            }

            return false;
        }

        // Add the workout to the user's favourites, or remove it if it is already there
        public async Task<(bool Success, bool IsFavourite, string Message)> ToggleFavouriteAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return (false, false, "Register an account to favourite workouts.");
            }

            ApiResult<FavouriteResponse> result = await _apiClient.PostAsync<FavouriteResponse>($"api/workouts/{workoutId}/favourite", null);

            if (!result.Success)
            {
                return (false, false, result.Message);
            }

            if (result.Data is null)
            {
                return (false, false, "Something went wrong. Please try again.");
            }

            return (true, result.Data.IsFavourite, result.Data.Message);
        }

        // Gets a workout's exercises, in order
        public async Task<List<WorkoutExercise>> GetExercisesForWorkoutAsync(int workoutId)
        {
            ApiResult<List<WorkoutExercise>> result = await _apiClient.GetAsync<List<WorkoutExercise>>($"api/workouts/{workoutId}/exercises");

            if (result.Success && result.Data != null)
            {
                return result.Data;
            }

            return new List<WorkoutExercise>();
        }

        // Add workout with exercises
        public async Task<int> AddWorkoutWithExercisesAsync(Workout workout, IEnumerable<WorkoutExercise> items)
        {
            if (!_userService.IsCurrentMember(workout.UserId))
            {
                return 0;
            }

            List<WorkoutExercise> itemList = items.ToList();

            // App-side validation
            if (!IsValidWorkout(workout, itemList))
            {
                return 0;
            }

            ApiResult<WorkoutCreatedResponse> result = await _apiClient.PostAsync<WorkoutCreatedResponse>("api/workouts", ToRequest(workout, itemList));

            if (result.Success && result.Data != null)
            {
                workout.Id = result.Data.WorkoutId;
                return result.Data.WorkoutId;
            }

            System.Diagnostics.Debug.WriteLine("Add workout failed: " + result.Message);
            return 0;
        }

        // Update a workout and replace its exercises
        public async Task<bool> UpdateWorkoutWithExercisesAsync(Workout workout, int userId, IEnumerable<WorkoutExercise> items)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return false;
            }

            List<WorkoutExercise> itemList = items.ToList();

            // App-side validation
            if (!IsValidWorkout(workout, itemList))
            {
                return false;
            }

            ApiResult<MessageResponse> result = await _apiClient.PutAsync<MessageResponse>($"api/workouts/{workout.Id}", ToRequest(workout, itemList));

            if (!result.Success)
            {
                System.Diagnostics.Debug.WriteLine("Update workout failed: " + result.Message);
            }

            return result.Success;
        }

        // Delete a workout
        public async Task<bool> DeleteWorkoutAsync(int workoutId, int userId)
        {
            if (!_userService.IsCurrentMember(userId))
            {
                return false;
            }

            ApiResult<MessageResponse> result = await _apiClient.DeleteAsync<MessageResponse>($"api/workouts/{workoutId}");

            if (!result.Success)
            {
                System.Diagnostics.Debug.WriteLine("Delete workout failed: " + result.Message);
            }

            return result.Success;
        }

        // Helpers
        // Input validation
        private static bool IsValidWorkout(Workout workout, List<WorkoutExercise> items)
        {
            string title = string.Empty;

            if (workout.Title is not null)
            {
                title = workout.Title.Trim();
            }

            if (title.Length == 0 || title.Length > 100)
            {
                return false;
            }

            if (items.Count == 0)
            {
                return false;
            }

            return true;
        }

        // Creates workout object for requests
        private static WorkoutRequest ToRequest(Workout workout, List<WorkoutExercise> items)
        {
            WorkoutRequest request = new WorkoutRequest
            {
                Title = workout.Title.Trim(),
                CategoryId = workout.CategoryId,
                IsPrivate = workout.IsPrivate
            };

            foreach (WorkoutExercise item in items)
            {
                request.Exercises.Add(new WorkoutExerciseRequest
                {
                    ExerciseId = item.ExerciseId,
                    SortOrder = item.SortOrder,
                    Sets = item.Sets,
                    Reps = item.Reps,
                    DurationMinutes = item.DurationMinutes
                });
            }

            return request;
        }
    }
}