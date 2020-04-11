using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class Trackpoint
    {
        public Trackpoint(DateTime time, Coordinate pos, double alt, double dis, double hr, double cadence = -1, Extension extensions = null)
        {
            Time = time;
            Position = pos;
            Altitude = alt;
            Distance = dis;
            HeartRate = hr;
            Cadence = cadence;
            Extensions = extensions;
        }

        public DateTime Time
        {
            get;
            private set;
        }

        public Coordinate Position
        {
            get;
            private set;
        }

        public double Altitude
        {
            get;
            private set;
        }

        public double Distance
        {
            get;
            private set;
        }

        public double HeartRate
        {
            get;
            private set;
        }

        public double Cadence
        {
            get;
            private set;
        }

        public Extension Extensions
        {
            get;
            private set;
        }

    }
}
