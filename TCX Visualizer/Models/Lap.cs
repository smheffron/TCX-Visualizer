using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class Lap
    {
        public Lap(DateTime startTime, double totalTime, double totalDistance, double maxSpeed, List<Trackpoint> trackpoints, Sport type, double caloriesBurned = -1)
        {
            StartTime = startTime;
            TotalTimeSeconds = totalTime;
            TotalDistanceMeters = totalDistance;
            MaxSpeed = maxSpeed;
            Trackpoints = trackpoints;
            Type = type;
            CaloriesBurned = caloriesBurned;
        }

        public Sport Type
        {
            get;
            private set;
        }

        public DateTime StartTime
        {
            get;
            private set;
        }

        public double TotalTimeSeconds
        {
            get;
            private set;
        }

        public double TotalDistanceMeters
        {
            get;
            private set;
        }

        public double MaxSpeed
        {
            get;
            private set;
        }

        public double CaloriesBurned
        {
            get;
            private set;
        }
        public double AvgHeartRate
        {
            get
            {
                double avg = Trackpoints.Average(x => x.HeartRate);
                return avg;
            }
        }

        public double MaxHeartRate
        {
            get
            {
                double max = Trackpoints.Max(x => x.HeartRate);
                return max;
            }
        }

        public double AvgPower
        {
            get
            {
                if(Type == Sport.Biking)
                {
                    double avg = Trackpoints.Average(x => (x.Extensions as BikeExtension).Watts);
                    return avg;
                }
                else
                {
                    return -1;
                }
                
            }
        }

        public double MaxPower
        {
            get
            {
                if (Type == Sport.Biking)
                {
                    double max = Trackpoints.Max(x => (x.Extensions as BikeExtension).Watts);
                    return max;
                }
                else
                {
                    return -1;
                }
            }
        }

        public double MaxCadence
        {
            get
            {
                if (Type == Sport.Running)
                {
                    double max = Trackpoints.Max(x => (x.Extensions as RunExtension).Cadence);
                    return max;
                }
                else if (Type == Sport.Biking)
                {
                    double max = Trackpoints.Max(x => x.Cadence);
                    return max;
                }
                else
                {
                    return -1;
                }
            }
        }

        public List<Trackpoint> Trackpoints
        {
            get;
            private set;
        }

        public double AvgSpeed
        {
            get
            {
                double avg = TotalDistanceMeters / TotalTimeSeconds;
                return avg;
            }
        }
        public double AvgCadence
        {
            get
            {
                if(Type == Sport.Running)
                {
                    double avg = Trackpoints.Average(x => (x.Extensions as RunExtension).Cadence);
                    return avg;
                }
                else if(Type == Sport.Biking)
                {
                    double avg = Trackpoints.Average(x => x.Cadence);
                    return avg;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
