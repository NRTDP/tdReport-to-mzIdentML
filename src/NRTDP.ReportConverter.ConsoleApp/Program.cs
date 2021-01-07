
using System;
using System.Diagnostics;


namespace NRTDP.tdReportConverter.ConsoleApp
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
