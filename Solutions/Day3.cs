using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day3 : Solution
  {

    private readonly List<string> _lines;
    public Day3() : base(3)
    {
      _lines = new List<string>();
    }

    public static void Go()
    {
      var solution = new Day3();
      solution.Solve();
    }

    private const char ONE = '1';
    private const char ZERO = '0';
    private static char GetMostCommonValueAtPosition(List<string> data, int position)
    {
      uint ones = 0;
      uint zeroes = 0;

      foreach (string bits in data)
      {
        if (bits[position] == ONE)
        {
          ++ones;
        }
        else
        {
          ++zeroes;
        }
      }

      if (ones == zeroes)
        return ONE;

      return (ones > zeroes ? ONE : ZERO);
    }

    protected override void ProcessLine(string line)
    {
      _lines.Add(line);
    }

    protected override ulong SolutionA()
    {
      int gamma = 0;
      int epsilon = 0;
      for (int i = 0; i < _lines[0].Length; ++i)
      {
        char common = GetMostCommonValueAtPosition(_lines, i);
        gamma <<= 1;
        epsilon <<= 1;
        if (common == ONE)
        {
          gamma |= 0x1;
        }
        else
        {
          epsilon |= 0x1;
        }

        Console.WriteLine($"Total Count: {_lines.Count}, Common: {common}, Gamma: {gamma}, Epsilon: {epsilon}, And: { gamma & epsilon }, Or: {gamma | epsilon} ");
      }
      
      return (ulong) (gamma * epsilon);
    }

    protected override ulong SolutionB()
    {
      List<string> commonValues = new List<string>(_lines);
      List<string> uncommonValues = new List<string>(_lines);

      int length = commonValues[0].Length;
      for (int i = 0; i < length; ++i)
      {
        // Status(i.ToString(), commonValues, uncommonValues);

        char common1 = GetMostCommonValueAtPosition(commonValues, i);
        commonValues = Filter(commonValues, i, common1);

        char common2 = GetMostCommonValueAtPosition(uncommonValues, i);
        uncommonValues = Filter(uncommonValues, i, common2 == ONE ? ZERO : ONE);
      }

      if (commonValues.Count > 1)
        throw new Exception("COMMON VALUES TOO LONG");
      if (uncommonValues.Count > 1)
        throw new Exception("UNCOMMON VALUES TOO LONG");

      int common = 0;
      int uncommon = 0;
      for (int i = 0; i < commonValues[0].Length; ++i)
      {
        common = AddBit(common, commonValues[0][i]);
        uncommon = AddBit(uncommon, uncommonValues[0][i]);
      }

      return (ulong) (common * uncommon);
    }

    private int AddBit(int starting, char value)
    {
      starting <<= 1;
      if (value == ONE)
        starting |= 0x1;
      return starting;
    }

    private void Status(string prefix, List<string> common, List<string> uncommon)
    {
      Console.WriteLine(prefix);
      Console.WriteLine("COMMON");
      WriteList(common);
      Console.WriteLine("UNCOMMON");
      WriteList(uncommon);
    }

    private void WriteList(List<string> items)
    {
      if (items.Count <= 6)
      {
        foreach (string item in items)
        {
          Console.WriteLine($"\t{item}");
        }
      }
      else
      {
        Console.WriteLine($"{items.Count} items");
      }
    }

    private List<string> Filter(List<string> values, int bitPosition, char expected)
    {
      if (values.Count == 1)
        return values;

      return values.Where(v => v[bitPosition] == expected).ToList();
    }

  }
}