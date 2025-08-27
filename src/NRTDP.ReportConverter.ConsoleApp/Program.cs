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

            IProgress<double> progress = new Progress<double>( x=> Console.WriteLine($"{(x * 100):N2}"));
            //MzidmlWriter.ConvertToSeperateCompressedMzId(args[0], args[1], Convert.ToDouble(args[2]));
            MzidmlWriter.ConvertToSeperateMzId(@"D:\Seer Dataset\2024-03-14-22-26-11_codeset_6_0_0.tdReport", @"D:\Seer Dataset", 0.01, progress);

            Console.WriteLine(sw.Elapsed);
        }
    }
}