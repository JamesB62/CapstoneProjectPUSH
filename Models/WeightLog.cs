namespace CapstoneProjectPUSH.Models
{
    public class WeightLog
    {
        public int Id { get; set; }

        public double WeightKg { get; set; } = 0;

        public DateTime DateTimeLogged { get; set; } = DateTime.Now;

        public int UserId { get; set; }

        public string UserDisplayName { get; set; } = string.Empty;
    }
}