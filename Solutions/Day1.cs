using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day1 : Solution
  {
    private readonly List<int> _numbers;
    private Day1() : base(1)
    {
      _numbers = new List<int>();
    }
    public static void Go()
    {
      var day1 = new Day1();
      day1.Solve();
    }

    protected override void ProcessLine(string line)
    {
      _numbers.Add(Convert.ToInt32(line));
    }

    protected override ulong SolutionA()
    {
      int previous = -1;
      ulong increases = 0;
      foreach (int number in _numbers)
      {
        int current = number;
        if ((previous >= 0) && (current > previous))
        {
          ++increases;
        }
        previous = current;
      }
      return increases;
    }
    
    protected override ulong SolutionB()
    {
      int previous = -1;
      ulong increases = 0;
      for (int i = 2; i < _numbers.Count; ++i)
      {
        int current = _numbers[i] + _numbers[i - 1] + _numbers[i - 2];
        if ((previous >= 0) && (current > previous))
        {
          ++increases;
        }
        previous = current;
      }
      return increases;
    }
  }
}