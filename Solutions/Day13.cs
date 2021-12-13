using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day13 : Solution
  {
    private readonly List<(int X, int Y)> _points;
    private readonly List<(bool Direction, int Coordinate)> _folds;

    private Day13() : base(13)
    {
      _points = new List<(int X, int Y)>();
      _folds = new List<(bool Direction, int Coordinate)>();
    }
    public static void Go()
    {
      var day = new Day13();
      day.Solve();
    }

    protected override void ProcessLine(string line)
    {
      var point = line.Split(',');
      if (point.Length < 2)
      {
        var fold = line.Split('=');
        if (fold.Length >= 2)
        {
          bool isX = fold[0].Contains('x');
          int coordinate = Convert.ToInt32(fold[1]);
          _folds.Add((isX, coordinate));
        }
      }
      else
      {
        int x = Convert.ToInt32(point[0]);
        int y = Convert.ToInt32(point[1]);
        _points.Add((x, y));
      }
    }
    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      _points.Clear();
      _folds.Clear();
    }
    protected override bool RunActuals => true;
    protected override ulong SolutionA()
    {
      var page = BuildPage(_points);

      Console.WriteLine($"Page size: {page[0].Count} x {page.Count}");
      Console.WriteLine($"First fold: {_folds[0].Direction}, {_folds[0].Coordinate}");

      FoldPage(page, _folds[0]);


      var dots = CountDots(page);

      return dots;
    }
    protected override ulong SolutionB()
    {
      var page = BuildPage(_points);
      foreach (var fold in _folds)
      {
        FoldPage(page, fold);
      }
      PrintPage(page);
      return 0;
    }

    private static List<List<bool>> BuildPage(List<(int X, int Y)> points)
    {
      int maxX = points.Select(p => p.X).Max();
      int maxY = points.Select(p => p.Y).Max();

      var page = new List<List<bool>>();
      for (int y = 0; y <= maxY; ++y)
      {
        var row = new List<bool>(maxY + 1);
        for (int x = 0; x <= maxX; ++x)
        {
          row.Add(false);
        }
        page.Add(row);
      }

      foreach (var point in points)
      {
        page[point.Y][point.X] = true;
      }

      return page;
    }
    private static void PrintPage(List<List<bool>> page)
    {
      foreach (var row in page)
      {
        foreach (bool flag in row)
        {
          Console.Write(flag ? '#' : '.');
        }
        Console.WriteLine();
      }
    }
    private static void FoldPage(List<List<bool>> page, (bool Direction, int Coordinate) fold)
    {
      if (fold.Direction)
      {
        // Folding along X
        for (int x = fold.Coordinate + 1; x < page[0].Count; ++x)
        {
          for (int y = 0; y < page.Count; ++y)
          {
            try
            {
              var row = page[y];
              if (row.Count < page[0].Count)
              {
                Console.WriteLine($"Row {y} length {row.Count} instead of {page[0].Count}");
              }
              var data = row[x];
              if (data)
              {
                page[y][2 * fold.Coordinate - x] = true;
              }
            }
            catch
            {
              Console.WriteLine($"Failing on {x}, {y}, going to {2 * fold.Coordinate - x}, {y}");
              throw;
            }
          }
        }

        foreach (var row in page)
        {
          row.RemoveRange(fold.Coordinate, row.Count - fold.Coordinate);
        }
      }
      else
      {
        // Folding along Y
        for (int y = fold.Coordinate + 1; y < page.Count; ++y)
        {
          for (int x = 0; x < page[y].Count; ++x)
          {
            if (page[y][x])
            {
              page[2 * fold.Coordinate - y][x] = true;
            }
          }
        }

        page.RemoveRange(fold.Coordinate, page.Count - fold.Coordinate);
      }
    }
    private static ulong CountDots(List<List<bool>> page)
    {
      ulong sum = 0;
      foreach (var row in page)
      {
        foreach (bool item in row)
        {
          if (item)
            ++sum;
        }
      }
      return sum;
    }
  }
}