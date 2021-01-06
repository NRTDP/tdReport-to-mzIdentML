
using System;
using System.Diagnostics;
using NRTDP.ReportConverter;


namespace NRTDP.ReportConverter.ConsoleApp
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            MzidmlWriter.ConvertToSeperateCompressedMzId(@"C:\Data\BPA_Test\BPA_ALL_2020_0808.tdReport", @"C:\Data\BPA_Test", 0.05);
                      
            
            Console.WriteLine(sw.Elapsed);
        }
    }
}
