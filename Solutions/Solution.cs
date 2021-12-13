using System;
using System.Collections.Generic;
using System.IO;

namespace Solutions
{
  public abstract class Solution
  {
    private readonly uint _day;
    
    protected Solution(uint day)
    {
      _day = day;
    }

    protected abstract ulong SolutionA();
    protected abstract ulong SolutionB();
    protected abstract void ProcessLine(string line);
    protected virtual bool RunSamples => false;
    protected virtual void ResetAfterSamples()
    {
    }
    protected virtual bool RunActuals => true;

    public void Solve()
    {
      if (RunSamples && File.Exists($"Inputs/Examples/day{_day}.txt"))
      {
        foreach (string line in File.ReadLines($"Inputs/Examples/day{_day}.txt"))
        {
          ProcessLine(line);
        }
        Console.WriteLine($"Day {_day} Example A: {SolutionA()}");
        Console.WriteLine($"Day {_day} Example B: {SolutionB()}");

        ResetAfterSamples();
      }
      
      if (RunActuals && File.Exists($"Inputs/day{_day}.txt"))
      {
        foreach (string line in File.ReadLines($"Inputs/day{_day}.txt"))
        {
          ProcessLine(line);
        }

        Console.WriteLine($"Day {_day} Solution A: {SolutionA()}");
        Console.WriteLine($"Day {_day} Solution B: {SolutionB()}");
      }
    }
  }
}