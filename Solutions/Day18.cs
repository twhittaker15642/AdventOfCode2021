using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class SnailNumber
    {
        private readonly SnailNumber _parent;
        private SnailNumber _left;
        private SnailNumber _right;
        private ulong _value;

        public SnailNumber(SnailNumber parent)
        {
            _parent = parent;
        }
        public bool IsPair => _left != null;

        private const char ZERO = '0';
        public void Parse(string text, ref int position)
        {
            if (text[position] == '[')
            {
                // This is a pair.  Skip the opening bracket.
                ++position;

                // Parse the left
                _left = new SnailNumber(this);
                _left.Parse(text, ref position);

                // Skip the comma
                ++position;

                // Parse the right
                _right = new SnailNumber(this);
                _right.Parse(text, ref position);

                // Skip the closing bracket
                ++position;
            }
            else
            {
                // It's a straight value.  Load it and move past it
                _value = (ulong) text[position] - ZERO;
                ++position;
            }
        }
        public override string ToString()
        {
            if (_left == null)
            {
                return _value.ToString();
            }
            else
            {
                return $"[{_left},{_right}]";
            }
        }
        public bool Explode(ulong depth)
        {
            // If this is not a pair, it cannot explode
            if (IsNumber)
                return false;

            // We don't explode if we're not at the right depth
            //  or if one of our children is a pair
            if (depth < 4 || _left.IsPair || _right.IsPair)
            {
                if (_left.Explode(depth + 1))
                    return true;

                if (_right.Explode(depth + 1))
                    return true;
            }

            // If we're at the right depth, and we have two child numbers, we explode!
            // First, add our left value to the most adjacent number to the left.
            _parent.AddToNextLeft(this, _left._value);
            

            return true;
        }

        private void AddToNextLeft(SnailNumber source, ulong value)
        {
            if (_parent._left == this)
            {
                _parent.AddToNextLeft(source, value);
            }
            else if (_parent._left.IsPair)
            {
                _parent._left.LookDownForNumber(source, value);
            }
        }
    }

  public class Day18 : Solution
  {
      private readonly List<SnailNumber> _numbers;
    private Day18() : base(18)
    {
        _numbers = new List<SnailNumber>();
    }
    public static void Go()
    {
      var day = new Day18();
      day.Solve();
    }

    protected override void ProcessLine(string line)
    {
        var number = new SnailNumber(null);
        int position = 0;
        number.Parse(line, ref position);
        _numbers.Add(number);
        Console.WriteLine(number);
    }
    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      _numbers.Clear();
    }
    protected override bool RunActuals => false;
    protected override ulong SolutionA()
    {
        while (_numbers.Count > 1)
        {
            for (int i = 0; i < _numbers.Count; ++i)
            {
                if (_numbers[i].Explode(0))
                {
                    // Start over again
                    i = -1;
                    continue;
                }
            }
        }
      return 0;
    }
    protected override ulong SolutionB()
    {
      return 0;
    }
  }
}