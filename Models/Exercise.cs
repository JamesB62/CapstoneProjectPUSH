using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneProjectPUSH.Models
{
    public class Exercise
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
    }
}
