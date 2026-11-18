namespace CapstoneProjectPUSH.Models.Charts
{
    public class ChartBar
    {
        public string Label { get; set; } = string.Empty;
        public double Minutes { get; set; }
        public double BarHeight { get; set; }

        public string ValueText
        {
            get
            {
                if (Minutes <= 0)
                {
                    return string.Empty;
                }
                
                if (Minutes >= 60)
                {
                    return $"{Minutes / 60:0.#}h";
                }

                return $"{Minutes:0}m";
            }
        }
    }
}