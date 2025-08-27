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

            IProgress<double> progress = new Progress<double>(x => Console.WriteLine($"{(x * 100):N2}"));
            //MzidmlWriter.ConvertToSeperateCompressedMzId(args[0], args[1], Convert.ToDouble(args[2]));
            if ((args.Length == 1 && args[0] == "-h") || args.Length == 0)
            {
                Console.WriteLine("Usage: <TDreportPath> <outputFolder (optional)> <FDR (optional defualt:0.01 i.e. 1%)>");
            }
            if (args.Length == 3)
                MzidmlWriter.ConvertToSeperateMzId(args[0], args[1], Convert.ToDouble(args[2]), progress);
            else if (args.Length == 2)
                MzidmlWriter.ConvertToSeperateMzId(args[0], args[1], 0.01, progress);
            else if (args.Length == 2)
            {
                MzidmlWriter.ConvertToSeperateMzId(args[0], args[1], 0.01, progress);
            }
            else
                throw new ArgumentException("There must be between 1-3 arguments.");

            Console.WriteLine(sw.Elapsed);
        }
    }
}