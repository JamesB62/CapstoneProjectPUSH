using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneProjectPUSH.Models
{
    [Table("Stats")]
    public class Stats
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public int TotalWorkouts { get; set; }
        public int TotalDuration { get; set; }
        public int TotalCalories { get; set; }

        public DateTime LastWorkoutDate { get; set; } = DateTime.Now;



    }
}
