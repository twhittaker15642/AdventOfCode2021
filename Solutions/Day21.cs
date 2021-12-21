using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class DeterministicDie
    {
        private uint _value;
        public DeterministicDie()
        {
            _value = 0;
        }
        public uint Roll()
        {
            _value++;
            RollCount++;
            if (_value > 100)
            {
                _value = 1;
            }
            return _value;
        }
        public ulong RollCount { get; private set; }
    }

    public class DiracDie
    {
        private uint _value;
    }

    public class Day21 : Solution
    {
        private uint _p1start;
        private uint _p2start;
        private bool _samples;
        private bool _process;
        private Day21() : base(21)
        {
            _samples = true;
            _process = true;
        }
        public static void Go()
        {
            var day = new Day21();
            day.Solve();
        }

        protected override void ProcessLine(string line)
        {
            if (_process && _samples && RunSamples)
            {
                _p1start = 4;
                _p2start = 8;
                _samples = false;
                _process = false;
            }
            else if (_process && RunActuals)
            {
                _p1start = 7;
                _p2start = 1;
            }
        }
        protected override bool RunSamples => true;
        protected override void ResetAfterSamples()
        {
            _process = true;
        }
        protected override bool RunActuals => true;

        protected override ulong SolutionA()
        {
            uint p1 = _p1start;
            uint p2 = _p2start;

            var die = new DeterministicDie();
            ulong p1Total = 0;
            ulong p2Total = 0;

            bool p1Turn = true;
            while (p1Total < 1000 && p2Total < 1000)
            {
                if (p1Turn)
                {
                    p1 = RollAndAdvanceNTimes(3, p1, die);
                    p1Total += p1;
                }
                else
                {
                    p2 = RollAndAdvanceNTimes(3, p2, die);
                    p2Total += p2;
                }
                p1Turn = !p1Turn;
            }

            ulong result = die.RollCount;
            if (p1Total >= 1000)
            {
                result *= p2Total;
            }
            else
            {
                result *= p1Total;
            }

            return result;
        }
        protected override ulong SolutionB()
        {
            return 0;
        }

        private static uint RollAndAdvanceNTimes(uint times, uint position, DeterministicDie die)
        {
            for (uint i = 0; i < times; ++i)
            {
                position = RollAndAdvance(position, die);
            }
            return position;
        }
        private static uint RollAndAdvance(uint position, DeterministicDie die)
        {
            position += die.Roll();
            while (position > 10)
                position -= 10;
            return position;
        }
    }
}