using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer
{
    class BarChartData
    {
        public BarChartData(List<ColumnItem> points, double max, double min)
        {
            this.Max = max;
            this.Min = min;
            this.Points = points;
        }

        public List<ColumnItem> Points
        {
            get;
            set;
        }

        public double Max
        {
            get;
            set;
        }

        public double Min
        {
            get;
            set;
        }
    }
}
