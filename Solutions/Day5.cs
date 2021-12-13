using System;
using System.Collections.Generic;
using System.Linq;

namespace Solutions
{
  public class Day5 : Solution
  {
    private const int SIZE = 1000;
    private readonly List<Tuple<Tuple<int, int>, Tuple<int, int>>> _coordinates;
    private Day5() : base(5)
    {
      _coordinates = new List<Tuple<Tuple<int, int>, Tuple<int, int>>>();
    }
    public static void Go()
    {
      var solution = new Day5();
      solution.Solve();
    }

    protected override void ProcessLine(string line)
    {
      var coordinates = line.Split(" -> ");
      var first = coordinates[0].Split(',');
      int x1 = Convert.ToInt32(first[0]);
      int y1 = Convert.ToInt32(first[1]);
      var second = coordinates[1].Split(',');
      int x2 = Convert.ToInt32(second[0]);
      int y2 = Convert.ToInt32(second[1]);
      _coordinates.Add(Tuple.Create(Tuple.Create(x1, y1), Tuple.Create(x2, y2)));
    }

    private static void MarkLine(int x1, int y1, int x2, int y2, bool useDiagonals, int[,] grid)
    {
      if (x1 == x2)
      {
        int startY = (y1 < y2 ? y1 : y2);
        int finishY = (y1 < y2 ? y2 : y1);
        for (int y = startY; y <= finishY; ++y)
        {
          grid[x1, y]++;
        }
      }
      else if (y1 == y2)
      {
        int startX = (x1 < x2 ? x1 : x2);
        int finishX = (x1 < x2 ? x2 : x1);
        for (int x = startX; x <= finishX; ++x)
        {
          grid[x, y1]++;
        }
      }
      else if (useDiagonals)
      {
        if (Math.Abs(x1 - x2) != Math.Abs(y1 - y2))
          throw new ArgumentException("Diagonal Line not right length");

        int x = -1;
        int y = -1;
        while (GetNextPointOnLine(x1, y1, x2, y2, ref x, ref y))
        {
          grid[x, y]++;
        }
      }
    }

    private static bool GetNextPointOnLine(int x1, int y1, int x2, int y2, ref int x, ref int y)
    {
      // First time in, x and y will be < 0;
      if (x < 0)
      {
        x = x1;
        y = y1;
        return true;
      }

      if (x1 > x2)
      {
        --x;
        if (x < x2)
          return false;
      }
      else
      {
        ++x;
        if (x > x2)
          return false;
      }

      if (y1 > y2)
        --y;
      else
        ++y;

      return true;
    }

    private static ulong CountIntersections(int[,] grid)
    {
      ulong intersections = 0;

      for (int x = 0; x < SIZE; ++x)
      {
        for (int y = 0; y < SIZE; ++y)
        {
          if (grid[x, y] > 1)
          {
            ++intersections;
          }
        }
      }

      return intersections;
    }

    protected override ulong SolutionA()
    {
      var grid = new int[SIZE, SIZE];

      foreach (var tuple in _coordinates)
      {
        MarkLine(
          tuple.Item1.Item1, tuple.Item1.Item2,
          tuple.Item2.Item1, tuple.Item2.Item2,
          false,
          grid
        );
      }

      return CountIntersections(grid);
    }

    protected override ulong SolutionB()
    {
      var grid = new int[SIZE, SIZE];
      
      foreach (var tuple in _coordinates)
      {
        MarkLine(
          tuple.Item1.Item1, tuple.Item1.Item2,
          tuple.Item2.Item1, tuple.Item2.Item2,
          true,
          grid
        );
      }

      return CountIntersections(grid);
    }
  }
}