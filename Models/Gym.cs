namespace CapstoneProjectPUSH.Models
{
    [Table("Gyms")]
    public class Gym
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Add unique constraints?
        [NotNull, MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [NotNull, MaxLength(250)]
        public string StreetAddress { get; set; } = string.Empty;

        [NotNull, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Ignore]
        public string FullAddress => $"{StreetAddress}, {City}";
    }
}