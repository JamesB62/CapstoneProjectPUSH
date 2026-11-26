namespace CapstoneProjectPUSH.Services
{
    // Sends a photo the user picked to the api or removes the current one
    public static class ImageUpload
    {
        // Determines the action to perform from the image value
        public static async Task<string?> ApplyAsync(ApiClient apiClient, string path, string? imageValue)
        {
            string image = string.Empty;

            if (imageValue is not null)
            {
                image = imageValue.Trim();
            }

            // Empty -- remove image
            if (image.Length == 0)
            {
                ApiResult<ImageResponse> removeResult = await apiClient.DeleteAsync<ImageResponse>(path);

                if (!removeResult.Success)
                {
                    return removeResult.Message;
                }

                return null;
            }

            // Existing image url -- do nothing
            if (!ImageService.IsAppImage(image))
            {
                return null;
            }

            // Image -- upload
            return await UploadAsync(apiClient, path, image);
        }

        // Uploads a picked photo, then deletes the phone's copy
        private static async Task<string?> UploadAsync(ApiClient apiClient, string path, string localPath)
        {
            if (!File.Exists(localPath))
            {
                return "That photo couldn't be found. Please choose it again.";
            }

            ApiResult<ImageResponse> result;

            using (FileStream stream = File.OpenRead(localPath))
            {
                result = await apiClient.PostFileAsync<ImageResponse>(path, stream, Path.GetFileName(localPath));
            }

            if (!result.Success)
            {
                return result.Message;
            }

            ImageService.DeleteImage(localPath);

            return null;
        }
    }
}