using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    // base class to hold all important data relating to a lap
    //including start time, total time, max speed, trackpoints list, calories burned
    class Lap
    {
        private double maxSpeed = 0;
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

        public String DisplayName
        {
            get;
            set;
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

        public String TotalTimeMinutes
        {
            // convert from seconds to a timespan in hours, minutes, seconds
            get
            {
                TimeSpan t;
                try
                {
                    t = TimeSpan.FromSeconds(TotalTimeSeconds);
                }
                catch
                {
                    return (TotalTimeSeconds/60).ToString("0.00");
                }
                return t.Hours.ToString("00") +":"+ t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00");
            }
        }

        public double TotalDistanceMeters
        {
            get;
            private set;
        }

        public String TotalDistanceMiles
        {
            get
            {
                return Math.Round((TotalDistanceMeters )* 0.000621371, 2).ToString("0.00");
            }
        }

        public double MaxSpeed
        {
            get
            {
                return maxSpeed *2.23694;
            }
            private set
            {
                maxSpeed = value;
            }
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
                if(TotalTimeSeconds <= 0)
                {
                    return 0;
                }
                else
                {
                    double avg = Math.Round((TotalDistanceMeters / TotalTimeSeconds) * 2.23694, 2);
                    return avg;
                }
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
        public double ElevationGain
        {
            // calculates elevation on the lap gain based on trackpoint elevation markers
            get
            {
                double asc = 0;
                double desc = 0;
                double largest = 0;
                double tempLarge = 0;
                bool prevAsc = false;
                var altPoints = Trackpoints.Select(x => x.Altitude);

                double prev = altPoints.First();
                foreach (double point in altPoints)
                {
                    if (prev < point)
                    {
                        asc += point - prev;
                        if (prevAsc)
                        {
                            tempLarge += point - prev;
                            if (tempLarge > largest)
                            {
                                largest = tempLarge;
                            }
                        }
                        prevAsc = true;
                    }
                    else if (prev > point)
                    {
                        prevAsc = false;
                        tempLarge = 0;
                        desc += prev - point;
                    }
                    prev = point;
                }

                return asc;
            }
        }
        public double ElevationLoss
        {
            // calculates elevation loss on the lap based on trackpoint eleveation markers
            get
            {
                double asc = 0;
                double desc = 0;
                double largest = 0;
                double tempLarge = 0;
                bool prevAsc = false;
                var altPoints = Trackpoints.Select(x => x.Altitude);

                double prev = altPoints.First();
                foreach (double point in altPoints)
                {
                    if (prev < point)
                    {
                        asc += point - prev;
                        if (prevAsc)
                        {
                            tempLarge += point - prev;
                            if (tempLarge > largest)
                            {
                                largest = tempLarge;
                            }
                        }
                        prevAsc = true;
                    }
                    else if (prev > point)
                    {
                        prevAsc = false;
                        tempLarge = 0;
                        desc += prev - point;
                    }
                    prev = point;
                }

                return desc;
            }
        }

        // calcultes the biggest climb on the lap from the trackpoint elevation markers
        public double BiggestClimb
        {
            get
            {
                double asc = 0;
                double desc = 0;
                double largest = 0;
                double tempLarge = 0;
                bool prevAsc = false;
                var altPoints = Trackpoints.Select(x => x.Altitude);

                double prev = altPoints.First();
                foreach (double point in altPoints)
                {
                    if (prev < point)
                    {
                        asc += point - prev;
                        if (prevAsc)
                        {
                            tempLarge += point - prev;
                            if (tempLarge > largest)
                            {
                                largest = tempLarge;
                            }
                        }
                        prevAsc = true;
                    }
                    else if (prev > point)
                    {
                        prevAsc = false;
                        tempLarge = 0;
                        desc += prev - point;
                    }
                    prev = point;
                }

                return largest;
            }
        }
    }
}
