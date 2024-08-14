using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Gravity.Logic.Models
{
    // a class used to calculate the motion of a 1 body gravitational system, this class works through solving the newtonian gravity differential equation 
    public class GravityCalculator
    {
        // The 2 bodies
        public DynamicBody DynamicObject { get; } // The body that moves
        public StaticBody StaticObject { get; } // The body that doesn't move
        readonly double G = 6.673 * Math.Pow(10, -11); // Gravitational Constant
        public double initialDisplacement { get; } // the initial distance from the dynamic and static bodies
        public Vector2 direction { get; } // a direction vector that may or may not be useful

        // variables that are the same across various methods
        readonly double C;
        readonly double C1;

        public GravityCalculator(DynamicBody dynamicObject, StaticBody staticObject)
        {
            DynamicObject = dynamicObject;
            StaticObject = staticObject;
            initialDisplacement = CalculateDistance();
            direction = Vector2.Subtract(StaticObject.InitialPosition, DynamicObject.InitialPosition);

            // initialize the constants used in the calculations
            C = (-1 * G * DynamicObject.Mass * StaticObject.Mass) / initialDisplacement;
            C1 = -1 / 2D * Math.PI * initialDisplacement * Math.Sqrt(-1 / 2D * DynamicObject.Mass / C);

        }
        // calculates the current distance of the 
        public float CalculateDistance()
        {
            return Vector2.Distance(DynamicObject.CurrentPosition, StaticObject.InitialPosition);
        }

        // calculates the position of the dynamic body after the specified amount of time.
        // precision is measured in digits after the decimal
        // each time this method it called, it will update DynamicObject.CurrentPosition
        public Vector2 CalculatePoint(double time, int precision)
        {
            double desiredValue = 2 * (time + C1) * Math.Sqrt(-2 * C / DynamicObject.Mass) / initialDisplacement; // the value that inputValue + sin(inputValue) needs to equal
            double inputValue = desiredValue; // initialize inputValue with desiredValue (desiredValue should be within 1 meter of the wanted inputValue because of the way the graph increases)
            double resultantValue = 0; // the value of inputValue - sin(inputValue)
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

            double resultDisplacement = initialDisplacement * Math.Pow(Math.Sin(inputValue / 2D), 2D); // gets the approximate distance of the dynamic object from the static object at a certain time
            double scale = resultDisplacement / initialDisplacement; // variable used to scale the initial position to the ending position using the initial and ending distance
            Vector2 result = new Vector2((float)((DynamicObject.InitialPosition.X - StaticObject.InitialPosition.X) * scale + StaticObject.InitialPosition.X), (float)((DynamicObject.InitialPosition.Y -StaticObject.InitialPosition.Y) * scale + StaticObject.InitialPosition.Y)); // scaling the initial postition of the dynamic object to the ending position

            DynamicObject.CurrentPosition = result; // change the current position of the dynamic object
            return result;
        }
        // Calculates the time it takes for the dynamic body to get to a certain distance
        public double CalculateTime(double displacement)
        {
            double repeatedTerm = initialDisplacement / displacement; // a term that is repeated throughout the calculation
            double coefficient = -1D * initialDisplacement / Math.Sqrt(-2D * C / DynamicObject.Mass); // a coefficient to a large term in the next calculation

            return coefficient * (-1 * Math.Sqrt((1D - 1D / repeatedTerm) / repeatedTerm) + Math.Asin(1 / Math.Sqrt(repeatedTerm))) - C1; // calculate time
        }
    }
}
