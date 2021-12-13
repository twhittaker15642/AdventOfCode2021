using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day7 : Solution
  {
    private readonly List<ulong> _positions;
    private ulong _max;
    private Day7() : base(7)
    {
      _positions = new List<ulong>();
    }
    public static void Go()
    {
      var solution = new Day7();
      solution.Solve();
    }

    protected override void ProcessLine(string line)
    {
      _positions.AddRange(line.Split(',').Select(s => Convert.ToUInt64(s)));
      _max = _positions.Max();
    }

    protected override ulong SolutionA()
    {
      ulong minPosition = 0;
      ulong minFuel = ulong.MaxValue;
      for (ulong i = 0; i <= _max; ++i)
      {
        ulong sum = 0;
        foreach (ulong pos in _positions)
        {
          if (pos > i)
            sum += pos - i;
          else
            sum += i - pos;
        }
        if (sum < minFuel)
        {
          minFuel = sum;
          minPosition = i;
        }
      }
      return minFuel;
    }
    protected override ulong SolutionB()
    {
      ulong minPosition = 0;
      ulong minFuel = ulong.MaxValue;
      for (ulong i = 0; i <= _max; ++i)
      {
        ulong sum = 0;
        foreach (ulong pos in _positions)
        {
          ulong distance = 0;
          if (pos > i)
            distance = pos - i;
          else
            distance = i - pos;

          ulong even = 0;
          ulong odd = 0;
          if (distance % 2 == 0)
          {
            even = distance / 2;
            odd = distance + 1;
          }
          else
          {
            even = (distance + 1) / 2;
            odd = distance;
          }
          sum += (even * odd);
        }
        if (sum < minFuel)
        {
          minFuel = sum;
          minPosition = i;
        }
      }
      return minFuel;
    }
  }
}