using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class BikeActivity : Activity
    {
        public BikeActivity(DateTime id, String device, List<Lap> laps) : base(id, device, laps)
        {

        }

        public String Name
        {
            get
            {
                return "Bike ride at " + Id.ToString();
            }
        }

        public double MaxCadence
        {
            get
            {
                double max = Laps.Max(x => x.MaxCadence);
                return max;
            }
        }

        public double AvgCadence
        {
            get
            {
                double avg = Laps.Average(x => x.Trackpoints.Average(y => y.Cadence));
                return avg;
            }
        }

        public double MaxPower
        {
            get
            {
                double max = Laps.Max(x => x.Trackpoints.Max(y => (y.Extensions as BikeExtension).Watts));
                return max;
            }
        }
        
        public double AvgPower
        {
            get
            {
                double avg = Laps.Average(x => x.Trackpoints.Average(y => (y.Extensions as BikeExtension).Watts));
                return avg;
            }
        }
        
        
    }
}
