using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneProjectPUSH.Models
{
    public class Exercise
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
    }
}
