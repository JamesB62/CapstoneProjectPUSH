using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneProjectPUSH.Models
{
    public class Workout
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
