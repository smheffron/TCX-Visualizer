using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class Lap
    {
        public Lap(DateTime startTime, double totalTime, double totalDistance, double maxSpeed, List<Trackpoint> trackpoints, double caloriesBurned = -1, double avgHeartRate = -1, double maxHr= -1, double maxCadence = -1)
        {
            StartTime = startTime;
            TotalTimeSeconds = totalTime;
            TotalDistanceMeters = totalDistance;
            MaxSpeed = maxSpeed;
            Trackpoints = trackpoints;
            CaloriesBurned = caloriesBurned;
            AvgHeartRate = avgHeartRate;
            MaxHeartRate = maxHr;
            MaxCadence = maxCadence;
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
            get;
            private set;
        }

        public double MaxHeartRate
        {
            get;
            private set;
        }

        public double MaxCadence
        {
            get;
            private set;
        }

        public List<Trackpoint> Trackpoints
        {
            get;
            private set;
        }
    }
}
