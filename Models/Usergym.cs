namespace CapstoneProjectPUSH.Models
{
    // Links a user to a gym (a user can link several gyms)
    [Table("UserGyms")]
    public class UserGym
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime DateTimeLinked { get; set; } = DateTime.Now;

        // A user can only link the same gym once
        [Indexed("UserGymIndex", 1, Unique = true)]
        public int UserId { get; set; }

        [Indexed("UserGymIndex", 2, Unique = true)]
        public int GymId { get; set; }
    }
}