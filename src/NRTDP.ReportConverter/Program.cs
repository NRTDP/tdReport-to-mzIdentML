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
            MzidmlWriter.ConvertToSeperateCompressedMzId(@"C:\Data\Golden\TDReports\Natural Killer\2018_Allen_-_AllData_APA_DS_07_Search_2W_NK_C_josephgreer_2020-10-14-01-12-27_12902_codeset_4_0_0.tdReport", @"C:\Data\Golden\TDReports\Natural Killer", 0.05);
            Console.WriteLine(sw.Elapsed);
        }
    }
}
