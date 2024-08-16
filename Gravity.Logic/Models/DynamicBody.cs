using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.Logic.Models
{
    // A class representing a body that is affected by gravity
    public class DynamicBody : Body
    {
        private Vector2 _initialPosition;
        public new Vector2 InitialPosition
        {
            set
            {
                CurrentPosition = _initialPosition = value;
            }
            get
            {
                return _initialPosition;
            }
        }
        public Vector2 CurrentPosition { get; set; }

        public DynamicBody(double mass, Vector2 initialPosition){ InitialPosition = initialPosition; Mass = mass; }
    }
}
