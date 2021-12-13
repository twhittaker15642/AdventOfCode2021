using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day11 : Solution
  {
    private class Octopus
    {
      private uint _value;
      private bool _hasFlashed;
      private readonly List<Octopus> _neighbors;
      public Octopus(uint initialValue)
      {
        _value = initialValue;
        _hasFlashed = false;
        _neighbors = new List<Octopus>();
      }
      public uint Value => _value;
      public void AddNeighbor(Octopus neighbor)
      {
        _neighbors.Add(neighbor);
      }
      public void RaiseEnergy()
      {
        // If this item has already flashed, we don't need to raise its energy any further
        if (_hasFlashed)
          return;

        ++_value;
      }
      public bool Flash()
      {
        if (_hasFlashed)
          return false;

        if (_value <= 9)
          return false;

        _hasFlashed = true;
        _neighbors.ForEach(n => n.RaiseEnergy());
        return true;
      }
      public bool ResetFlash()
      {
        if (!_hasFlashed)
          return false;

        _value = 0;
        _hasFlashed = false;
        return true;
      }
    }

    private readonly List<List<Octopus>> _data;
    private Day11() : base(11)
    {
      _data = new List<List<Octopus>>();
    }
    public static void Go()
    {
      var day = new Day11();
      day.Solve();
    }

    const char ZERO = '0';
    protected override void ProcessLine(string line)
    {
      var list = new List<Octopus>();
      foreach (char c in line)
      {
        list.Add(new Octopus(Convert.ToUInt32(c - ZERO)));
      }
      if (_data.Count > 0 && _data[0].Count != list.Count)
        throw new Exception($"Current row has {list.Count} items instead of {_data[0].Count} items");

      _data.Add(list);
    }
    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      _data.Clear();
    }
    protected override bool RunActuals => true;

    private static void SetAllNeighbors(List<List<Octopus>> data)
    {
      for (int row = 0; row < data.Count; ++row)
      {
        for (int col = 0; col < data[row].Count; ++col)
        {
          var current = data[row][col];
          if (row > 0)
          {
            if (col > 0)
              current.AddNeighbor(data[row - 1][col - 1]);
            current.AddNeighbor(data[row - 1][col]);
            if (col < data[row].Count - 1)
              current.AddNeighbor(data[row - 1][col + 1]);
          }
          if (col > 0)
            current.AddNeighbor(data[row][col - 1]);
          if (row < data.Count - 1)
          {
            if (col > 0)
              current.AddNeighbor(data[row + 1][col - 1]);
            current.AddNeighbor(data[row + 1][col]);
            if (col < data[row].Count - 1)
              current.AddNeighbor(data[row + 1][col + 1]);
          }
          if (col < data[row].Count - 1)
            current.AddNeighbor(data[row][col + 1]);
        }
      }
    }
    private static ulong RaiseEnergy(List<List<Octopus>> data)
    {
      // Raise each items energy by 1
      foreach (var row in data)
      {
        foreach (var octopus in row)
        {
          octopus.RaiseEnergy();
        }
      }

      // Loop over the grid checking for new flashes after each pass
      //  if at least one flash has occurred, we'll try again
      bool oneFlash = true;
      while (oneFlash)
      {
        oneFlash = false;
        foreach (var row in data)
        {
          foreach (var octopus in row)
          {
            if (octopus.Flash())
              oneFlash = true;
          }
        }
      }

      // Now count the items that have flashed and reset their energy to 0
      ulong flashes = 0;
      foreach (var row in data)
      {
        foreach (var octopus in row)
        {
          if (octopus.ResetFlash())
          {
            ++flashes;
          }
        }
      }
      return flashes;
    }

    private static List<List<Octopus>> Copy(List<List<Octopus>> data)
    {
      var newData = new List<List<Octopus>>();
      foreach (var row in data)
      {
        newData.Add(new List<Octopus>(row.Select(o => new Octopus(o.Value))));
      }
      return newData;
    }

    private static void PrintGrid(int iteration, List<List<Octopus>> data)
    {
      Console.WriteLine($"After {iteration} pass:");
      foreach (var row in data)
      {
        foreach (var o in row)
        {
          Console.Write(o.Value);
        }
        Console.WriteLine();
      }
    }

    protected override ulong SolutionA()
    {
      var data = Copy(_data);
      SetAllNeighbors(data);

      ulong total = 0;
      //PrintGrid(0, _data);
      for (int i = 0; i < 100; ++i)
      {
        total += RaiseEnergy(data);
        //PrintGrid(i + 1, _data);
      }
      return total;
    }
    protected override ulong SolutionB()
    {
      var data = Copy(_data);
      SetAllNeighbors(data);

      ulong totalOctopi = (ulong) (data.Count * data[0].Count);

      ulong step = 0;
      //PrintGrid((int)step, data);

      while (true)
      {
        ulong flashes = RaiseEnergy(data);
        ++step;
        //PrintGrid((int)step, data);

        if (flashes == totalOctopi)
        {
          PrintGrid((int)step, data);
          break;
        }
      }

      return step;
    }
  }
}