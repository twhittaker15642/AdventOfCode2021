using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class Space
    {
        public Space(char holds = (char)0)
        {
            Holds = holds;
        }
        public override string ToString()
        {
            return OccupiedBy == null ? "." : OccupiedBy.Value.ToString();
        }
        public bool IsRoom => Holds != 0;
        public bool IsTopRoom => IsRoom && South != null;
        public Pawn OccupiedBy { get; set; }
        public char Holds { get; }
        public Space North { get; set; }
        public Space South { get; set; }
        public Space East { get; set; }
        public Space West { get; set; }
    }

    public class Pawn
    {
        private readonly char _value;
        private readonly ulong _cost;
        public Pawn(char value, Space initialSpace)
        {
            _value = value;
            switch (value)
            {
                case 'A':
                    _cost = 1;
                    break;

                case 'B':
                    _cost = 10;
                    break;

                case 'C':
                    _cost = 100;
                    break;

                case 'D':
                    _cost = 1000;
                    break;
            }
            CurrentSpace = initialSpace;
            CurrentSpace.OccupiedBy = this;
        }
        public Space CurrentSpace { get; private set; }
        public char Value => _value;
        public ulong MoveCost { get; private set; }
    }

  public class Day23 : Solution
  {
      private uint _lineCount;
      private readonly List<Space> _board;
      private readonly List<Pawn> _pawns;
    private Day23() : base(23)
    {
        _board = new List<Space>();
        for (int i = 0; i < 11; ++i)
        {
            _board.Add(new Space());
        }

        for (int i = 0; i < 10; ++i)
        {
            if (i < 9)
                _board[i].East = _board[i + 1];
            if (i > 0)
                _board[i].West = _board[i - 1];
        }

        for (char holds = 'A'; holds <= 'D'; ++holds)
        {
            var room1 = new Space(holds);
            var room2 = new Space(holds);
            var hall = _board[2 + (holds - 'A') * 2];
            room1.South = room2;
            room1.North = hall;
            hall.South = room1;
            room2.North = room1;
            _board.Add(room1);
            _board.Add(room2);
        }

        _pawns = new List<Pawn>();
    }
    public static void Go()
    {
      var day = new Day23();
      day.Solve();
    }

    protected override void ProcessLine(string line)
    {
        if (_lineCount == 2)
        {
            // Initial room 1
            _pawns.Add(new Pawn(line[3], _board.First(s => s.IsTopRoom && s.Holds == 'A')));
            _pawns.Add(new Pawn(line[5], _board.First(s => s.IsTopRoom && s.Holds == 'B')));
            _pawns.Add(new Pawn(line[7], _board.First(s => s.IsTopRoom && s.Holds == 'C')));
            _pawns.Add(new Pawn(line[9], _board.First(s => s.IsTopRoom && s.Holds == 'D')));
        }
        else if (_lineCount == 3)
        {
            // Initial room 2
            _pawns.Add(new Pawn(line[3], _board.First(s => s.IsRoom && !s.IsTopRoom && s.Holds == 'A')));
            _pawns.Add(new Pawn(line[5], _board.First(s => s.IsRoom && !s.IsTopRoom && s.Holds == 'B')));
            _pawns.Add(new Pawn(line[7], _board.First(s => s.IsRoom && !s.IsTopRoom && s.Holds == 'C')));
            _pawns.Add(new Pawn(line[9], _board.First(s => s.IsRoom && !s.IsTopRoom && s.Holds == 'D')));
        }
        ++_lineCount;
    }
    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      _lineCount = 0;
    }
    protected override bool RunActuals => false;
    protected override ulong SolutionA()
    {
        PrintBoard(_board);

        MovePawnRecursive(_pawns);

        ulong sum = 0;
        foreach (var pawn in _pawns)
        {
            sum += pawn.MoveCost;
        }
        return sum;
    }
    protected override ulong SolutionB()
    {
      return 0;
    }

    private static void MovePawnRecursive(IEnumerable<Pawn> pawns)
    {
        foreach (var pawn in pawns)
        {
            var move = pawn.Move();
            if (move != null)
            {
                MovePawnRecursive(pawns);
                pawn.MoveBack(move);
            }
        }
    }

    private static void PrintBoard(List<Space> board)
    {
        Console.WriteLine("#############");

        Console.Write('#');
        for (int i = 0; i < 11; ++i)
        {
            Console.Write(board[i]);
        }
        Console.WriteLine('#');

        Console.Write("###");
        Console.Write(board[11]);
        Console.Write("#");
        Console.Write(board[13]);
        Console.Write("#");
        Console.Write(board[15]);
        Console.Write("#");
        Console.Write(board[17]);
        Console.WriteLine("###");

        Console.Write("###");
        Console.Write(board[12]);
        Console.Write("#");
        Console.Write(board[14]);
        Console.Write("#");
        Console.Write(board[16]);
        Console.Write("#");
        Console.Write(board[18]);
        Console.WriteLine("###");

        Console.WriteLine("  #########  ");
    }
  }
}