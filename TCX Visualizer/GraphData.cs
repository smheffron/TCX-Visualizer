using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer
{
    class GraphData
    {
        public GraphData(List<DataPoint> points, double max, double min)
        {
            this.Max = max;
            this.Min = min;
            this.Points = points;
        }

         public List<DataPoint> Points
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
