namespace CapstoneProjectPUSH.Models
{
    [Table("Gyms")]
    public class Gym
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(250)]
        public string Name { get; set; } = string.Empty;

        [NotNull, MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [NotNull, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Ignore]
        public string FullAddress => $"{Address}, {City}";

        [Ignore]
        public string NameAndCity => $"{Name} ({City})";
    }
}