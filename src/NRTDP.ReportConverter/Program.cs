using System;

namespace NRTDP.ReportConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            MzidmlWriter.ConvertToSeperateMzId(@"\\resfiles.northwestern.edu\krgData\User Data\Hollas.Mike\Updated parameters\Yeast\yeast_new_parameters.tdReport", @"C:\Data\Golden\TDReports\",0.05);
        }
    }
}
