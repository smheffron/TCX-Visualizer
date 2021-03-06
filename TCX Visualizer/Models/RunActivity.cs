﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    // extends base Activity class, no need for bike things such as power
    class RunActivity : Activity
    {
        public RunActivity(DateTime id, String device, List<Lap> laps) : base(id, device, laps)
        {

        }

        public String Name
        {
            get
            {
                return "Run at " + Id.ToString();
            }
        }

        public double MaxCadence
        {
            get { 
                double max = Laps.Max(x => x.Trackpoints.Max(y => (y.Extensions as RunExtension).Cadence));
                return max;
            }
        }

        public double AvgCadence
        {
            get
            {
                double avg = Laps.Average(x => x.Trackpoints.Average(y => (y.Extensions as RunExtension).Cadence));
                return avg;
            }
        }
    }
}
