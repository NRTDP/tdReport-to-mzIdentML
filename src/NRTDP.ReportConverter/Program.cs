using System;

namespace NRTDP.ReportConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            MzidmlWriter.ConvertToSeperateCompressedMzId(@"C:\Data\Golden\TDReports\Yeast.tdReport", @"C:\Data\Golden\TDReports\comp", 0.05);
        }
    }
}
