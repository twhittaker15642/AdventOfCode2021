using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Solutions
{
  public class Signal
  {
    private readonly List<string> _inputs;
    private readonly List<string> _outputs;
    public Signal(IEnumerable<string> inputs, IEnumerable<string> outputs)
    {
      _inputs = new List<string>();
      foreach (var input in inputs)
      {
        if (input.Trim().Length == 0)
          continue;
        string item = String.Concat(input.OrderBy(c => c));
        _inputs.Add(item);
        //_inputs.Add(input);
      }
      _outputs = new List<string>();
      foreach (var output in outputs)
      {
        if (output.Trim().Length == 0)
          continue;
        string item = String.Concat(output.OrderBy(c => c));
        _outputs.Add(item);
        //_outputs.Add(output);
      }
    }
    public int GetOnesFoursSevensAndEightsInOutput()
    {
      return _outputs.Where(s => s.Length == 2 || s.Length == 4 || s.Length == 3 || s.Length == 7).Count();
    }
    private static void WriteMapping(StringBuilder output, Dictionary<char, char> mapping)
    {
      for (char c = 'a'; c <= 'g'; ++c)
      {
        output.AppendLine($"{c}: {mapping[c]}");
      }
    }
    private static void WriteNumbers(StringBuilder output, string[] numbers)
    {
      for (int i = 0; i < numbers.Length; ++i)
      {
        output.AppendLine($"{i}: {numbers[i]}");
      }
    }
    private const char DEFAULT = '_';
    public ulong ComputeOutput()
    {
      StringBuilder debug = new StringBuilder();
      string[] numbers = new string[10];
      numbers[1] = _inputs.First(s => s.Length == 2);
      numbers[4] = _inputs.First(s => s.Length == 4);
      numbers[7] = _inputs.First(s => s.Length == 3);
      numbers[8] = _inputs.First(s => s.Length == 7);

      // Maps correct value to current value
      Dictionary<char, char> mapping = new Dictionary<char, char>();
      mapping['a'] = DEFAULT;
      mapping['b'] = DEFAULT;
      mapping['c'] = DEFAULT;
      mapping['d'] = DEFAULT;
      mapping['e'] = DEFAULT;
      mapping['f'] = DEFAULT;
      mapping['g'] = DEFAULT;

      // Compare 7 to 1.
      //  Extra item found in 7 is a
      for (int i = 0; i < 3; i++)
      {
        if (!numbers[1].Contains(numbers[7][i]))
        {
          mapping[numbers[7][i]] = 'a';
          break;
        }
      }
      debug.AppendLine("After finding a");
      WriteMapping(debug, mapping);
      WriteNumbers(debug, numbers);

      // Compare items with 6 characters (these are 0, 6, and 9)
      //  Whichever item is missing either segment of number[1] is number[6].
      //  Also, the missing segment is C and the remaining segment of number[1] is F
      List<string> sixdigits = _inputs.Where(s => s.Length == 6).ToList();
      for (int i = 0; i < sixdigits.Count; ++i)
      {
        if (!sixdigits[i].Contains(numbers[1][0]))
        {
          numbers[6] = sixdigits[i];
          sixdigits.RemoveAt(i);
          mapping[numbers[1][0]] = 'c';
          mapping[numbers[1][1]] = 'f';
          break;
        }
        else if (!sixdigits[i].Contains(numbers[1][1]))
        {
          numbers[6] = sixdigits[i];
          sixdigits.RemoveAt(i);
          mapping[numbers[1][1]] = 'c';
          mapping[numbers[1][0]] = 'f';
          break;
        }
      }
      debug.AppendLine("After finding c and f");
      WriteMapping(debug, mapping);
      WriteNumbers(debug, numbers);

      // Compare remaining two sixdigit number (0 and 9)
      //  whichever one doesn't have both b + d (the two remaining unknowns in 4) is the 0
      //  the missing item is d, the remaining is b
      List<char> bd = numbers[4].Where(c => !numbers[1].Contains(c)).ToList();
      for (int i = 0; i < sixdigits.Count; ++i)
      {
        for (int j = 0; j < bd.Count; ++j)
        {
          if (!sixdigits[i].Contains(bd[j]))
          {
            numbers[0] = sixdigits[i];
            numbers[9] = sixdigits[(i + 1) % 2];
            mapping[bd[j]] = 'd';
            mapping[bd[(j + 1) % 2]] = 'b';
          }
        }
      }
      debug.AppendLine("After finding b and d");
      WriteMapping(debug, mapping);
      WriteNumbers(debug, numbers);

      // Look at 9 and see which segment isn't a, b, c, d, or f
      //  That segment must be g
      for (int i = 0; i < numbers[9].Length; ++i)
      {
        if (mapping[numbers[9][i]] == DEFAULT)
        {
          mapping[numbers[9][i]] = 'g';
          break;
        }
      }
      debug.AppendLine("After testing 9");
      WriteMapping(debug, mapping);
      WriteNumbers(debug, numbers);

      // Look at the 8 and see which segment isn't a, b, c, d, f, or g
      //  That segment must be e
      for (int i = 0; i < numbers[8].Length; ++i)
      {
        if (mapping[numbers[8][i]] == DEFAULT)
        {
          mapping[numbers[8][i]] = 'e';
          break;
        }
      }
      debug.AppendLine("After testing 8");
      WriteMapping(debug, mapping);
      WriteNumbers(debug, numbers);

      // Can now identify 2, 3, and 5
      // 2 will have a, XB, c, d, e, XF, g <== if it has e it is 2
      // 5 will have a, b, XC, d, XE, f, g <== if it has b it is 5
      // 3 will have a, XB, c, d, XE, f, g <== otherwise it's the 3
      var fives = _inputs.Where(s => s.Length == 5).ToList();
      foreach (string number in fives)
      {
        var e = mapping.Where(pair => pair.Value == 'e').First();
        var b = mapping.Where(pair => pair.Value == 'b').First();
        if (number.Contains(e.Key))
        {
          debug.AppendLine($"FOUND 2: {number}, {e.Key}");
          numbers[2] = number;
        }
        else if (number.Contains(b.Key))
        {
          debug.AppendLine($"FOUND 5: {number}, {b.Key}");
          numbers[5] = number;
        }
        else
        {
          debug.AppendLine($"FOUND 3: {number}, {b.Key}, {e.Key}");
          numbers[3] = number;
        }
      }
      debug.AppendLine("After 2, 3, and 5");
      WriteMapping(debug, mapping);
      WriteNumbers(debug, numbers);
      
      // Now determine what the 4 output numbers are
      int result = 0;
      foreach (string output in _outputs)
      {
        result *= 10;
        int index = Array.IndexOf(numbers, output);
        if (index < 0)
        {
          Console.Write(debug);
          throw new Exception($"Number {output} not found!");
        }

        result += index;
      }
      return (uint) result;
    }
  }
  
  public class Day8 : Solution
  {
    private readonly List<Signal> _signals;

    private Day8() : base(8)
    {
      _signals = new List<Signal>();
    }
    public static void Go()
    {
      var day = new Day8();
      day.Solve();
    }

    protected override void ProcessLine(string line)
    {
      string[] io = line.Split('|');
      string[] inputs = io[0].Split(' ');
      string[] outputs = io[1].Split(' ');
      _signals.Add(new Signal(inputs, outputs));
    }
    protected override ulong SolutionA()
    {
      int sum = 0;
      foreach (var signal in _signals)
      {
        sum += signal.GetOnesFoursSevensAndEightsInOutput();
      }
      return (ulong) sum;
    }
    protected override ulong SolutionB()
    {
      ulong sum = 0;
      foreach (var signal in _signals)
      {
        sum += signal.ComputeOutput();
      }
      return sum;
    }
  }
}