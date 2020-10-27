using System;

namespace NRTDP.ReportConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            MzidmlWriter.ConvertToMzId(@"C:\Data\Golden\TDReports\golden_4.tdReport", @"C:\Data\Golden\TDReports\golden_4_Test.mzid","4",0.05);
        }
    }
}
