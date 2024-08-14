using Gravity.Logic.Models;
using System.Numerics;

namespace Gravity.Testing
{
    public class CalculatorTest
    {
        static DynamicBody dynamicBody = new DynamicBody(10000, new Vector2(2, 0));
        static StaticBody staticBody = new StaticBody(10000000000);
        static GravityCalculator gravityCalculator = new GravityCalculator(dynamicBody, staticBody);

        [Fact]
        public void CalculateTimeTest()
        {
            float desiredResult = 3.14707507923F;

            Assert.True(Math.Abs(gravityCalculator.CalculateTime(1) - desiredResult) < 0.0001);
        }
        [Fact]
        public void CalculatePointTest()
        {
            Vector2 desiredResult = new Vector2(1.11344F, 0);
            Vector2 result = gravityCalculator.CalculatePoint(3, 5);

            Assert.True(Math.Abs(desiredResult.X - result.X) < 0.00001);
        }
        [Fact]
        public void CalculatePointTest2D() // move the dynamic body anywhere in a 2 dimensional space
        {
            DynamicBody dynamicBody = new DynamicBody(10000, new Vector2((float)Math.Sqrt(2), (float)Math.Sqrt(2)));
            StaticBody staticBody = new StaticBody(10000000000);
            GravityCalculator gravityCalculator = new GravityCalculator(dynamicBody, staticBody);

            float desiredResult = 1.11344F;
            Vector2 result = gravityCalculator.CalculatePoint(3, 5);

            Assert.True(Math.Abs(Vector2.Distance(result, staticBody.InitialPosition) - desiredResult) < 0.00001);
        }

        [Fact]
        public void CalculatePointTest2DStatic() // move the static body away from the origin
        {
            DynamicBody dynamicBody = new DynamicBody(10000, new Vector2((float)Math.Sqrt(2) / 2, (float)Math.Sqrt(2) / 2));
            StaticBody staticBody = new StaticBody(10000000000, new Vector2(-(float)Math.Sqrt(2) / 2, -(float)Math.Sqrt(2) / 2));
            GravityCalculator gravityCalculator = new GravityCalculator(dynamicBody, staticBody);

            float desiredResult = 1.11344F;
            Vector2 result = gravityCalculator.CalculatePoint(3, 5);

            Assert.True(Math.Abs(Vector2.Distance(result, staticBody.InitialPosition) - desiredResult) < 0.00001);
        }
    }
}