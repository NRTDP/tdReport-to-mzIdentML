﻿
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

            MzidmlWriter.ConvertToSeperateCompressedMzId(args[0], args[1], 0.05);
          
            
            Console.WriteLine(sw.Elapsed);
        }
    }
}
