using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Cave
  {
    private readonly string _name;
    private readonly List<Cave> _connections;
    private uint _visitCount;
    public Cave(string name)
    {
      _name = name;
      _connections = new List<Cave>();
      _visitCount = 0;
    }
    public void Reset()
    {
    }
    public string Name => _name;
    public void ConnectTo(Cave cave)
    {
      cave._connections.Add(this);
      this._connections.Add(cave);
    }
    public List<List<Cave>> FindPathsOut(int remainingVisitsToSmallCaves)
    {
      if ((_visitCount > 0) && !char.IsUpper(_name[0]))
      {
        if (remainingVisitsToSmallCaves <= 0 || _name == "start")
          return null;

        --remainingVisitsToSmallCaves;
      }
      
      var paths = new List<List<Cave>>();
      if (_name == "end")
        return new List<List<Cave>>() { new List<Cave>() { this } };

      // We have now visited this room one more time than previously
      ++_visitCount;

      foreach (var cave in _connections)
      {
        var next = cave.FindPathsOut(remainingVisitsToSmallCaves);
        if (next != null)
        {
          foreach (var path in next)
          {
            path.Insert(0, this);
            paths.Add(path);
          }
        }
      }

      // We are leaving this room now
      --_visitCount;

      if ((_visitCount > 0) && !char.IsUpper(_name[0]))
      {
        if (remainingVisitsToSmallCaves == 0)
          ++remainingVisitsToSmallCaves;
      }

      // Return all the paths we found
      return paths;
    }
  }

  public class Day12 : Solution
  {
    private readonly Dictionary<string, Cave> _caves;
    private Day12() : base(12)
    {
      _caves = new Dictionary<string, Cave>();
    }
    public static void Go()
    {
      var day = new Day12();
      day.Solve();
    }

    protected override void ProcessLine(string line)
    {
      var caves = line.Split('-');
      if (!_caves.TryGetValue(caves[0], out Cave cave1))
      {
        cave1 = new Cave(caves[0]);
        _caves[caves[0]] = cave1;
      }

      if (!_caves.TryGetValue(caves[1], out Cave cave2))
      {
        cave2 = new Cave(caves[1]);
        _caves[caves[1]] = cave2;
      }

      cave1.ConnectTo(cave2);
    }
    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      _caves.Clear();
    }
    protected override bool RunActuals => true;
    
    private static void PrintPaths(List<List<Cave>> paths)
    {
      foreach (var path in paths)
      {
        for (int i = 0; i < path.Count; ++i)
        {
          if (i > 0)
          {
            Console.Write("-");
          }
          Console.Write(path[i].Name);
        }
        Console.WriteLine();
      }
    }
    protected override ulong SolutionA()
    {
      var start = _caves["start"];

      Console.WriteLine("Starting");
      
      var paths = start.FindPathsOut(0);

      Console.WriteLine("Finished");
      
      //PrintPaths(paths);
      
      return (ulong) paths.Count;
    }
    protected override ulong SolutionB()
    {
      var start = _caves["start"];

      Console.WriteLine("Starting");
      
      var paths = start.FindPathsOut(1);

      Console.WriteLine("Finished");
      
      //PrintPaths(paths);
      
      return (ulong) paths.Count;
    }
  }
}