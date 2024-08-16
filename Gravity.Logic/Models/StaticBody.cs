using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Gravity.Logic.Models
{
    // representing a body that is NOT affected by gravity, but it still has a gravitational field
    public class StaticBody : Body
    {
        public StaticBody(double mass, Vector2 initialPosition)
        {
            Mass = mass;
            InitialPosition = initialPosition;
        }
        public StaticBody(double mass)
        {
            Mass = mass;
            InitialPosition = new Vector2(0, 0);
        }
    }
}
