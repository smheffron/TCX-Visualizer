using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class Coordinate
    {
        public Coordinate(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }
        public double Lat
        {
            get;
            private set;
        }
        public double Lon
        {
            get;
            private set;
        }
    }
}
