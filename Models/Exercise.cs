namespace CapstoneProjectPUSH.Models
{
    [Table("Exercises")]
    public class Exercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        // May need to increase MaxLength value
        [NotNull, MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Indexed]
        public int CategoryId { get; set; }

        [Ignore]
        public string CategoryName { get; set; } = string.Empty;
    }
}