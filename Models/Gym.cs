namespace CapstoneProjectPUSH.Models
{
    public class Gym
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string FullAddress => $"{Address}, {City}";

        public string NameAndCity => $"{Name} ({City})";
    }
}