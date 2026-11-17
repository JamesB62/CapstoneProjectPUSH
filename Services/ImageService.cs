namespace CapstoneProjectPUSH.Services
{
    public class ImageService
    {
        private static readonly string ImageFolder = Path.Combine(FileSystem.AppDataDirectory, "Images");

        private readonly UserService _userService;

        public ImageService(UserService userService)
        {
            _userService = userService;
        }

        public async Task<string?> PickImageAsync()
        {
            // Guests and signed-out users can't upload photos
            if (_userService.GetCurrentMember() is null)
            {
                return null;
            }

            var results = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Choose a photo",
                SelectionLimit = 1,
                MaximumWidth = 1024,
                MaximumHeight = 1024
            });

            if (results is null || results.Count == 0)
            {
                return null;
            }

            var photo = results[0];

            Directory.CreateDirectory(ImageFolder);

            string extension = Path.GetExtension(photo.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            string newPath = Path.Combine(ImageFolder, $"{Guid.NewGuid()}{extension}");

            using (var source = await photo.OpenReadAsync())
            using (var destination = File.Create(newPath))
            {
                await source.CopyToAsync(destination);
            }

            return newPath;
        }

        public static bool IsAppImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return false;
            }

            string fullPath = Path.GetFullPath(imagePath);
            string folder = Path.GetFullPath(ImageFolder) + Path.DirectorySeparatorChar;

            return fullPath.StartsWith(folder, StringComparison.OrdinalIgnoreCase);
        }

        public static void DeleteImage(string? imagePath)
        {
            if (!IsAppImage(imagePath))
            {
                return;
            }

            try
            {
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}