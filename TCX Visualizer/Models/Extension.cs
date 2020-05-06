using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    // base class to hold trackpoint stats, only speed
    class Extension
    {
        private double speed;

        public Extension(double speed = -1)
        {
            Speed = speed;
        }

        public virtual double Speed
        {
            get
            {
                return speed;
            }
            private set
            {
                speed = value;
            }
        }
    }
}
