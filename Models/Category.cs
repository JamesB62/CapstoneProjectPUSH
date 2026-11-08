namespace CapstoneProjectPUSH.Models
{
    [Table("Categories")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, Unique, MaxLength(50)]
        public string Title { get; set; } = string.Empty;
    }
}