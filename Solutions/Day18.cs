using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class SnailNumber
    {
        private SnailNumber _parent;
        private SnailNumber _left;
        private SnailNumber _right;
        private ulong _value;

        public SnailNumber() : this(null)
        {
        }
        private SnailNumber(SnailNumber parent)
        {
            _parent = parent;
        }
        private SnailNumber(SnailNumber parent, ulong value) : this(parent)
        {
            _value = value;
        }
        public SnailNumber Copy(SnailNumber parent)
        {
            var newnumber = new SnailNumber(parent);
            newnumber._left = _left?.Copy(newnumber);
            newnumber._right = _right?.Copy(newnumber);
            newnumber._value = _value;
            return newnumber;
        }
        public bool IsPair => _left != null;
        public bool IsNumber => !IsPair;

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
                _value = (ulong)text[position] - ZERO;
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
            if (depth >= 4 && IsPair && _left.IsNumber && _right.IsNumber)
            {
                _parent.AddUpAndToLeft(this, _left._value);
                _parent.AddUpAndToRight(this, _right._value);
                _left = null;
                _right = null;
                _value = 0;
                return true;
            }

            if (IsPair)
            {
                if (_left.Explode(depth + 1))
                    return true;

                if (_right.Explode(depth + 1))
                    return true;
            }

            return false;
        }
        public bool Split()
        {
            if (IsPair)
            {
                if (_left.Split())
                    return true;
                if (_right.Split())
                    return true;
            }
            else if (_value >= 10)
            {
                _left = new SnailNumber(this, _value / 2);
                _right = new SnailNumber(this, (_value + 1) / 2);
                _value = 0;
                return true;
            }

            return false;
        }
        public SnailNumber Add(SnailNumber rhs)
        {
            var number = new SnailNumber();
            number._left = this;
            number._left._parent = number;
            number._right = rhs;
            number._right._parent = number;
            return number;
        }

        public ulong CalculateMagnitude()
        {
            if (IsPair)
            {
                return 3 * _left.CalculateMagnitude() + 2 * _right.CalculateMagnitude();
            }
            else
            {
                return _value;
            }
        }

        private void AddUpAndToRight(SnailNumber source, ulong value)
        {
            if (_left == source)
            {
                _right.AddDownAndToLeft(value);
            }
            else
            {
                _parent?.AddUpAndToRight(this, value);
            }
        }
        private void AddDownAndToLeft(ulong value)
        {
            if (IsPair)
            {
                _left.AddDownAndToLeft(value);
            }
            else
            {
                _value += value;
            }
        }

        private void AddUpAndToLeft(SnailNumber source, ulong value)
        {
            if (_right == source)
            {
                _left.AddDownAndToRight(value);
            }
            else
            {
                _parent?.AddUpAndToLeft(this, value);
            }
        }
        private void AddDownAndToRight(ulong value)
        {
            if (IsPair)
            {
                _right.AddDownAndToRight(value);
            }
            else
            {
                _value += value;
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
            var number = new SnailNumber();
            int position = 0;
            number.Parse(line, ref position);
            _numbers.Add(number);
        }
        protected override bool RunSamples => true;
        protected override void ResetAfterSamples()
        {
            _numbers.Clear();
        }
        protected override bool RunActuals => true;
        protected override ulong SolutionA()
        {
            var number = AddNumbers(_numbers);

            return number.CalculateMagnitude();
        }
        protected override ulong SolutionB()
        {
            ulong max = 0;
            for (int i = 0; i < _numbers.Count; ++i)
            {
                for (int j = 0; j < _numbers.Count; ++j)
                {
                    if (i == j)
                        continue;

                    var number = AddNumbers(new List<SnailNumber>() { _numbers[i], _numbers[j] });
                    var magnitude = number.CalculateMagnitude();
                    if (magnitude > max)
                        max = magnitude;

                    number = AddNumbers(new List<SnailNumber>() { _numbers[j], _numbers[i] });
                    magnitude = number.CalculateMagnitude();
                    if (magnitude > max)
                        max = magnitude;
                }
            }

            return max;
        }

        private static SnailNumber AddNumbers(IEnumerable<SnailNumber> enumerableNumbers)
        {
            var numbers = new List<SnailNumber>();
            foreach (var number in enumerableNumbers)
            {
                numbers.Add(number.Copy(null));
            }

            while (true)
            {
                bool repeat = false;

                for (int i = 0; i < numbers.Count; ++i)
                {
                    if (numbers[i].Explode(0))
                    {
                        repeat = true;
                        break;
                    }
                }

                if (repeat)
                    continue;

                for (int i = 0; i < numbers.Count; ++i)
                {
                    if (numbers[i].Split())
                    {
                        repeat = true;
                        break;
                    }
                }

                if (repeat)
                    continue;

                if (numbers.Count == 1)
                    break;

                numbers[0] = numbers[0].Add(numbers[1]);
                numbers.RemoveAt(1);
            }

            return numbers[0];
        }
    }
}