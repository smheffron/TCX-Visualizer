using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    // extends base class Extension that gets its cadence from a different place than a cycling extension, also no power data
    class RunExtension : Extension
    {
        private double cadence = -1;
        public RunExtension(double speed, double cadence) : base(speed)
        {
            Cadence = cadence;
        }

        public double Cadence
        {
            get
            {
                return this.cadence * 2;
            }
            private set
            {
                this.cadence = value;
            }
        }
    }
}
