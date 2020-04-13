using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class Trackpoint
    {
        private double cadence = -1;
        private double altitude = -1;
        public Trackpoint(DateTime time, Coordinate pos, double alt, double dis, double hr, Sport type, double cadence = -1, Extension extensions = null)
        {
            Time = time;
            Position = pos;
            Altitude = alt;
            Distance = dis;
            HeartRate = hr;
            Cadence = cadence;
            Extensions = extensions;
            Type = type;
        }

        public Sport Type
        {
            get;
            private set;
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
            get
            {
                return this.altitude * 3.28084;
            }
            private set
            {
                this.altitude = value;
            }
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
            get
            {
                if(Type == Sport.Running)
                {
                    return (Extensions as RunExtension).Cadence;
                }
                else if(Type == Sport.Biking)
                {
                    return this.cadence;
                }
                else
                {
                    return -1;
                }
            }
            private set
            {
                this.cadence = value;
            }
        }

        public Extension Extensions
        {
            get;
            private set;
        }

    }
}
