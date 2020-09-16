using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NRTDP.ReportConverter
{
    [TestClass ]
    public class MzIdMLConverterTest
    {
        [TestMethod]
        public void FirstTest()
        {
             MzidmlWriter.ConvertToMzId(@"C:\Data\Golden\TDReports\golden_TDPortal31.tdReport", @"C:\Data\Golden\TDReports\golden.mzid");
        }
    }
}
