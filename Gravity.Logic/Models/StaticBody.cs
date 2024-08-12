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
    public class StaticBody : Body
    {
        public StaticBody(double mass, Vector<double> initialPosition) : base(mass, initialPosition) { }
        public StaticBody(double mass) : base(mass, new Vector<double>(new double[] {0,0})) { }
    }
}
