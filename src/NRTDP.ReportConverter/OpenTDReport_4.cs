using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NRTDP.TDReport4;

namespace NRTDP.ReportConverter
{
    public class OpenTDReport_4 : IDisposable, IOpenTDReport
    {
        private ReadTDReport_4 _db;
        private string _path;
        private Dictionary<string, int> _scoreType = new Dictionary<string, int>();

        public OpenTDReport_4(string path)
        {
            //System.Console.WriteLine($"loading: {path}");
            _db = new ReadTDReport_4(path);
            _path = path;
            SetScoreTypeDict();
        }


        public List<DBSequence> GetDBSequences(double FDR)
        {


            var entry_quiry = from I in _db.Isoform
                              join e in _db.Entry on I.EntryId equals e.Id
                              join bio in _db.BiologicalProteoform on I.Id equals bio.IsoformId
                              join h in _db.Hit on bio.ChemicalProteoformId equals h.ChemicalProteoformId
                              join q1 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 0 } equals new { ID = q1.HitId, agg = q1.AggregationLevel }
                              join q2 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 2 } equals new { ID = q2.HitId, agg = q2.AggregationLevel }
                              where q1.GlobalQvalue < FDR && q2.GlobalQvalue < FDR

                              select new DBSequence
                              {
                                  ID = I.Id,
                                  Accession = I.AccessionNumber,
                                  Sequence = I.Sequence,
                                  UniProtID = e.UniProtId,
                                  TaxonID = e.TaxonId,
                                  SciName = "UnIdenitified", //Implement a lookup?
                                  Description = e.Description
                              };



            return entry_quiry.ToList();


        }
        public void SetScoreTypeDict()
        {
            var entry_quiry = from st in _db.ScoreType
                              select new { st.Id, st.KeyWord };

            var data = entry_quiry.ToArray();

            foreach (var scoretype in data)
            {
                _scoreType.Add(scoretype.KeyWord, scoretype.Id);
            }


        }

        public Dictionary<string, double> GetMassTable()
        {
            var outDict = new Dictionary<string, double>();
            var entry_quiry = from aa in _db.AminoAcid
                              select new { Symbol = aa.Symbol, Mass = aa.MonoisotopicMass };
            var results = entry_quiry.ToList();


            foreach (var res in results)
            {

                outDict[res.Symbol] = res.Mass;
            }
            return outDict;
        }

        public Dictionary<int, string> GetResultSets()
        {
            var outDict = new Dictionary<int, string>();
            var entry_quiry = from rs in _db.ResultSet
                              select new { ID = rs.Id, Name = rs.Name, IsActive = rs.IsActive };
            var results = entry_quiry.ToList();


            foreach (var res in results)
            {
                if (res.IsActive)
                    outDict[res.ID] = res.Name;
            }
            return outDict;
        }

        public Dictionary<string, string> GetResultSetParameters(int ResultSetId)
        {

            var quiry = from para in _db.ResultParameter
                        where para.ResultSetId == ResultSetId
                       select new { Name = para.Name, Value = para.Value };

            var results = quiry.ToList();
            
            if (results.Count == 0)
            {
                quiry = from para in _db.ResultParameter
                        join rs in _db.ResultSet on ResultSetId equals rs.Id
                        where para.GroupName == rs.Name
                        select new { Name = para.Name, Value = para.Value };

                results = quiry.ToList();
            }


            var outDict = new Dictionary<string, string>();
            foreach (var res in results)
            {

                outDict[res.Name] = res.Value;
            }
            return outDict;
        }

        public Dictionary<string, Dictionary<string, string>> GetParameters()
        {
            var quiry = from para in _db.ResultParameter
                        where para.GroupName != null
                        select new { groupName = para.GroupName, Name = para.Name, Value = para.Value };

            var results = quiry.ToList();

            var outDict = new Dictionary<string, Dictionary<string, string>>();
            foreach (var res in results)
            {
                if (!outDict.ContainsKey(res.groupName))
                {
                    outDict[res.groupName] = new Dictionary<string, string>();
                    outDict[res.groupName][res.Name] = res.Value;
                }
                else
                {
                    outDict[res.groupName][res.Name] = res.Value;
                }


            }
            return outDict;

        }


        public Dictionary<int, Tuple<string, string>> GetDataFiles()
        {
            var entry_quiry = from df in _db.DataFile
                              select new { ID = df.Id, Name = df.Name, filePath = df.FilePath };

            var results = entry_quiry.ToList();

            var outDict = new Dictionary<int, Tuple<string, string>>();
            foreach (var res in results)
            {
                outDict[res.ID] = new Tuple<string, string>(res.Name, res.filePath);
            }

            return outDict;
        }


        public IEnumerable<BiologicalProetoform> GetBiologicalProteoforms(double FDR)
        {
            var entry_quiry = from h in _db.Hit
                              join c in _db.ChemicalProteoform on h.ChemicalProteoformId equals c.Id
                              join bio in _db.BiologicalProteoform on c.Id equals bio.ChemicalProteoformId
                              join I in _db.Isoform on bio.IsoformId equals I.Id


                              join q1 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 0 } equals new { ID = q1.HitId, agg = q1.AggregationLevel }
                              join q2 in _db.GlobalQualitativeConfidence on new { ID = bio.IsoformId, agg = 2 } equals new { ID = q2.ExternalId, agg = q2.AggregationLevel }
                              where q1.GlobalQvalue < FDR && q2.GlobalQvalue < FDR
                              group new { ChemId = c.Id, IsoSequence = I.Sequence, ID = bio.Id, Sequence = c.Sequence, ModificationHash = c.ModificationHash, DBSequenceID = bio.IsoformId, CterminalModID = c.CTerminalModificationId, CterminalModSetID = c.CTerminalModificationSetId, NterminalModID = c.NTerminalModificationId, NterminalModSetID = c.NTerminalModificationSetId, StartIndex = bio.StartIndex, EndIndex = bio.EndIndex } by bio.Id into group1

                              select new BiologicalProetoform { ChemId = group1.Max(x => x.ChemId), IsoformSeqence = group1.Max(x => x.IsoSequence), ID = group1.Key, Sequence = group1.Max(x => x.Sequence), ModificationHash = group1.Max(x => x.ModificationHash), DBSequenceID = group1.Max(x => x.DBSequenceID), CterminalModID = group1.Max(x => x.CterminalModID), CterminalModSetID = group1.Max(x => x.CterminalModSetID), NterminalModID = group1.Max(x => x.NterminalModID), NterminalModSetID = group1.Max(x => x.NterminalModSetID), StartIndex = group1.Max(x => x.StartIndex), EndIndex = group1.Max(x => x.EndIndex) }
                              ;

            var output = entry_quiry.ToList();

            return output;

        }
        public IEnumerable<ChemicalProetoform> GetChemicalProteoforms(double FDR)
        {
            var entry_quiry = from h in _db.Hit
                              join c in _db.ChemicalProteoform on h.ChemicalProteoformId equals c.Id
                              join bio in _db.BiologicalProteoform on c.Id equals bio.ChemicalProteoformId
                              join I in _db.Isoform on bio.IsoformId equals I.Id


                              join q1 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 0 } equals new { ID = q1.HitId, agg = q1.AggregationLevel }
                              join q2 in _db.GlobalQualitativeConfidence on new { ID = bio.IsoformId, agg = 2 } equals new { ID = q2.ExternalId, agg = q2.AggregationLevel }
                              where q1.GlobalQvalue < FDR && q2.GlobalQvalue < FDR
                              group new { ChemId = c.Id, IsoSequence = I.Sequence, BioId = bio.Id, Sequence = c.Sequence, ModificationHash = c.ModificationHash, DBSequenceID = bio.IsoformId, CterminalModID = c.CTerminalModificationId, CterminalModSetID = c.CTerminalModificationSetId, NterminalModID = c.NTerminalModificationId, NterminalModSetID = c.NTerminalModificationSetId, StartIndex = bio.StartIndex, EndIndex = bio.EndIndex } by c.Id into group1

                              select new ChemicalProetoform { BioId = group1.Max(x => x.ChemId), IsoformSeqence = group1.Max(x => x.IsoSequence), ID = group1.Key, Sequence = group1.Max(x => x.Sequence), ModificationHash = group1.Max(x => x.ModificationHash), DBSequenceID = group1.Max(x => x.DBSequenceID), CterminalModID = group1.Max(x => x.CterminalModID), CterminalModSetID = group1.Max(x => x.CterminalModSetID), NterminalModID = group1.Max(x => x.NterminalModID), NterminalModSetID = group1.Max(x => x.NterminalModSetID), StartIndex = group1.Max(x => x.StartIndex), EndIndex = group1.Max(x => x.EndIndex) }
                              ;

            var output = entry_quiry.ToList();

            return output;

        }

        // I don;t like this! probably best to include this is the get petptides method
        public BioMod ModLookup(int? modId, string? ModificationSetId,int startIndex, int chemId)
        {
            var entry_quiry = from m in _db.Modification

                              where m.Id == modId && m.ModificationSetId == ModificationSetId
                              select new BioMod { DiffAverage = m.DiffAverage, DiffMono = m.DiffMonoisotopic, ModName = m.Name, AminoAcid=m.Residues, ModSetId = ModificationSetId, ModId = modId, StartIndex = startIndex, ChemId = chemId }
                               ;
            return entry_quiry.FirstOrDefault();

        }

        public List<BioMod> ParseModHash(string ModHash, int chemId)
        {
            List<BioMod> outModList = new List<BioMod>();

            //eg RESID:30@68|RESID:30@122

            var splitArray = ModHash.Split('|');

            foreach (var mod in splitArray)
            {
                if (mod != "")
                {
  string modSet = mod.Split(':')[0];
                string modString = mod.Split(':')[1];

                int modId = Convert.ToInt32(modString.Split('@')[0]);
                int modLoc = Convert.ToInt32(modString.Split('@')[1]);
                var modtoadd = ModLookup(modId,modSet,modLoc,chemId);

                outModList.Add(modtoadd);
                }
              

            }
            return outModList;

        }




        public Dictionary<int, Dictionary<string, IList<FragmentIon>>> GetFragmentsforHit(int hitId)
        {
            var frag_query = from h in _db.Hit
                             join f in _db.MatchingIon on h.Id equals f.HitId
                             where h.Id == hitId
                             select new FragmentIon { ObservedMz = f.ObservedMz, TheoreticalMz = f.TheoreticalMz, Charge = f.Charge, IonNumber = f.IonNumber, IonType = f.IonTypeId };

            var output = frag_query.ToList();

            var chargeIonTypeFragDict = new Dictionary<int, Dictionary<string, IList<FragmentIon>>>();

            foreach (var ion in output)
            {
                if (!chargeIonTypeFragDict.ContainsKey(ion.Charge))
                {
                    chargeIonTypeFragDict[ion.Charge] = new Dictionary<string, IList<FragmentIon>>();
                    chargeIonTypeFragDict[ion.Charge][ion.IonType] = new List<FragmentIon> { ion };
                }
                else if (!chargeIonTypeFragDict[ion.Charge].ContainsKey(ion.IonType))
                {
                    chargeIonTypeFragDict[ion.Charge][ion.IonType] = new List<FragmentIon> { ion };
                }
                else
                {
                    chargeIonTypeFragDict[ion.Charge][ion.IonType].Add(ion);
                }
            }

            return chargeIonTypeFragDict;
        }

        public Dictionary<int, Dictionary<int, SpectrumIdentificationItem_Hit>> CreateBatchOfHitsWithIons(int ResultSetId, int dataFileId, double FDR = 0.05)
        {


            //TODO Change from 2020 etc!!! this is not uniform!
            //get the hits - need Isoform ID, chemId, all bioIds, scan no - group by hit, have multi hits for different scans seperate
            var hit_quiry = from h in _db.Hit
                            join bio in _db.BiologicalProteoform on h.ChemicalProteoformId equals bio.ChemicalProteoformId
                            join c in _db.ChemicalProteoform on h.ChemicalProteoformId equals c.Id
                            join hToS in _db.HitToSpectrum on h.Id equals hToS.HitId
                            join sToSH in _db.ScanHeaderToSpectrum on hToS.SpectrumId equals sToSH.SpectrumId
                            join SH in _db.ScanHeader on sToSH.ScanHeaderId equals SH.Id

                            join ps in _db.HitScore on new { id = h.Id, type = _scoreType["kelleher_pScore"] } equals new { id = ps.HitId, type = ps.ScoreTypeId }
                            join es in _db.HitScore on new { id = h.Id, type = _scoreType["kelleher_eValue"] } equals new { id = es.HitId, type = es.ScoreTypeId }
                            join cs in _db.HitScore on new { id = h.Id, type = _scoreType["kelleher_cScore"] } equals new { id = cs.HitId, type = cs.ScoreTypeId }
                            join pcs in _db.HitScore on new { id = h.Id, type = _scoreType["kelleher_interResidueCleavages"] } equals new { id = pcs.HitId, type = pcs.ScoreTypeId }

                            join q1 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 0 } equals new { ID = q1.HitId, agg = q1.AggregationLevel }
                            join q2 in _db.GlobalQualitativeConfidence on new { ID = bio.IsoformId, agg = 2 } equals new { ID = q2.ExternalId, agg = q2.AggregationLevel }


                            where h.DataFileId == dataFileId && h.ResultSetId == ResultSetId && q1.GlobalQvalue < FDR && q2.GlobalQvalue < FDR
                            select new { gqvalue = q1.GlobalQvalue, pscore = ps.Value, escore = es.Value, cscore = cs.Value, Cleavages = pcs.Value, ChemId = c.Id, HitId = h.Id, ObsPreMass = h.ObservedPrecursorMass, TheoPreMass = c.MonoisotopicMass, IsoformId = bio.IsoformId, BioId = bio.Id, ScanNo = SH.ScanIndex }

                             ;


            var output = hit_quiry.ToList();

            var outList = new Dictionary<int, Dictionary<int, SpectrumIdentificationItem_Hit>>();

            foreach (var hitscan in output)
            {
                if (!outList.ContainsKey(hitscan.ScanNo))
                {
                    outList[hitscan.ScanNo] = new Dictionary<int, SpectrumIdentificationItem_Hit>();
                    outList[hitscan.ScanNo][hitscan.HitId] = new SpectrumIdentificationItem_Hit { GlobalQValue = hitscan.gqvalue, BioId = new HashSet<int>() { hitscan.BioId }, ChemId = hitscan.ChemId, IsoformId = new HashSet<int>() { hitscan.IsoformId }, ObsPreMass = hitscan.ObsPreMass, TheoPreMass = hitscan.TheoPreMass, Scans = new HashSet<int>() { hitscan.ScanNo }, FragmentIons = this.GetFragmentsforHit(hitscan.HitId), PScore = hitscan.pscore, CScore = hitscan.cscore, EValue = hitscan.escore, Cleavages = hitscan.Cleavages };
                }
                else if (!outList[hitscan.ScanNo].ContainsKey(hitscan.HitId))
                {
                    outList[hitscan.ScanNo][hitscan.HitId] = new SpectrumIdentificationItem_Hit { GlobalQValue = hitscan.gqvalue, BioId = new HashSet<int>() { hitscan.BioId }, ChemId = hitscan.ChemId, IsoformId = new HashSet<int>() { hitscan.IsoformId }, ObsPreMass = hitscan.ObsPreMass, TheoPreMass = hitscan.TheoPreMass, Scans = new HashSet<int>() { hitscan.ScanNo }, FragmentIons = this.GetFragmentsforHit(hitscan.HitId), PScore = hitscan.pscore, CScore = hitscan.cscore, EValue = hitscan.escore, Cleavages = hitscan.Cleavages };

                }
                else
                {
                    outList[hitscan.ScanNo][hitscan.HitId].Scans.Add(hitscan.ScanNo);
                    outList[hitscan.ScanNo][hitscan.HitId].IsoformId.Add(hitscan.IsoformId);
                    outList[hitscan.ScanNo][hitscan.HitId].BioId.Add(hitscan.BioId);
                }
            }
            //Create Dictonary and group everything
            return outList;
        }


        public Dictionary<int, Dictionary<int, ProteinAmbiguityGroup>> GetproteinDetectiondata(int ResultSetId, int dataFileId, double FDR = 0.05)
        {

            var hit_quiry = from h in _db.Hit
                            join bio in _db.BiologicalProteoform on h.ChemicalProteoformId equals bio.ChemicalProteoformId
                            join c in _db.ChemicalProteoform on h.ChemicalProteoformId equals c.Id




                            join q1 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 0 } equals new { ID = q1.HitId, agg = q1.AggregationLevel }
                            join q2 in _db.GlobalQualitativeConfidence on new { ID = bio.IsoformId, agg = 2 } equals new { ID = q2.ExternalId, agg = q2.AggregationLevel }


                            where h.DataFileId == dataFileId && h.ResultSetId == ResultSetId && q1.GlobalQvalue < FDR && q2.GlobalQvalue < FDR
                            select new { gqvalue = q2.GlobalQvalue, ChemId = c.Id, HitId = h.Id, IsoformId = bio.IsoformId, BioId = bio.Id }

                             ;


            var output = hit_quiry.ToList();

            var outList = new Dictionary<int, Dictionary<int, ProteinAmbiguityGroup>>();

            foreach (var hitscan in output)
            {
                if (!outList.ContainsKey(hitscan.IsoformId))
                {
                    outList[hitscan.IsoformId] = new Dictionary<int, ProteinAmbiguityGroup>();
                    outList[hitscan.IsoformId][hitscan.ChemId] = new ProteinAmbiguityGroup { GlobalQvalue = hitscan.gqvalue, BioId = new HashSet<int> { hitscan.BioId }, ChemId = hitscan.ChemId, IsoformId = hitscan.IsoformId, HitId = new HashSet<int> { hitscan.HitId } };
                }
                else if (!outList[hitscan.IsoformId].ContainsKey(hitscan.ChemId))
                {
                    outList[hitscan.IsoformId][hitscan.ChemId] = new ProteinAmbiguityGroup { GlobalQvalue = hitscan.gqvalue, BioId = new HashSet<int> { hitscan.BioId }, ChemId = hitscan.ChemId, IsoformId = hitscan.IsoformId, HitId = new HashSet<int> { hitscan.HitId } };

                }
                else
                {
                    outList[hitscan.IsoformId][hitscan.ChemId].HitId.Add(hitscan.HitId);
                    outList[hitscan.IsoformId][hitscan.ChemId].BioId.Add(hitscan.BioId);
                }

            }
            //Create Dictonary and group everything
            return outList;
        }


        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }
    }
}
