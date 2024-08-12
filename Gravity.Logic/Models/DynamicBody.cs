using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Logic.Models
{
    public class DynamicBody : Body
    {
        public Vector<double> CurrentPosition { get; set; }

        public DynamicBody(double mass, Vector<double> initialPosition) : base(mass, initialPosition) { CurrentPosition = InitialPosition; }
    }
}
