namespace CapstoneProjectPUSH.Models.Charts
{
    // Values needed to plot data on line graph
    public class WeightPoint
    {
        public DateTime Date { get; set; }
        public double WeightKg { get; set; }
    }

    public class WeightChartDrawable : IDrawable
    {
        // Graph padding
        private const float LeftPadding = 35;
        private const float RightPadding = 10;
        private const float TopPadding = 10;
        private const float BottomPadding = 20;

        // Data drawable needs to create the graph
        private readonly List<WeightPoint> _points;
        private readonly DateTime _rangeStart;
        private readonly DateTime _rangeEnd;
        private readonly List<string> _labels;

        // Initialises the graph with placeholder/empty values
        public WeightChartDrawable()
        {
            _points = new List<WeightPoint>();
            _rangeStart = DateTime.Today;
            _rangeEnd = DateTime.Today.AddDays(1);
            _labels = new List<string>();
        }

        // Stores chart data for drawing
        public WeightChartDrawable(List<WeightPoint> points, DateTime rangeStart, DateTime rangeEnd, List<string> labels)
        {
            _points = points;
            _rangeStart = rangeStart;
            _rangeEnd = rangeEnd;
            _labels = labels;
        }

        // Calculates and draws the line graph
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Defines the drawable graph area
            float chartLeft = dirtyRect.Left + LeftPadding;
            float chartRight = dirtyRect.Right - RightPadding;
            float chartTop = dirtyRect.Top + TopPadding;
            float chartBottom = dirtyRect.Bottom - BottomPadding;

            // Calculates the width and height of the drawable area
            float chartWidth = chartRight - chartLeft;
            float chartHeight = chartBottom - chartTop;

            // Defines the chart colours from the app theme
            Color lineColour = GetThemeColour("Accent", Colors.LimeGreen);
            Color labelColour = GetThemeColour("TextSecondary", Colors.LightGray);
            Color baselineColour = GetThemeColour("InputBorder", Colors.Gray);

            // Defines the font colour and size for labels
            canvas.FontColor = labelColour;
            canvas.FontSize = 12;

            // Draws the x-axis labels, one per filter bucket (matches the bar chart)
            if (_labels.Count > 0)
            {
                float slotWidth = chartWidth / _labels.Count;

                for (int i = 0; i < _labels.Count; i++)
                {
                    float labelX = chartLeft + (slotWidth * i);

                    canvas.DrawString(_labels[i], labelX, chartBottom + 4, slotWidth, BottomPadding - 4, HorizontalAlignment.Center, VerticalAlignment.Top);
                }
            }

            // Draws the baseline along the bottom of the graph
            canvas.StrokeColor = baselineColour;
            canvas.StrokeSize = 1;
            canvas.DrawLine(chartLeft, chartBottom, chartRight, chartBottom);

            // Graph equivalent of an "empty view"
            if (_points.Count == 0)
            {
                canvas.DrawString("No weigh-ins for this period", chartLeft, chartTop, chartWidth, chartHeight, HorizontalAlignment.Center, VerticalAlignment.Center);
                return;
            }

            // Pads the y-axis so a flat line doesn't sit on the edges
            double minWeight = Math.Floor(_points.Min(p => p.WeightKg) - 1);
            double maxWeight = Math.Ceiling(_points.Max(p => p.WeightKg) + 1);
            double weightRange = maxWeight - minWeight;

            // Y-axis labels
            canvas.DrawString($"{maxWeight:0}", dirtyRect.Left, chartTop - 6, LeftPadding - 6, 14, HorizontalAlignment.Right, VerticalAlignment.Center);
            canvas.DrawString($"{minWeight:0}", dirtyRect.Left, chartBottom - 6, LeftPadding - 6, 14, HorizontalAlignment.Right, VerticalAlignment.Center);

            // Calculates where each weigh-in sits on graph
            double totalDays = (_rangeEnd - _rangeStart).TotalDays;
            var screenPoints = new List<PointF>();

            foreach (var point in _points)
            {
                // Sets each day's point to the middle of that day
                double dayOffset = (point.Date.Date - _rangeStart).TotalDays + 0.5;

                float x = chartLeft + (float)(dayOffset / totalDays) * chartWidth;
                float y = chartBottom - (float)((point.WeightKg - minWeight) / weightRange) * chartHeight;

                screenPoints.Add(new PointF(x, y));
            }

            // Draws a line between weigh-ins
            canvas.StrokeColor = lineColour;
            canvas.StrokeSize = 2;
            canvas.StrokeLineJoin = LineJoin.Round;

            for (int i = 1; i < screenPoints.Count; i++)
            {
                canvas.DrawLine(screenPoints[i - 1], screenPoints[i]);
            }

            // Draws a dot on each weigh-in
            canvas.FillColor = lineColour;

            foreach (var screenPoint in screenPoints)
            {
                canvas.FillCircle(screenPoint, 3);
            }
        }

        // Gets a colour from colors.xaml so the chart matches the app theme
        private static Color GetThemeColour(string key, Color fallback)
        {
            if (Application.Current != null && Application.Current.Resources.TryGetValue(key, out object value))
            {
                if (value is Color colour)
                {
                    return colour;
                }
            }

            return fallback;
        }
    }
}