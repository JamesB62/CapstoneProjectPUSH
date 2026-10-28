namespace CapstoneProjectPUSH.Models
{
    [Table("Categories")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Need case-insensitive duplicate check
        [NotNull, Unique, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}