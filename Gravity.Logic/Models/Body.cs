using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Logic.Models
{
    // base class used to define an object with a gravitational field
    public abstract class Body
    {
        public double Mass { get; set; }
        public Vector2 InitialPosition { get; }

        public Body(double mass, Vector2 initialPosition)
        {
            Mass = mass;
            InitialPosition = initialPosition;
        }
    }
}
