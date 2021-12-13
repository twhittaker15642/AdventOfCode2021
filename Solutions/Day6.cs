using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day6 : Solution
  {
    private readonly ulong[] _fish;
    private Day6() : base(6)
    {
      _fish = new ulong[DAYS];
    }
    public static void Go()
    {
      var solution = new Day6();
      solution.Solve();
    }

    private const int DAYS = 9;
    private static void ProcessFish(ulong[] fish)
    {
      ulong zero = fish[0];
      for (int i = 0; i < DAYS - 1; ++i)
      {
        fish[i] = fish[i + 1];
      }
      fish[6] += zero;
      fish[8] = zero;
    }
    private static ulong Sum(ulong[] fish)
    {
      ulong sum = 0;
      for (int i = 0; i < fish.Length; ++i)
      {
        sum += fish[i];
      }
      return sum;
    }

    protected override void ProcessLine(string line)
    {
      var data = line.Split(',').Select(s => Convert.ToUInt64(s));
      foreach (var item in data)
      {
        _fish[item]++;
      }
    }
    protected override ulong SolutionA()
    {
      for (int i = 0; i < 80; ++i)
      {
        ProcessFish(_fish);
      }
      return Sum(_fish);
    }
    protected override ulong SolutionB()
    {
      for (int i = 0; i < 256; ++i)
      {
        ProcessFish(_fish);
        Console.WriteLine($"After day {i + 1}: {Sum(_fish)}");
      }
      return Sum(_fish);
    }
  }
}