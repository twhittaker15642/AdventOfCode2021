using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Solutions
{
    public abstract class Packet
    {
        protected Packet(uint version)
        {
            Version = version;
        }
        public uint Version { get; }
        public string DebugBits { get; private set; }
        public abstract void Parse(string bits, ref int position);
        public abstract ulong GetValue();
        public virtual ulong GetVersionTotal()
        {
            return Version;
        }
        public static ulong BinToUInt(string binary)
        {
            uint value = 0;
            foreach (char c in binary)
            {
                value <<= 1;
                if (c == '1')
                {
                    value++;
                }
            }
            return value;
        }
        public static ulong ReadBitsAsUInt(string source, ref int position, int length)
        {
            string text = source.Substring(position, length);
            position += length;
            return Packet.BinToUInt(text);
        }
        public void SetDebugBits(string bits, int start, int stop)
        {
            DebugBits = bits.Substring(start, stop - start);
        }
        public static Packet Factory(string bits, ref int position)
        {
            int initialPosition = position;
            uint version = (uint)Packet.ReadBitsAsUInt(bits, ref position, 3);
            uint type = (uint)Packet.ReadBitsAsUInt(bits, ref position, 3);

            Packet packet = null;
            switch (type)
            {
                case 0:
                    packet = new SumPacket(version);
                    break;

                case 1:
                    packet = new ProductPacket(version);
                    break;

                case 2:
                    packet = new MinimumPacket(version);
                    break;

                case 3:
                    packet = new MaximumPacket(version);
                    break;

                case 4:
                    packet = new LiteralPacket(version);
                    break;

                case 5:
                    packet = new GreaterThanPacket(version);
                    break;

                case 6:
                    packet = new LessThanPacket(version);
                    break;

                case 7:
                    packet = new EqualToPacket(version);
                    break;

                default:
                    throw new Exception($"Packet type not found: {type}");
            }

            packet.Parse(bits, ref position);
            packet.SetDebugBits(bits, initialPosition, position);
            return packet;
        }
    }

    public class LiteralPacket : Packet
    {
        private ulong _value;
        public LiteralPacket(uint version) : base(version)
        {
        }
        public override string ToString()
        {
            return _value.ToString();
        }
        public override ulong GetValue()
        {
            return _value;
        }
        public override void Parse(string bits, ref int position)
        {
            bool keepReading = true;
            var number = new StringBuilder();
            int originalPosition = position;
            while (bits.Length >= position + 5 && keepReading)
            {
                keepReading = (bits[position] == '1');
                ++position;
                number.Append(bits.Substring(position, 4));
                position += 4;
            }
            _value = BinToUInt(number.ToString());
        }
    }

    public abstract class OperatorPacket : Packet
    {
        private readonly List<Packet> _subPackets;
        protected OperatorPacket(uint version) : base(version)
        {
            _subPackets = new List<Packet>();
        }
        public override string ToString()
        {
            string output = $"Operator: ";
            foreach (var packet in _subPackets)
            {
                output += "(";
                output += packet.ToString();
                output += ") ";
            }
            return output;
        }
        public override ulong GetVersionTotal()
        {
            ulong sum = base.GetVersionTotal();
            foreach (var packet in _subPackets)
            {
                sum += packet.GetVersionTotal();
            }
            return sum;
        }
        public override void Parse(string bits, ref int position)
        {
            if (bits[position] == '0')
            {
                ++position;
                uint subPacketLengthInBits = (uint)Packet.ReadBitsAsUInt(bits, ref position, 15);
                uint endPosition = (uint)position + subPacketLengthInBits;
                while (position < endPosition)
                {
                    _subPackets.Add(Packet.Factory(bits, ref position));
                }
            }
            else
            {
                ++position;
                uint numberOfSubpackets = (uint)Packet.ReadBitsAsUInt(bits, ref position, 11);
                for (uint i = 0; i < numberOfSubpackets; ++i)
                {
                    _subPackets.Add(Packet.Factory(bits, ref position));
                }
            }
        }
        protected List<Packet> SubPackets => _subPackets;
    }

    public class SumPacket : OperatorPacket
    {
        public SumPacket(uint version) : base(version)
        {
        }
        public override string ToString()
        {
            string output = "(";
            for (int i = 0; i < SubPackets.Count; ++i)
            {
                if (i > 0)
                    output += " + ";
                output += SubPackets[i].ToString();
            }
            output += ")";
            return output;
        }
        public override ulong GetValue()
        {
            ulong sum = 0;
            foreach (var packet in SubPackets)
            {
                sum += packet.GetValue();
            }
            return sum;
        }
    }
    public class ProductPacket : OperatorPacket
    {
        public ProductPacket(uint version) : base(version)
        {
        }
        public override string ToString()
        {
            string output = "(";
            for (int i = 0; i < SubPackets.Count; ++i)
            {
                if (i > 0)
                    output += " * ";
                output += SubPackets[i].ToString();
            }
            output += ")";
            return output;
        }
        public override ulong GetValue()
        {
            ulong product = 1;
            foreach (var packet in SubPackets)
            {
                product *= packet.GetValue();
            }
            return product;
        }
    }

    public class MinimumPacket : OperatorPacket
    {
        public MinimumPacket(uint version) : base(version)
        {
        }
        public override string ToString()
        {
            string output = "MIN(";
            for (int i = 0; i < SubPackets.Count; ++i)
            {
                if (i > 0)
                    output += " , ";
                output += SubPackets[i].ToString();
            }
            output += ")";
            return output;
        }
        public override ulong GetValue()
        {
            ulong min = ulong.MaxValue;
            foreach (var packet in SubPackets)
            {
                var value = packet.GetValue();
                if (value < min)
                    min = value;
            }
            return min;
        }
    }
    public class MaximumPacket : OperatorPacket
    {
        public MaximumPacket(uint version) : base(version)
        {
        }
        public override string ToString()
        {
            string output = "MAX(";
            for (int i = 0; i < SubPackets.Count; ++i)
            {
                if (i > 0)
                    output += ",";
                output += SubPackets[i].ToString();
            }
            output += ")";
            return output;
        }
        public override ulong GetValue()
        {
            ulong max = 0;
            foreach (var packet in SubPackets)
            {
                var value = packet.GetValue();
                if (value > max)
                    max = value;
            }
            return max;
        }
    }

    public class GreaterThanPacket : OperatorPacket
    {
        public GreaterThanPacket(uint version) : base(version)
        {
        }
        public override string ToString()
        {
            return $"({SubPackets[0].ToString()} > {SubPackets[1].ToString()})";
        }
        public override ulong GetValue()
        {
            var first = SubPackets[0].GetValue();
            var second = SubPackets[1].GetValue();
            var result = (first > second ? 1U : 0);
            return result;
        }
    }

    public class LessThanPacket : OperatorPacket
    {
        public LessThanPacket(uint version) : base(version)
        {
        }
        public override string ToString()
        {
            return $"({SubPackets[0].ToString()} < {SubPackets[1].ToString()})";
        }
        public override ulong GetValue()
        {
            var first = SubPackets[0].GetValue();
            var second = SubPackets[1].GetValue();
            var result = (first < second ? 1U : 0);
            return result;
        }
    }

    public class EqualToPacket : OperatorPacket
    {
        public EqualToPacket(uint version) : base(version)
        {
        }
        public override string ToString()
        {
            return $"({SubPackets[0].ToString()} == {SubPackets[1].ToString()})";
        }
        public override ulong GetValue()
        {
            var first = SubPackets[0].GetValue();
            var second = SubPackets[1].GetValue();
            return (first == second ? 1U : 0);
        }
    }

    public class Day16 : Solution
    {
        private string _input;
        private Day16() : base(16)
        {
        }
        public static void Go()
        {
            var day = new Day16();
            day.Solve();
        }

        protected override void ProcessLine(string line)
        {
            _input = line;
        }
        protected override bool RunSamples => true;
        protected override void ResetAfterSamples()
        {
            _input = null;
        }
        protected override bool RunActuals => false;
        protected override ulong SolutionA()
        {
            string bits = HexToBits(_input);
            // Console.WriteLine(bits);

            int position = 0;
            var packet = Packet.Factory(bits, ref position);
            //Console.WriteLine(packet.ToString());

            return packet.GetVersionTotal();
        }
        protected override ulong SolutionB()
        {
            string bits = HexToBits(_input);

            int position = 0;
            var packet = Packet.Factory(bits, ref position);
            Console.WriteLine(packet.ToString());

            return packet.GetValue();
        }


        private static string HexToBits(string input)
        {
            var s = new StringBuilder();
            foreach (char c in input)
            {
                int hex = int.Parse(c.ToString(), System.Globalization.NumberStyles.HexNumber);
                var binary = Convert.ToString(hex, 2);
                s.Append('0', 4 - binary.Length);
                s.Append(binary);
            }
            return s.ToString();
        }
    }
}