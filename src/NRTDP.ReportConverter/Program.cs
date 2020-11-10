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
            MzidmlWriter.ConvertToSeperateMzId(@"C:\Data\Golden\TDReports\golden_new_parameters.tdReport", @"C:\Data\Golden\TDReports", 0.05);
            Console.WriteLine(sw.Elapsed);
        }
    }
}
