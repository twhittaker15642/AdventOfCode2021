using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class BingoBoard
  {
    private const int ROWS = 5;
    private readonly List<Space> _spaces;

    public BingoBoard(List<string> rows)
    {
      if (rows.Count != ROWS)
        throw new ArgumentException($"Row count is {rows.Count}");

      _spaces = new List<Space>();
      foreach (string row in rows)
      {
        var values = row.Split(' ', 5, StringSplitOptions.RemoveEmptyEntries);
        _spaces.AddRange(values.Select(value => new Space(Convert.ToInt32(value))));
      }

      if (_spaces.Count != ROWS * ROWS)
        throw new ArgumentException($"Space count is {_spaces.Count}");
    }

    public override string ToString()
    {
      string output = string.Empty;
      for (int i = 0; i < _spaces.Count; ++i)
      {
        if (i % 5 == 0)
          output += Environment.NewLine;
        
        if (_spaces[i].Marked)
          output += "*";
        output += _spaces[i].Value;
        output += " ";
      }
      return output;
    }

    public void Mark(int value)
    {
      var space = _spaces.FirstOrDefault(s => s.Value == value);
      if (space != null)
      {
        space.Marked = true;
        Winner = HasWin();
      }
    }

    public ulong Score(int lastNumberCalled)
    {
      int sum = _spaces.Where(s => !s.Marked).Select(s => s.Value).Sum();
      Console.WriteLine($"Sum: {sum}, Last Number: {lastNumberCalled}");
      return (ulong) (sum * lastNumberCalled);
    }

    public bool Winner { get; private set; }

    private bool HasWin()
    {
      // Check all rows
      for (int row = 0; row < ROWS; ++row)
      {
        int rowOffset = row * ROWS;
        bool allMarkedInRow = true;
        for (int col = 0; col < ROWS; ++col)
        {
          if (!_spaces[rowOffset + col].Marked)
          {
            allMarkedInRow = false;
            break;
          }
        }

        if (allMarkedInRow)
          return true;
      }

      // Check all columns
      for (int col = 0; col < ROWS; ++col)
      {
        bool allMarkedInColumn = true;
        for (int row = 0; row < ROWS; ++row)
        {
          int rowOffset = row * ROWS;
          if (!_spaces[col + rowOffset].Marked)
          {
            allMarkedInColumn = false;
            break;
          }
        }

        if (allMarkedInColumn)
          return true;
      }

      // Check "backslash" diagonal (0, 6, 12, 18, 24)
      bool allMarkedOnDiagonal1 = true;
      for (int row = 0; row < ROWS; ++row)
      {
        if (!_spaces[row * (ROWS + 1)].Marked)
        {
          allMarkedOnDiagonal1 = false;
          break;
        }
      }
      if (allMarkedOnDiagonal1)
        return true;

      // Check "forward slash" diagonal (4, 8, 12, 16, 20)
      bool allMarkedOnDiagonal2 = true;
      for (int row = 0; row < ROWS; ++row)
      {
        if (!_spaces[(row + 1) * (ROWS - 1)].Marked)
        {
          allMarkedOnDiagonal2 = false;
          break;
        }
      }
      if (allMarkedOnDiagonal2)
        return true;

      // No rows, columns, or diagonals were fully marked.  Not a winner
      return false;
    }

    private class Space
    {
      public Space(int value)
      {
        Value = value;
        Marked = false;
      }
      public int Value { get; }
      public bool Marked { get; set; }
    }
  }

  public class Day4 : Solution
  {
    private readonly List<int> _calls;
    private readonly List<BingoBoard> _boards;
    private readonly List<string> _lines;
    private Day4() : base(4)
    {
      _calls = new List<int>();
      _boards = new List<BingoBoard>();
      _lines = new List<string>();
    }
    public static void Go()
    {
      var solution = new Day4();
      solution.Solve();
    }

    protected override void ProcessLine(string line)
    {
      if (_calls.Count == 0)
      {
        _calls.AddRange(line.Split(",").Select(s => Convert.ToInt32(s)));
      }
      else
      {
        if (line.Trim().Length > 0)
        {
          _lines.Add(line);
          if (_lines.Count == 5)
          {
            _boards.Add(new BingoBoard(_lines));
            _lines.Clear();
          }
        }
      }
    }

    protected override ulong SolutionA()
    {
      BingoBoard winner = null;
      int lastNumberCalled = 0;
      for (int i = 0; i < _calls.Count && winner == null; ++i)
      {
        lastNumberCalled = _calls[i];
        Console.Write($"{lastNumberCalled} ");
        _boards.ForEach(b => b.Mark(lastNumberCalled));
        winner = _boards.FirstOrDefault(b => b.Winner);
      }

      Console.WriteLine();
      Console.WriteLine("WINNER!");
      Console.WriteLine(winner.ToString());

      return winner.Score(lastNumberCalled);
    }
    
    protected override ulong SolutionB()
    {
      BingoBoard winner = null;
      int lastNumberCalled = 0;
      for (int i = 0; i < _calls.Count && winner == null; ++i)
      {
        lastNumberCalled = _calls[i];
        Console.Write($"{lastNumberCalled} ");
        _boards.ForEach(b => b.Mark(lastNumberCalled));
        if (_boards.Count == 1 && _boards[0].Winner)
        {
          winner = _boards[0];
        }
        else
        {
         _boards.RemoveAll(b => b.Winner);
        }
      }

      Console.WriteLine();
      Console.WriteLine("LAST WINNER!");
      Console.WriteLine(winner.ToString());

      return winner.Score(lastNumberCalled);

    }
  }
}