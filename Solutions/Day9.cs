using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day9 : Solution
  {
    private readonly List<List<ulong>> _map;
    private Day9() : base(9)
    {
      _map = new List<List<ulong>>();
    }
    public static void Go()
    {
      var day = new Day9();
      day.Solve();
    }

    private const char ZERO = '0';
    protected override void ProcessLine(string line)
    {
      _map.Add(line.Select(c => (ulong)(c - ZERO)).ToList());
    }

    private static List<Tuple<int, int>> FindLowPoints(List<List<ulong>> map)
    {
      var lows = new List<Tuple<int, int>>();
      for (int row = 0; row < map.Count; ++row)
      {
        for (int col = 0; col < map[row].Count; ++col)
        {
          ulong current = map[row][col];

          // First column, don't compare to left
          if (col > 0 && current >= map[row][col - 1])
          {
            continue;
          }

          // Last column, don't compare to right
          if (col < map[row].Count - 1 && current >= map[row][col + 1])
          {
            continue;
          }

          // First row, don't compare above
          if (row > 0 && current >= map[row - 1][col])
          {
            continue;
          }

          // Last row, don't compare below
          if (row < map.Count - 1 && current >= map[row + 1][col])
          {
            continue;
          }

          lows.Add(Tuple.Create(row, col));
        }
      }
      return lows;
    }
    private static ulong GetRiskLevel(Tuple<int, int> point, List<List<ulong>> map)
    {
      //Console.WriteLine($"({point.Item1}, {point.Item2}): {map[point.Item1][point.Item2]}");
      return map[point.Item1][point.Item2] + 1;
    }

    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      _map.Clear();
    }
    
    protected override ulong SolutionA()
    {
      var lows = FindLowPoints(_map);
      //Console.WriteLine($"Lows: {lows.Count}");
      ulong sum = 0;
      foreach (var tuple in lows)
      {
        sum += GetRiskLevel(tuple, _map);
      }
      return sum;
    }

    private static ulong GetBasinSize(Tuple<int, int> low, List<List<ulong>> map)
    {
      var basin = new List<Tuple<int, int>>() { low };
      //Console.WriteLine($"Finding basin for ({low.Item1}, {low.Item2})");
      while (true)
      {
        int originalSize = basin.Count;
        for (int i = 0; i < originalSize; ++i)
        {
          var point = basin.ElementAt(i);
          ExtendLeft(point, map, basin);
        }
        //Console.WriteLine($"Basin is size {basin.Count}");
        for (int i = 0; i < originalSize; ++i)
        {
            var point = basin.ElementAt(i);
            ExtendRight(point, map, basin);
        }
        //Console.WriteLine($"Basin is size {basin.Count}");
        for (int i = 0; i < originalSize; ++i)
        {
            var point = basin.ElementAt(i);
            ExtendUp(point, map, basin);
        }
        //Console.WriteLine($"Basin is size {basin.Count}");
        for (int i = 0; i < originalSize; ++i)
        {
            var point = basin.ElementAt(i);
            ExtendDown(point, map, basin);
        }
        //Console.WriteLine($"Basin is size {basin.Count}");
        if (originalSize == basin.Count)
            break;
      }

      return (ulong) basin.Count;
    }

    private static void AddToList(Tuple<int, int> point, List<Tuple<int, int>> basin)
    {
      if (basin.FirstOrDefault(t => t.Item1 == point.Item1 && t.Item2 == point.Item2) == null)
      {
          basin.Add(point);
      }
    }
    private static void ExtendLeft(Tuple<int, int> point, List<List<ulong>> map, List<Tuple<int, int>> basin)
    {
      for (int i = point.Item1 - 1; i >= 0 && map[i][point.Item2] != 9; --i)
      {
        AddToList(Tuple.Create(i, point.Item2), basin);
      }
    }
    private static void ExtendRight(Tuple<int, int> point, List<List<ulong>> map, List<Tuple<int, int>> basin)
    {
      for (int i = point.Item1 + 1; i < map.Count && map[i][point.Item2] != 9; ++i)
      {
        AddToList(Tuple.Create(i, point.Item2), basin);
      }
    }
    private static void ExtendUp(Tuple<int, int> point, List<List<ulong>> map, List<Tuple<int, int>> basin)
    {
      for (int i = point.Item2 - 1; i >= 0 && map[point.Item1][i] != 9; --i)
      {
        AddToList(Tuple.Create(point.Item1, i), basin);
      }
    }
    private static void ExtendDown(Tuple<int, int> point, List<List<ulong>> map, List<Tuple<int, int>> basin)
    {
      for (int i = point.Item2 + 1; i < map[0].Count && map[point.Item1][i] != 9; ++i)
      {
        AddToList(Tuple.Create(point.Item1, i), basin);
      }
    }

    protected override ulong SolutionB()
    {
      var lows = FindLowPoints(_map);

      var basins = new List<Tuple<Tuple<int, int>, ulong>>();
      foreach (var tuple in lows)
      {
        ulong basinSize = GetBasinSize(tuple, _map);
        Console.WriteLine($"({tuple.Item1}, {tuple.Item2}): {basinSize}");
        basins.Add(Tuple.Create(tuple, basinSize));
      }

      // Get largest 3 basins
      ulong product = 1;
      foreach (var basin in basins.OrderByDescending(b => b.Item2).Take(3))
      {
        product *= basin.Item2;
      }
      return product;
    }
  }
}