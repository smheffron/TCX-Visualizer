using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class RunExtension : Extension
    {
        public RunExtension(double speed, double cadence) : base(speed)
        {
            Cadence = cadence;
        }

        public double Cadence
        {
            get;
            private set;
        }
    }
}
