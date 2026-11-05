using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneProjectPUSH.Models
{
    internal class WeightLog
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int UserId { get; set; }

        [NotNull]
        public double Weight { get; set; }

        [NotNull]
        public DateTime DateTimeLogged { get; set; } = DateTime.Now;

        [NotNull]
        public string WeightStatus { get; set; } = string.Empty;

        [NotNull]
        public int WeightKg { get; set; } = 0;





    }
}
