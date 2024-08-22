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
        private double _mass;
        private Vector2 _initialPosition;
        public double Mass
        {
            get { return _mass; }
            set
            {
                _mass = value;
                OnChange?.Invoke(this, EventArgs.Empty);
            }
        }
        public Vector2 InitialPosition
        {
            get { return _initialPosition; }
            set 
            {
                _initialPosition = value; 
                OnChange?.Invoke(this, EventArgs.Empty); 
            }
        }


        public delegate void Body_Changed(object sender, EventArgs e); // delegate for event
        public event Body_Changed ?OnChange; // event that is raised when the dynamic or static body is changed
    }
}
