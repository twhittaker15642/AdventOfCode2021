using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Solutions
{
    public class Day14 : Solution
    {
        private string _initialPattern;
        private readonly Dictionary<string, char> _insertions;
        private Day14() : base(14)
        {
            _insertions = new Dictionary<string, char>();
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
                _insertions[data[0]] = data[1][0];
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
            return Solve(10);
        }
        private ulong Solve(uint maxDepth)
        {
            // Initialize our counts dictionary
            var counts = CreateCountDictionary(_initialPattern, _insertions);
            for (int i = 0; i < _initialPattern.Length - 1; ++i)
            {
                RecursiveInsert(_initialPattern[i], _initialPattern[i + 1], 0, maxDepth, _insertions, counts, true);
            }
            counts[_initialPattern[_initialPattern.Length - 1]]++;

            var pairs = counts.Values.OrderBy(v => v).ToList();

            return pairs[pairs.Count - 1] - pairs[0];
        }
        protected override ulong SolutionB()
        {
            return Solve(40);
        }
        private static Dictionary<char, ulong> CreateCountDictionary(string initialPattern, Dictionary<string, char> insertions)
        {
            var counts = new Dictionary<char, ulong>();
            foreach (var c in initialPattern)
            {
                counts[c] = 0;
            }
            foreach (var pair in insertions)
            {
                foreach (var c in pair.Key)
                {
                    counts[c] = 0;
                }
                counts[pair.Value] = 0;
            }
            return counts;
        }
        private static void RecursiveInsert(char first, char second, uint currentDepth, uint maxDepth, Dictionary<string, char> insertions, Dictionary<char, ulong> counts, bool countIfAtMaxDepth)
        {
            if (currentDepth == maxDepth)
            {
                if (countIfAtMaxDepth)
                {
                    counts[first]++;
                    counts[second]++;
                }
                return;
            }

            string pair = $"{first}{second}";
            if (insertions.TryGetValue(pair, out char insertion))
            {
                RecursiveInsert(first, insertion, currentDepth + 1, maxDepth, insertions, counts, true);
                RecursiveInsert(insertion, second, currentDepth + 1, maxDepth, insertions, counts, false);
            }
        }
        // private static void InsertBetweenPairs(List<char> pattern, Dictionary<string, char> insertions)
        // {
        //     for (int i = 0; i < pattern.Count - 1; ++i)
        //     {
        //         string pair = $"{pattern[i]}{pattern[i + 1]}";
        //         if (insertions.TryGetValue(pair, out char insertion))
        //         {
        //             pattern.Insert(i + 1, insertion);
        //             ++i;
        //         }
        //     }
        // }
    }
}