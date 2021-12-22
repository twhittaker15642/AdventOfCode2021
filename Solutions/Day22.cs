using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class Cuboid
    {
        public Cuboid(string text)
        {
            IsValidStartup = true;
            var first = text.Split(' ');
            if (first[0] == "on")
            {
                TurnOn = true;
            }
            else if (first[0] == "off")
            {
                TurnOn = false;
            }
            else
            {
                throw new Exception($"Invalid turn on/off instruction {first[0]}");
            }

            var areas = first[1].Split(',');
            foreach (string area in areas)
            {
                var coord = area.Split('=');
                var minmax = coord[1].Split("..");
                int min = Int32.Parse(minmax[0]);
                if (min < -50 || min > 50)
                    IsValidStartup = false;
                int max = Int32.Parse(minmax[1]);
                if (max < -50 || max > 50)
                    IsValidStartup = false;
                if (coord[0] == "x")
                {
                    XMin = min;
                    XMax = max;
                }
                else if (coord[0] == "y")
                {
                    YMin = min;
                    YMax = max;
                }
                else if (coord[0] == "z")
                {
                    ZMin = min;
                    ZMax = max;
                }
                else
                {
                    throw new Exception($"Invalid coord {coord[0]}");
                }
            }
        }
        public bool TurnOn { get; }

        public long XMin { get; }
        public long XMax { get; }
        public long YMin { get; }
        public long YMax { get; }
        public long ZMin { get; }
        public long ZMax { get; }

        public bool IsValidStartup { get; }
    }

    public class Day22 : Solution
    {
        private readonly List<Cuboid> _instructions;
        private Day22() : base(22)
        {
            _instructions = new List<Cuboid>();
        }
        public static void Go()
        {
            var day = new Day22();
            day.Solve();
        }

        protected override void ProcessLine(string line)
        {
            _instructions.Add(new Cuboid(line));
        }
        protected override bool RunSamples => true;
        protected override void ResetAfterSamples()
        {
            _instructions.Clear();
        }
        protected override bool RunActuals => true;
        protected override ulong SolutionA()
        {
            bool[,,] grid = new bool[101, 101, 101];

            foreach (var i in _instructions)
            {
                if (!i.IsValidStartup)
                    continue;

                for (long x = i.XMin + 50; x <= i.XMax + 50; ++x)
                {
                    for (long y = i.YMin + 50; y <= i.YMax + 50; ++y)
                    {
                        for (long z = i.ZMin + 50; z <= i.ZMax + 50; ++z)
                        {
                            grid[x, y, z] = i.TurnOn;
                        }
                    }
                }
            }

            return CountEnabled(grid);
        }
        protected override ulong SolutionB()
        {
            return 0;
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