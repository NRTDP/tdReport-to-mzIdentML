using System;
using System.Diagnostics;

namespace NRTDP.ReportConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            MzidmlWriter.ConvertToSeperateCompressedMzId(@"C:\Data\BPA_Test\output.tdReport", @"C:\Data\BPA_Test", 0.05);
            Console.WriteLine(sw.Elapsed);
        }
    }
}
