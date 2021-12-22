using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
    public class Day20 : Solution
    {
        private string _enhance;
        private readonly List<List<char>> _image;
        private Day20() : base(20)
        {
            _image = new List<List<char>>();
        }
        public static void Go()
        {
            var day = new Day20();
            day.Solve();
        }

        protected override void ProcessLine(string line)
        {
            if (_enhance == null)
                _enhance = line;
            else if (string.IsNullOrEmpty(line))
                return;
            else
                _image.Add(new List<char>(line));
        }
        protected override bool RunSamples => true;
        protected override void ResetAfterSamples()
        {
            _enhance = null;
            _image.Clear();
        }
        protected override bool RunActuals => true;
        protected override ulong SolutionA()
        {
            var image = _image;

            // PrintImage(image, "INITIAL");

            for (int i = 0; i < 2; ++i)
            {
                image = EnhanceImage(image, _enhance);
                // PrintImage(image, $"AFTER PASS {i + 1}");
            }

            ulong count = 0;
            foreach (var line in image)
            {
                foreach (var item in line)
                {
                    if (item == '#')
                    {
                        ++count;
                    }
                }
            }
            return count;
        }
        protected override ulong SolutionB()
        {
            var image = _image;

            // PrintImage(image, "INITIAL");

            for (int i = 0; i < 50; ++i)
            {
                image = EnhanceImage(image, _enhance);
                // PrintImage(image, $"AFTER PASS {i + 1}");
            }

            ulong count = 0;
            foreach (var line in image)
            {
                foreach (var item in line)
                {
                    if (item == '#')
                    {
                        ++count;
                    }
                }
            }
            return count;
        }

        private static void PrintImage(List<List<char>> image, string text = null)
        {
            Console.WriteLine($"===START IMAGE {text ?? string.Empty}===");
            foreach (var line in image)
            {
                foreach (var item in line)
                {
                    Console.Write(item);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"===END IMAGE {text ?? string.Empty}===");
        }
        private static List<List<char>> EnhanceImage(List<List<char>> image, string enhance)
        {
            // PrintImage(image, "INITIAL");

            // Pad the image up, down, left, and right with dots
            PadImage(image, enhance);

            // PrintImage(image, "PADDING");

            // Enhance image
            var newImage = new List<List<char>>();
            for (int row = 1; row < image.Count - 1; ++row)
            {
                var line = new List<char>();
                for (int col = 1; col < image[row].Count - 1; ++col)
                {
                    int offset = GetBinaryData(row, col, image);
                    line.Add(enhance[offset]);
                }
                newImage.Add(line);
            }

            // PrintImage(newImage, "ENHANCED");

            // Unpad the image if needed
            //UnpadImage(newImage, enhance);

            //PrintImage(newImage, "UNPADDED");

            return newImage;
        }

        private static void UnpadImage(List<List<char>> image, string enhance)
        {
            if (enhance[0] == '#' && enhance[enhance.Length - 1] == '.')
            {
                image.RemoveAt(0);
                image.RemoveAt(image.Count - 1);
                foreach (var line in image)
                {
                    line.RemoveAt(0);
                    line.RemoveAt(line.Count - 1);
                }
            }
        }

        private static int GetBinaryData(int row, int col, List<List<char>> image)
        {
            int data = 0;
            for (int r = row - 1; r <= row + 1; ++r)
            {
                for (int c = col - 1; c <= col + 1; ++c)
                {
                    data <<= 1;
                    if (image[r][c] == '#')
                    {
                        data++;
                    }
                }
            }
            return data;
        }

        private static void PadImage(List<List<char>> image, string enhance)
        {
            char padding = '.';
            if (enhance[0] == '#')
            {
                // If the default value is to add '#' and the first line is all '#', our padding is '#'
                if (!image[0].Contains('.'))
                {
                    padding = '#';
                }
            }

            for (int i = 0; i < image.Count; ++i)
            {
                image[i].InsertRange(0, new string(padding, 3));
                image[i].AddRange(new string(padding, 3));
            }

            string line = new string(padding, image[0].Count);
            image.Insert(0, new List<char>(line));
            image.Insert(0, new List<char>(line));
            image.Insert(0, new List<char>(line));
            image.Add(new List<char>(line));
            image.Add(new List<char>(line));
            image.Add(new List<char>(line));
        }
    }
}