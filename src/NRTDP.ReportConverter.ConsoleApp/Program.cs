
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
            

            //string masterFolder = @"X:\Projects\2018 Allen - AllData";

            //List<string> foldersToSearch = new List<string>() { "DS_01", "DS_02", "DS_03", "DS_04", "DS_05", "DS_06", "DS_07", "DS_08", "DS_09", "DS_10", "DS_13", "DS_16", "DS_17", "DS_25", "DS_26", "DS_27", "DS_28", "DS_29", "DS_30", "DS_31", "DS_32", "DS_33", "DS_34", "DS_35", "DS_40", "DS_41", "DS_42", "DS_43", "DS_44", "DS_45", "DS_46", "DS_47", "DS_48", "DS_49", "DS_51", "DS_52", "DS_53", "DS_54", "DS_55", "DS_56", "DS_60", "DS_61", "DS_62", "DS_63", "DS_64", "DS_65", "DS_66", "DS_67", "DS_68", "DS_69", "DS_70", "DS_71", "DS_72", "DS_73", "DS_74", "DS_75", "DS_76", "DS_77" };

            //foreach (var expname in foldersToSearch)
            //{
            //    string thisFolder = Path.Combine(masterFolder, "APA_" + expname);


            //    DirectoryInfo topDir = new DirectoryInfo(thisFolder);

            //    var directories = topDir.GetDirectories().ToList();
                
            //    foreach (var subDir in directories)
            //    {
            //        var subDirectories = subDir.GetDirectories().ToList().Select(x=>x.Name);
            //        if (subDirectories.Contains("Results"))
            //    {
            //            var dir = new DirectoryInfo(Path.Combine(subDir.FullName, "Results"));
            //            var files = dir.GetFiles("*.tdReport");
            //        var tdreport =  $"{files.OrderByDescending(x=>x.LastWriteTime).FirstOrDefault()}";
            //        MzidmlWriter.ConvertToSeperateCompressedMzId(tdreport, dir.FullName, 0.05);


            //    }

            //    }
               

            //}


           
            
            Console.WriteLine(sw.Elapsed);
        }
    }
}
