using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class Cuboid
    {
        private readonly List<Cuboid> _subtracted;

        public Cuboid(long xmin, long xmax, long ymin, long ymax, long zmin, long zmax, bool turnOn)
        {
            XMin = xmin;
            XMax = xmax;
            YMin = ymin;
            YMax = ymax;
            ZMin = zmin;
            ZMax = zmax;
            IsValidStartup =
                XMin >= -50 && XMin <= 50 &&
                YMin >= -50 && YMin <= 50 &&
                ZMin >= -50 && ZMin <= 50 &&
                XMax >= -50 && XMax <= 50 &&
                YMax >= -50 && YMax <= 50 &&
                ZMax >= -50 && ZMax <= 50;

            TurnOn = turnOn;
            _subtracted = new List<Cuboid>();
        }
        public bool TurnOn { get; }

        public long XMin { get; }
        public long XMax { get; }
        public long YMin { get; }
        public long YMax { get; }
        public long ZMin { get; }
        public long ZMax { get; }
        public ulong CalculateVolume()
        {
            ulong volume = (ulong)((XMax - XMin + 1) * (YMax - YMin + 1) * (ZMax - ZMin + 1));

            foreach (var cuboid in _subtracted)
            {
                ulong lostVolume = cuboid.CalculateVolume();
                if (lostVolume > volume)
                    throw new Exception("Removing too much volume!");

                volume -= lostVolume;
            }

            return volume;
        }

        public bool IsValidStartup { get; }
        public void Subtract(Cuboid other)
        {
            // Get the dimensions of the subtracted cuboid
            //  If there are any axes that don't *actually* intersect, there's no true intersection here.  Just exit.
            long xmax = Math.Min(other.XMax, this.XMax);
            long xmin = Math.Max(other.XMin, this.XMin);
            if (xmax < xmin)
                return;

            long ymax = Math.Min(other.YMax, this.YMax);
            long ymin = Math.Max(other.YMin, this.YMin);
            if (ymax < ymin)
                return;

            long zmax = Math.Min(other.ZMax, this.ZMax);
            long zmin = Math.Max(other.ZMin, this.ZMin);
            if (zmax < zmin)
                return;

            // If we have a valid subtraction, subtract it from each of our other cuboids
            foreach (var cuboid in _subtracted)
            {
                cuboid.Subtract(other);
            }
            _subtracted.Add(new Cuboid(xmin, xmax, ymin, ymax, zmin, zmax, false));
        }
    }

    public class Day22 : Solution
    {
        private readonly List<Cuboid> _cuboids;
        private Day22() : base(22)
        {
            _cuboids = new List<Cuboid>();
        }
        public static void Go()
        {
            var day = new Day22();
            day.Solve();
        }

        protected override void ProcessLine(string line)
        {
            var first = line.Split(' ');

            bool turnOn = false;
            if (first[0] == "on")
            {
                turnOn = true;
            }
            else if (first[0] == "off")
            {
                turnOn = false;
            }
            else
            {
                throw new Exception($"Invalid turn on/off instruction {first[0]}");
            }

            var areas = first[1].Split(',');
            long xmin = 0, xmax = 0, ymin = 0, ymax = 0, zmin = 0, zmax = 0;

            foreach (string area in areas)
            {
                string[] coord = area.Split('=');
                string[] minmax = coord[1].Split("..");
                long min = long.Parse(minmax[0]);
                long max = long.Parse(minmax[1]);

                if (coord[0] == "x")
                {
                    xmin = min;
                    xmax = max;
                }
                else if (coord[0] == "y")
                {
                    ymin = min;
                    ymax = max;
                }
                else if (coord[0] == "z")
                {
                    zmin = min;
                    zmax = max;
                }
                else
                {
                    throw new Exception($"Invalid coord {coord[0]}");
                }
            }

            _cuboids.Add(new Cuboid(xmin, xmax, ymin, ymax, zmin, zmax, turnOn));
        }
        protected override bool RunSamples => false;
        protected override void ResetAfterSamples()
        {
            _cuboids.Clear();
        }
        protected override bool RunActuals => true;
        protected override ulong SolutionA()
        {
            // TMW 12/22/2021 -- Currently, the use of the same objects in A and B solutions causes problems...
            //  can only uncomment EITHER solution A OR solution B.
            return 0;
            // ulong answer1 = CalculateVolume(_cuboids.Where(c => c.IsValidStartup));
            // //Console.WriteLine("NEXT");
            // ulong answer2 = CountEnabled(_cuboids);
            // return answer1;
        }
        protected override ulong SolutionB()
        {
            return CalculateVolume(_cuboids);
        }

        private static ulong CalculateVolume(IEnumerable<Cuboid> enumerableCuboids)
        {
            var cuboids = new List<Cuboid>(enumerableCuboids);
            for (int i = 0; i < cuboids.Count; ++i)
            {
                var current = cuboids[i];
                for (int j = 0; j < i; ++j)
                {
                    var other = cuboids[j];
                    other.Subtract(current);
                }
                if (!current.TurnOn)
                {
                    cuboids.RemoveAt(i);
                    --i;
                }
                Console.WriteLine(SumVolumes(cuboids.Take(i + 1)));
            }

            return SumVolumes(cuboids);
        }

        private static ulong SumVolumes(IEnumerable<Cuboid> cuboids)
        {
            ulong total = 0;
            foreach (var cuboid in cuboids)
            {
                total += cuboid.CalculateVolume();
            }
            return total;
        }

        private static ulong CountEnabled(List<Cuboid> cuboids)
        {
            bool[,,] grid = new bool[101, 101, 101];

            foreach (var cuboid in cuboids)
            {
                if (!cuboid.IsValidStartup)
                    continue;

                for (long x = cuboid.XMin + 50; x <= cuboid.XMax + 50; ++x)
                {
                    for (long y = cuboid.YMin + 50; y <= cuboid.YMax + 50; ++y)
                    {
                        for (long z = cuboid.ZMin + 50; z <= cuboid.ZMax + 50; ++z)
                        {
                            grid[x, y, z] = cuboid.TurnOn;
                        }
                    }
                }

                //Console.WriteLine(CountEnabled(grid));
            }

            return CountEnabled(grid);
        }

        private static ulong CountEnabled(bool[,,] grid)
        {
            ulong total = 0;
            for (int x = 0; x < grid.GetLength(0); ++x)
            {
                for (int y = 0; y < grid.GetLength(1); ++y)
                {
                    for (int z = 0; z < grid.GetLength(2); ++z)
                    {
                        if (grid[x, y, z])
                        {
                            ++total;
                        }
                    }
                }
            }

            return total;
        }
    }
}