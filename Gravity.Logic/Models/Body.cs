using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Logic.Models
{
    public abstract class Body
    {
        public double Mass { get; set; }
        public Vector<double> InitialPosition { get; }

        public Body(double mass, Vector<double> initialPosition)
        {
            Mass = mass;
            InitialPosition = initialPosition;
        }
    }
}
