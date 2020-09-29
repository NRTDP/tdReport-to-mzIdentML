using System;

namespace NRTDP.ReportConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            MzidmlWriter.ConvertToMzId(@"C:\Data\Golden\TDReports\golden_TDPortal31.tdReport", @"C:\Data\Golden\TDReports\golden.mzid",0.05);
        }
    }
}
