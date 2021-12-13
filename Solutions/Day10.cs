using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day10 : Solution
  {
    private readonly List<string> _lines;
    private static readonly Dictionary<char, char> s_pairs;
    private static readonly Dictionary<char, ulong> s_errorScores;
    private static readonly Dictionary<char, ulong> s_incompleteScores;
    static Day10()
    {
      s_errorScores = new Dictionary<char, ulong>()
      {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 }
      };
      s_incompleteScores = new Dictionary<char, ulong>()
      {
        { ')', 1 },
        { ']', 2 },
        { '}', 3 },
        { '>', 4 }
      };
      s_pairs = new Dictionary<char, char>()
      {
        { '(', ')' },
        { '[', ']' },
        { '{', '}' },
        { '<', '>' }
      };
    }
    private Day10() : base(10)
    {
      _lines = new List<string>();
    }
    public static void Go()
    {
      var day = new Day10();
      day.Solve();
    }

    protected override void ProcessLine(string line)
    {
      _lines.Add(line);
    }
    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      _lines.Clear();
    }
    protected override bool RunActuals => true;

    private static Stack<char> GetCharactersNeededToCloseLine(string line, out int invalidPosition)
    {
      invalidPosition = -1;

      var stack = new Stack<char>();
      for (int i = 0; i < line.Length; ++i)
      {
        if (s_pairs.TryGetValue(line[i], out char expectedOutput))
        {
          // If we find the character in the _pairs array, we are looking
          //  at an input character.  Push its expected output character
          //  onto our stack for later checking.
          stack.Push(expectedOutput);
        }
        else
        {
          // If the character was NOT in _pairs array, then this is not a
          //  valid opening character.
          
          // If it does not match the last opened item on the stack,
          //  we have found an error.
          if (stack.Peek() != line[i])
          {
            invalidPosition = i;
            break;
          }

          // If it matches the proper opening, we'll pop it and move on
          stack.Pop();
        }
      }

      return (invalidPosition >= 0 ? null : stack);
    }

    protected override ulong SolutionA()
    {
      ulong sum = 0;

      for (int lineNumber = 0; lineNumber < _lines.Count; ++lineNumber)
      {
        string line = _lines[lineNumber];
        if (GetCharactersNeededToCloseLine(line, out int invalidPosition) == null)
        {
          //Console.WriteLine($"Line {lineNumber}: {invalidPosition}, {line[invalidPosition]}");
          sum += s_errorScores[line[invalidPosition]];
        }
      }

      return sum;
    }
    protected override ulong SolutionB()
    {
      List<ulong> scores = new List<ulong>();
      for (int i = 0; i < _lines.Count; ++i)
      {
        string line = _lines[i];
        
        // Skip invalid lines
        var stack = GetCharactersNeededToCloseLine(line, out int invalidPosition);
        if (stack == null)
          continue;

        // For incomplete lines, compute the score
        var closing = stack.ToArray();

        ulong score = 0;
        for (int j = 0; j < closing.Length; ++j)
        {
          score *= 5;
          score += s_incompleteScores[closing[j]];
        }
        //Console.WriteLine($"Line {i}: {score}");
        scores.Add(score);
      }

      scores.Sort();

      return scores[scores.Count / 2];
    }
  }
}