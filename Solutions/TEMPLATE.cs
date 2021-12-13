using System;
using System.Linq;
using System.Collections.Generic;

namespace Solutions
{
  public class Day : Solution
  {
    private Day() : base(0)
    {
    }
    public static void Go()
    {
      var day = new Day();
      day.Solve();
    }

    protected override void ProcessLine(string line)
    {
    }
    protected override bool RunSamples => true;
    protected override void ResetAfterSamples()
    {
      
    }
    protected override bool RunActuals => false;
    protected override ulong SolutionA()
    {
      return 0;
    }
    protected override ulong SolutionB()
    {
      return 0;
    }
  }
}