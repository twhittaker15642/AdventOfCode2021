using System;
using System.Collections.Generic;
using System.Linq;

namespace Solutions
{
  public class Day2 : Solution
  {
    private enum Direction
    {
      forward,
      down,
      up
    }
    private readonly List<Tuple<Direction, ulong>> _instructions;


    private Day2() : base(2)
    {
      _instructions = new List<Tuple<Direction, ulong>>();
    }
    public static void Go()
    {
      var day2 = new Day2();
      day2.Solve();
    }

    protected override void ProcessLine(string line)
    {
      string[] step = line.Split(" ");
      Direction d = Enum.Parse<Direction>(step[0]);
      ulong amount = Convert.ToUInt64(step[1]);
      _instructions.Add(Tuple.Create(d, amount));      
    }

    protected override ulong SolutionA()
    {
      ulong depth = 0;
      ulong progress = 0;
      foreach (var item in _instructions)
      {
        switch (item.Item1)
        {
          case Direction.forward:
            progress += item.Item2;
            break;

          case Direction.down:
            depth += item.Item2;
            break;

          case Direction.up:
            depth -= item.Item2;
            break;

          default:
            throw new ArgumentException("WTF?");
        }
      }

      return depth*progress;
    }

    protected override ulong SolutionB()
    {
      ulong depth = 0;
      ulong progress = 0;
      ulong aim = 0;
      foreach (var item in _instructions)
      {
        switch (item.Item1)
        {
          case Direction.forward:
            progress += item.Item2;
            depth += item.Item2 * aim;
            break;

          case Direction.down:
            aim += item.Item2;
            break;

          case Direction.up:
            aim -= item.Item2;
            break;

          default:
            throw new ArgumentException("WTF?");
        }
      }

      return depth*progress;
    }
  }
}