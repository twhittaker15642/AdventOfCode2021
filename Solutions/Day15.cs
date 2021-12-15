using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class Location
    {
        public Location(uint value)
        {
            Value = value;
        }

        public ulong Value { get; }
        public ulong Distance { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
    public class Day15 : Solution
    {
        private readonly List<List<Location>> _risks;
        private Day15() : base(15)
        {
            _risks = new List<List<Location>>();
        }
        public static void Go()
        {
            var day = new Day15();
            day.Solve();
        }

        private const char ZERO = '0';
        protected override void ProcessLine(string line)
        {
            var row = new List<Location>();
            foreach (char c in line)
            {
                row.Add(new Location((uint)(c - ZERO)));
            }
            _risks.Add(row);
        }
        protected override bool RunSamples => true;
        protected override void ResetAfterSamples()
        {
            _risks.Clear();
        }
        protected override bool RunActuals => true;
        protected override ulong SolutionA()
        {
            return FindBestPathDijkstra(_risks);
        }
        protected override ulong SolutionB()
        {
            var newRisks = new List<List<Location>>();
            foreach (var row in _risks)
            {
                newRisks.Add(new List<Location>());
            }

            ExtendHorizontal(newRisks, _risks, 0);
            ExtendHorizontal(newRisks, _risks, 1);
            ExtendHorizontal(newRisks, _risks, 2);
            ExtendHorizontal(newRisks, _risks, 3);
            ExtendHorizontal(newRisks, _risks, 4);

            var allRisks = new List<List<Location>>();
            ExtendVertical(allRisks, newRisks, 0);
            ExtendVertical(allRisks, newRisks, 1);
            ExtendVertical(allRisks, newRisks, 2);
            ExtendVertical(allRisks, newRisks, 3);
            ExtendVertical(allRisks, newRisks, 4);

            return FindBestPathDijkstra(allRisks);
        }

        const ulong NINE = 9;
        private static Location CopyLocation(Location source, uint increment)
        {
            return new Location((uint)(source.Value + increment > NINE ? source.Value + increment - NINE : source.Value + increment));
        }
        private static void ExtendHorizontal(List<List<Location>> dest, List<List<Location>> source, uint increment)
        {
            for (int i = 0; i < source.Count; ++i)
            {
                dest[i].AddRange(source[i].Select(l => CopyLocation(l, increment)));
            }
        }

        private static void ExtendVertical(List<List<Location>> dest, List<List<Location>> source, uint increment)
        {
            for (int i = 0; i < source.Count; ++i)
            {
                dest.Add(new List<Location>(source[i].Select(l => CopyLocation(l, increment))));
            }
        }

        private static Location FindVertexWithMinimumDistance(HashSet<Location> locations)
        {
            Location location = null;
            foreach (var l in locations)
            {
                if (l.Distance < ulong.MaxValue && (location == null || location.Distance > l.Distance))
                {
                    location = l;
                }
            }
            return location;
        }
        private static void EvaluateDistance(int row, int col, Location current, List<List<Location>> risks, HashSet<Location> locations)
        {
            // Don't go out of bounds
            if (row < 0 || col < 0 || row >= risks.Count || col >= risks[0].Count)
                return;

            Location next = risks[row][col];
            if (!locations.Contains(next))
                return;

            ulong newDistance = risks[row][col].Value + current.Distance;
            if (newDistance < next.Distance)
            {
                next.Distance = newDistance;
            }
        }
        private static ulong FindBestPathDijkstra(List<List<Location>> risks)
        {
            int totalNodes = risks.Count * risks[0].Count;

            var remaining = new HashSet<Location>();
            for (int row = 0; row < risks.Count; ++row)
            {
                for (int col = 0; col < risks[row].Count; ++col)
                {
                    risks[row][col].Distance = ulong.MaxValue;
                    risks[row][col].Row = row;
                    risks[row][col].Col = col;

                    remaining.Add(risks[row][col]);
                }
            }
            risks[0][0].Distance = 0;

            while (remaining.Count > 0)
            {
                var location = FindVertexWithMinimumDistance(remaining);

                remaining.Remove(location);

                EvaluateDistance(location.Row - 1, location.Col, location, risks, remaining);
                EvaluateDistance(location.Row + 1, location.Col, location, risks, remaining);
                EvaluateDistance(location.Row, location.Col - 1, location, risks, remaining);
                EvaluateDistance(location.Row, location.Col + 1, location, risks, remaining);
            }

            return risks[risks.Count - 1][risks.Count - 1].Distance;
        }
    }
}