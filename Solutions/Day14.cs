using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Solutions
{
    public class Day14 : Solution
    {
        private string _initialPattern;
        private readonly Dictionary<string, Tuple<string, string>> _insertions;
        private Day14() : base(14)
        {
            _insertions = new Dictionary<string, Tuple<string, string>>();
        }
        public static void Go()
        {
            var day = new Day14();
            day.Solve();
        }

        protected override void ProcessLine(string line)
        {
            var data = line.Split(" -> ");
            if (data.Length < 2)
            {
                if (line.Length > 0)
                {
                    _initialPattern = line;
                }
            }
            else
            {
                _insertions[data[0]] = Tuple.Create($"{data[0][0]}{data[1][0]}", $"{data[1][0]}{data[0][1]}");
            }
        }
        protected override bool RunSamples => true;
        protected override void ResetAfterSamples()
        {
            _insertions.Clear();
            _initialPattern = null;
        }
        protected override bool RunActuals => true;
        protected override ulong SolutionA()
        {
            return SolveFishyStyle(10);
        }
        protected override ulong SolutionB()
        {
            return SolveFishyStyle(40);
        }

        private ulong SolveFishyStyle(uint maxDepth)
        {
            var counts = new Dictionary<string, ulong>();
            for (int i = 0; i < _initialPattern.Length - 1; ++i)
            {
                string pattern = $"{_initialPattern[i]}{_initialPattern[i + 1]}";
                AddCount(counts, pattern, 1);
            }

            for (uint i = 0; i < maxDepth; ++i)
            {
                counts = UpdateCounts(counts, _insertions);
            }

            var letterCounts = new Dictionary<char, ulong>();
            foreach (var pair in counts)
            {
                if (!letterCounts.ContainsKey(pair.Key[0]))
                    letterCounts[pair.Key[0]] = 0;
                letterCounts[pair.Key[0]] += pair.Value;
                if (!letterCounts.ContainsKey(pair.Key[1]))
                    letterCounts[pair.Key[1]] = 0;
                letterCounts[pair.Key[1]] += pair.Value;
            }

            var values = letterCounts.Values.Select(v => (v + 1) / 2).OrderBy(v => v).ToList();

            return values[values.Count - 1] - values[0];
        }
        private static void AddCount(Dictionary<string, ulong> counts, string pattern, ulong amount)
        {
            if (!counts.ContainsKey(pattern))
                counts[pattern] = 0;

            counts[pattern] += amount;
        }
        private static Dictionary<string, ulong> UpdateCounts(Dictionary<string, ulong> initial, Dictionary<string, Tuple<string, string>> _insertions)
        {
            var counts = new Dictionary<string, ulong>();
            foreach (var pair in initial)
            {
                var tuple = _insertions[pair.Key];
                AddCount(counts, tuple.Item1, pair.Value);
                AddCount(counts, tuple.Item2, pair.Value);
            }
            return counts;
        }
    }
}