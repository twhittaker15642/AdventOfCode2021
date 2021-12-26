using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class Cucumber
    {
        public const int NOMOVE = -1;
        public Cucumber(char direction)
        {
            if (direction == 'v')
            {
                MovesRight = false;
            }
            else if (direction == '>')
            {
                MovesRight = true;
            }
            else if (direction == '.')
            {
                IsBlank = true;
            }
            else
            {
                throw new Exception($"Invalid direction: {direction}");
            }

            MoveTo = NOMOVE;
        }
        public override string ToString()
        {
            if (IsBlank)
                return ".";
            else if (MovesRight)
                return ">";
            else
                return "v";
        }
        public bool MovesRight { get; }
        public bool IsBlank { get; }
        public int MoveTo {get; set;}
    }
  public class Day25 : Solution
  {
      private readonly List<List<Cucumber>> _cucumbers;
    private Day25() : base(25)
    {
        _cucumbers = new List<List<Cucumber>>();
    }
    public static void Go()
    {
      var day = new Day25();
      day.Solve();
    }

    protected override void ProcessLine(string line)
    {
        var cukes = new List<Cucumber>();
        foreach (char c in line)
        {
            cukes.Add(new Cucumber(c));
        }
        _cucumbers.Add(cukes);
    }
    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      _cucumbers.Clear();
    }
    protected override bool RunActuals => true;
    protected override ulong SolutionA()
    {
        bool anyMoved = true;
        ulong moves = 0;
        // PrintCukes(_cucumbers);
        while (anyMoved)
        {
            ++moves;
            anyMoved = MoveCukes(_cucumbers);
            if (moves % 5 == 0)
                Console.WriteLine($"{moves} moves complete");
            // if (moves == 5)
            // {
            //     PrintCukes(_cucumbers);
            //     break;
            // }
        }
      return moves;
    }
    protected override ulong SolutionB()
    {
      return 0;
    }

    private static void PrintCukes(List<List<Cucumber>> cucumbers)
    {
        foreach (var row in cucumbers)
        {
            foreach (var c in row)
            {
                Console.Write(c);
            }
            Console.WriteLine();
        }
    }
    private static bool MoveCukes(List<List<Cucumber>> cukes)
    {
        bool move = false;
        if (MoveEast(cukes))
            move = true;
        if (MoveSouth(cukes))
            move = true;
        return move;
    }
    private static bool MoveEast(List<List<Cucumber>> cukes)
    {
        bool oneMove = false;
        for (int row = 0; row < cukes.Count; ++row)
        {
            for (int col = 0; col < cukes[row].Count; ++col)
            {
                var cuke = cukes[row][col];
                if (!cuke.IsBlank && cuke.MovesRight)
                {
                    int nextCol = col + 1;
                    if (nextCol == cukes[row].Count)
                        nextCol = 0;

                    if (cukes[row][nextCol].IsBlank)
                    {
                        cukes[row][col].MoveTo = nextCol;
                        oneMove = true;
                    }
                }
            }
        }

        // Move all cukes that can
        for (int row = 0; row < cukes.Count; ++row)
        {
            for (int col = 0; col < cukes[row].Count; ++col)
            {
                var cuke = cukes[row][col];
                if (cuke.MoveTo != Cucumber.NOMOVE)
                {
                    var temp = cukes[row][cuke.MoveTo];
                    cukes[row][cuke.MoveTo] = cuke;
                    cukes[row][col] = temp;
                    cuke.MoveTo = Cucumber.NOMOVE;
                }
            }
        }

        return oneMove;
    }
    private static bool MoveSouth(List<List<Cucumber>> cukes)
    {
        bool oneMove = false;
        for (int row = 0; row < cukes.Count; ++row)
        {
            for (int col = 0; col < cukes[row].Count; ++col)
            {
                var cuke = cukes[row][col];
                if (!cuke.IsBlank && !cuke.MovesRight)
                {
                    int nextRow = row + 1;
                    if (nextRow == cukes.Count)
                        nextRow = 0;

                    if (cukes[nextRow][col].IsBlank)
                    {
                        cukes[row][col].MoveTo = nextRow;
                        oneMove = true;
                    }
                }
            }
        }

        // Move all cukes that can
        for (int row = 0; row < cukes.Count; ++row)
        {
            for (int col = 0; col < cukes[row].Count; ++col)
            {
                var cuke = cukes[row][col];
                if (cuke.MoveTo != Cucumber.NOMOVE)
                {
                    var temp = cukes[cuke.MoveTo][col];
                    cukes[cuke.MoveTo][col] = cuke;
                    cukes[row][col] = temp;
                    cuke.MoveTo = Cucumber.NOMOVE;
                }
            }
        }

        return oneMove;
    }
  }
}