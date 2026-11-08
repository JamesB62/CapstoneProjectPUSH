namespace CapstoneProjectPUSH.Models
{
    [Table("WeightLogs")]
    public class WeightLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public double WeightKg { get; set; } = 0;

        public DateTime DateTimeLogged { get; set; } = DateTime.Now;

        [Indexed]
        public int UserId { get; set; }

        [Ignore]
        public string UserDisplayName { get; set; } = string.Empty;
    }
}