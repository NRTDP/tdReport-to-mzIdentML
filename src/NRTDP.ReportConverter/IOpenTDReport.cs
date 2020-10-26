using System;
using System.Collections.Generic;

namespace NRTDP.ReportConverter
{
    public interface IOpenTDReport
    {
        List<DBSequence> GetDBSequences(double FDR);
        Dictionary<string, double> GetMassTable();
        Dictionary<int, string> GetResultSets();

        Dictionary<string, string> GetResultSetParameters(int ResultSetId);
        Dictionary<string, Dictionary<string, string>> GetParameters();
        Dictionary<int, Tuple<string, string>> GetDataFiles();
        IEnumerable<BiologicalProetoform> GetBiologicalProteoforms(double FDR);
        IEnumerable<ChemicalProetoform> GetChemicalProteoforms(double FDR);
        BioMod ModLookup(int? modId, string? ModificationSetId, int startIndex, int chemId);
        public List<BioMod> ParseModHash(string ModHash, int chemId);

        Dictionary<int, Dictionary<string, IList<FragmentIon>>> GetFragmentsforHit(int hitId);


        Dictionary<int, Dictionary<int, SpectrumIdentificationItem_Hit>> CreateBatchOfHitsWithIons(int ResultSetId, int dataFileId, double FDR = 0.05);

            Dictionary<int, Dictionary<int, ProteinAmbiguityGroup>> GetproteinDetectiondata(int ResultSetId, int dataFileId, double FDR = 0.05);

        void Dispose();
    }
}
