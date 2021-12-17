using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class Day17 : Solution
    {
        private int _xMin;
        private int _xMax;
        private int _yMin;
        private int _yMax;
        private bool _samples;

        private Day17() : base(17)
        {
            _samples = true;
        }
        public static void Go()
        {
            var day = new Day17();
            day.Solve();
        }

        protected override void ProcessLine(string line)
        {
            if (RunSamples && _samples)
            {
                // Setting up sample values
                _xMin = 20;
                _xMax = 30;
                _yMin = -10;
                _yMax = -5;
            }
            else
            {
                // Setting up actual values
                _xMin = 117;
                _xMax = 164;
                _yMin = -140;
                _yMax = -89;
            }
        }
        protected override bool RunSamples => true;
        protected override void ResetAfterSamples()
        {
            _samples = false;
        }
        protected override bool RunActuals => false;

        protected override ulong SolutionA()
        {
            // Don't care about X, since it's independent

            ulong maxValue = 0;
            ulong maxSpeed = (ulong)(-_yMin);
            for (ulong initialVelocity = 1; initialVelocity < maxSpeed; ++initialVelocity)
            {
                var height = (initialVelocity * (initialVelocity + 1)) / 2;

                // It will be going the same velocity when it get's back to 0, but in the opposite direction.
                int y = 0;
                for (int v = (int)initialVelocity; v < (int)maxSpeed; ++v)
                {
                    y -= v;
                    if (y >= _yMin && y <= _yMax)
                    {
                        // Success!
                        maxValue = height;
                        break;
                    }
                }
            }

            return maxValue;
        }
        protected override ulong SolutionB()
        {
            var valid = new List<Tuple<int, int>>();

            // At some point our xrange ends its valid values.  These are those extrema.
            double vxMinMax = -0.5 + Math.Sqrt(2 * _xMin + 1);
            int maxInitialVX = (int)Math.Ceiling(vxMinMax);
            double vxMaxMin = -0.5 + Math.Sqrt(2 * _xMax + 1);
            int minFinalVX = (int)Math.Floor(vxMaxMin);

            for (int steps = 1; steps < 100; ++steps)
            {
                int offset = (steps * (steps - 1)) / 2;
                int initialVX = (_xMin + offset) / steps;
                if ((_xMin + offset) % steps != 0)
                    ++initialVX;
                if (steps >= maxInitialVX)
                    initialVX = maxInitialVX;
                int finalVX = (_xMax + offset) / steps;
                if (steps >= minFinalVX)
                    finalVX = minFinalVX;

                int initialVY = (_yMax + offset) / steps;
                if ((_yMax + offset) % steps != 0)
                    --initialVY;
                int finalVY = (_yMin + offset) / steps;

                for (int vX = initialVX; vX <= finalVX; ++vX)
                {
                    for (int vY = initialVY; vY >= finalVY; --vY)
                    {
                        valid.Add(Tuple.Create(vX, vY));
                    }
                }
            }

            // for (ulong initialVelocity = 1; initialVelocity < maxSpeed; ++initialVelocity)
            // {
            //     var height = (initialVelocity * (initialVelocity + 1)) / 2;

            //     // It will be going the same velocity when it get's back to 0, but in the opposite direction.
            //     int y = 0;
            //     for (int v = (int)initialVelocity; v < (int)maxSpeed; ++v)
            //     {
            //         y -= v;
            //         if (y >= _yMin && y <= _yMax)
            //         {
            //             // Success!
            //             maxValue = height;
            //             break;
            //         }
            //     }
            // }
            return (ulong)valid.Count;
        }
    }
}