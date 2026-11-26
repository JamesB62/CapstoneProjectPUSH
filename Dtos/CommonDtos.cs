namespace CapstoneProjectPUSH.Dtos
{
    // Simple success/message reply
    public class MessageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // Reply after adding something that the app needs the new id of
    public class CreatedResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}