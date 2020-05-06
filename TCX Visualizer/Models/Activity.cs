using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class Activity
    {
        // base class that holds basic data such as id, device, laps
        public Activity(DateTime id, String device, List<Lap> laps)
        {
            Id = id;
            Device = device;
            Laps = laps;
        }

        public DateTime Id
        {
            get;
            private set;
        }

        public String Device
        {
            get;
            private set;
        }

        public List<Lap> Laps
        {
            get;
            private set;
        }

        public double MaxSpeed
        {
            get
            {
                double max = Laps.Max(x => x.MaxSpeed);
                return max;
            }
        }

        public double MaxHeartRate
        {
            get
            {
                double max = Laps.Max(x => x.MaxHeartRate);
                return max;
            }
        }

        public double TotalDistance
        {
            get
            {
                double total = Laps.Sum(x => x.TotalDistanceMeters);
                return total;
            }
        }

        public double TotalTime
        {
            get
            {

                double total = Laps.Sum(x => x.TotalTimeSeconds);
                return total;
            }
        }

        public double TotalCalories
        {
            get
            {
                double total = Laps.Sum(x => x.CaloriesBurned);
                return total;
            }
        }

        public double AvgSpeed
        {
            get
            {
                return (TotalDistance/TotalTime) * 2.23694;
            }
        }

        public double AvgHeartRate
        {
            get
            {
                double total = Laps.Average(x => x.AvgHeartRate);
                return total;
            }
        }


    }
}
