using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Gravity.Logic.Models
{
    // a class used to calculate the motion of a 1 body gravitational system, this class works through solving the newtonian gravity differential equation 
    public class GravityCalculator
    {
        readonly double G = 6.673 * Math.Pow(10, -11); // Gravitational Constant

        // variables that are the same across various methods
        private double C;
        private double C1;

        public double Direction { get; private set; } // direction in degrees of the force relative to the dynamic body
        public double InitialDistance { get; private set; } // the initial distance from the dynamic and static bodies
        public double TimeOfCollision { get; private set; }

        public delegate void StaticOrDynamicBody_Changed(object sender, EventArgs e); // delegate for event
        public event StaticOrDynamicBody_Changed OnChange; // event that is raised when the dynamic or static body is changed

        // The 2 bodies
        private DynamicBody _dynamicObject;
        private StaticBody _staticObject;

        public DynamicBody DynamicObject // The body that moves
        {
            get
            {
                return _dynamicObject;
            }
            set
            {
                Changed(this, EventArgs.Empty);
                _dynamicObject = value;
            }
        } 
        public StaticBody StaticObject // The body that doesn't move
        {
            get { return _staticObject; }
            set
            {
                Changed(this, EventArgs.Empty);
                _staticObject = value;
            }
        }
        public double CurrentDistance // the current distance from the dynamic and static bodies
        {
            get { return Vector2.Distance(DynamicObject.CurrentPosition, StaticObject.InitialPosition); }
        }
        public double CurrentSpeed // the current speed of the dynamic body
        {
            get { return Math.Sqrt((2 * G * StaticObject.Mass / CurrentDistance) + (2 * C / DynamicObject.Mass)); }
        }
        public double CurrentAcceleration // the current acceleraction of the dynamic body
        {
            get { return G * StaticObject.Mass / Math.Pow(CurrentDistance, 2); }
        }

        public GravityCalculator(DynamicBody dynamicObject, StaticBody staticObject) //constructor
        {
            // initialize the bodies
            DynamicObject = dynamicObject;
            StaticObject = staticObject;

            DynamicObject.OnChange += Changed; // connect the method to the event
            StaticObject.OnChange += Changed;
            OnChange += On_Changed;
            OnChange?.Invoke(this, new EventArgs()); // initialize all properties and variables
        }

        // calculates the position of the dynamic body after the specified amount of time.
        // precision is measured in digits after the decimal
        // each time this method it called, it will update DynamicObject.CurrentPosition
        public Vector2 CalculatePoint(double time, int precision)
        {
            double desiredValue = 2 * (time + C1) * Math.Sqrt(-2 * C / DynamicObject.Mass) / InitialDistance; // the value that inputValue + sin(inputValue) needs to equal
            double inputValue = desiredValue; // initialize inputValue with desiredValue (desiredValue should be within 1 meter of the wanted inputValue because of the way the graph increases)
            double resultantValue = inputValue - Math.Sin(inputValue); ; // the value of inputValue - sin(inputValue)
            int Count = 0; // how many times the loop has iterated

            // This loop uses the binary search algorithm but on the mathematical function x - sin(x)
            while (Math.Abs(desiredValue - resultantValue) > Math.Pow(10, -1 * (precision+1)))
            {
                resultantValue = inputValue - Math.Sin(inputValue);
                
                if (resultantValue < desiredValue)
                {
                    inputValue += Math.Pow(2, -1 * Count);
                }
                else if (resultantValue > desiredValue)
                {
                    inputValue -= Math.Pow(2, -1 * Count);
                }
                else // if they are equal (very unlikely)
                {
                    break;
                }

                Count++;
            }

            double resultDisplacement = InitialDistance * Math.Pow(Math.Sin(inputValue / 2D), 2D); // gets the approximate distance of the dynamic object from the static object at a certain time
            double scale = resultDisplacement / InitialDistance; // variable used to scale the initial position to the ending position using the initial and ending distance
            Vector2 result = new Vector2((float)((DynamicObject.InitialPosition.X - StaticObject.InitialPosition.X) * scale + StaticObject.InitialPosition.X), (float)((DynamicObject.InitialPosition.Y -StaticObject.InitialPosition.Y) * scale + StaticObject.InitialPosition.Y)); // scaling the initial postition of the dynamic object to the ending position

            DynamicObject.CurrentPosition = result; // change the current position of the dynamic object
            return result;
        }
        // Calculates the time it takes for the dynamic body to get to a certain distance
        public double CalculateTime(double displacement)
        {
            double repeatedTerm = InitialDistance / displacement; // a term that is repeated throughout the calculation
            double coefficient = -1 * InitialDistance / Math.Sqrt(-2 * C / DynamicObject.Mass); // a coefficient to a large term in the next calculation

            return coefficient * (-1 * Math.Sqrt((1D - 1D / repeatedTerm) / repeatedTerm) + Math.Asin(1 / Math.Sqrt(repeatedTerm))) - C1; // calculate time
        }
        private void On_Changed(object sender, EventArgs e) // method invoked when OnChange is raised
        {
            // update direction
            Vector2 directionVector = Vector2.Subtract(DynamicObject.InitialPosition, StaticObject.InitialPosition);
            Direction = (directionVector.Y < 0 ? -1 : 1) * Math.Acos(directionVector.X / Math.Sqrt(Math.Pow(directionVector.Y, 2) + Math.Pow(directionVector.X, 2))) * 180 / Math.PI;

            // update initial distance
            InitialDistance = Vector2.Distance(DynamicObject.InitialPosition, StaticObject.InitialPosition);

            // update constants
            C = (-1 * G * DynamicObject.Mass * StaticObject.Mass) / InitialDistance;
            C1 = -1 / 2D * Math.PI * InitialDistance * Math.Sqrt(-1 / 2D * DynamicObject.Mass / C);

            // update time of collision
            TimeOfCollision = -C1;
        }
        private void Changed(object sender, EventArgs e)
        {
            OnChange?.Invoke(this, EventArgs.Empty);
        }
    }
}
