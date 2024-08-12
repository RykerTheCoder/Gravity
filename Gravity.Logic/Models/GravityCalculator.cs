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
    public class GravityCalculator
    {
        DynamicBody DynamicObject { get; set; }
        StaticBody StaticObject { get; set; }
        readonly double G = 6.673 * Math.Pow(10, -11);
        double initialDisplacement { get; set; }
        Vector<double> direction { get; set; }

        public GravityCalculator(DynamicBody dynamicObject, StaticBody staticObject)
        {
            DynamicObject = dynamicObject;
            StaticObject = staticObject;
            initialDisplacement = CalculateDisplacement();
            direction = Vector.Subtract(StaticObject.InitialPosition, DynamicObject.InitialPosition);
        }
        public double CalculateDisplacement()
        {
            double dx = DynamicObject.CurrentPosition[0] - StaticObject.InitialPosition[0];
            double dy = DynamicObject.CurrentPosition[1] - StaticObject.InitialPosition[1];

            return Math.Sqrt(dx * dx + dy * dy);
        }
        public Vector<double> CalculatePoint(double time, int precision)
        {
            double C = (-1 * G * DynamicObject.Mass * StaticObject.Mass) / initialDisplacement;
            double C1 = -1 / 2 * Math.PI * initialDisplacement * Math.Sqrt(-1 / 2 * DynamicObject.Mass / C);
            double desiredValue = 2 * (time + C1) * Math.Sqrt(-2 * C / DynamicObject.Mass) / initialDisplacement;
            double inputValue = desiredValue;
            double resultantValue = 0;
            int Count = 0;

            while (Math.Abs(desiredValue - resultantValue) < Math.Pow(10, -1 * (precision - 1)))
            {
                resultantValue = Math.Sin(inputValue) + inputValue;
                
                if (resultantValue < desiredValue)
                {
                    inputValue += Math.Pow(2, -1 * Count);
                }
                else if (resultantValue > desiredValue)
                {
                    inputValue -= Math.Pow(2, -1 * Count);
                }
                else
                {
                    break;
                }

                Count++;
            }

            double resultDisplacement = initialDisplacement * Math.Pow(Math.Sin(inputValue / 2), 2);
            double scale = resultDisplacement / initialDisplacement;
            Vector<double> result = new Vector<double>(new double[] { DynamicObject.InitialPosition[0] * scale, DynamicObject.InitialPosition[1] * scale });

            DynamicObject.CurrentPosition = result;
            return result;
        }
        public double CalculateTime(double displacement)
        {
            double C = (-1 * G * DynamicObject.Mass * StaticObject.Mass) / initialDisplacement;
            double C1 = -1 / 2 * Math.PI * initialDisplacement * Math.Sqrt(-1 / 2 * DynamicObject.Mass / C);
            double repeatedTerm = initialDisplacement / displacement;
            double coefficient = -1 * initialDisplacement / Math.Sqrt(-2 * C / DynamicObject.Mass);

            return coefficient * (-1 * Math.Sqrt((1 - 1 / repeatedTerm) / repeatedTerm) + Math.Asin(1 / Math.Sqrt(repeatedTerm))) - C1;
        }
    }
}
