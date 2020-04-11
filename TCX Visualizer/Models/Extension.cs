using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCX_Visualizer.Models
{
    class Extension
    {
        public Extension(double speed = -1)
        {
            Speed = speed;
        }

        public double Speed
        {
            get;
            private set;
        }
    }
}
