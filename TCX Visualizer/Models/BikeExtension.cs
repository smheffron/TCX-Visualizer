using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class BikeExtension : Extension
    {
        public BikeExtension(double speed, double watts) : base(speed)
        {
            Watts = watts;
        }

        public double Watts
        {
            get;
            private set;
        }

        public override double Speed
        {
            get
            {
                return base.Speed * 2.23694;
            }
        }
    }
}
