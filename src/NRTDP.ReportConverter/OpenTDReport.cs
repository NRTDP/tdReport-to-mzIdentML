using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRTDP.ReportConverter
{
    public class OpenTDReport : IDisposable
    {
        private ReadTDReport _db;
        private string _path;

        public OpenTDReport(string path)
        {
            //System.Console.WriteLine($"loading: {path}");
            _db = new ReadTDReport(path);
            _path = path;
        }

        public List<DBSequence> GetDBSequences(double FDR)
        {


            var entry_quiry = from I in _db.Isoform
                              join e in _db.Entry on I.EntryId equals e.Id
                              join bio in _db.BiologicalProteoform on I.Id equals bio.IsoformId
                              join h in _db.Hit on bio.ChemicalProteoformId equals h.ChemicalProteoformId
                              
                              join q1 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 0 } equals new { ID = q1.HitId, agg = q1.AggregationLevel }
                              join q2 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 2 } equals new { ID = q2.HitId, agg = q2.AggregationLevel }
                             join t in _db.Taxon on e.TaxonId equals t.Id into taxon
                              from t in taxon.DefaultIfEmpty()
                              where q1.GlobalQvalue < FDR && q2.GlobalQvalue < FDR
                               
                              select new DBSequence{ ID = I.Id, Accession = I.AccessionNumber, Sequence= I.Sequence, UniProtID = e.UniProtId,
                              TaxonID = e.TaxonId, SciName = t.ScientificName ?? "UnIdenitified", Description = e.Description};



            return entry_quiry.ToList();


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
            {   if (res.IsActive)
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
                        select new {groupName = para.GroupName,  Name = para.Name, Value = para.Value };

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


        public Dictionary<int,Tuple<string,string>> GetDataFiles()
        {
            var entry_quiry = from df in _db.DataFile
                              select new { ID = df.ID, Name = df.Name, filePath = df.FilePath };
                              
            var results = entry_quiry.ToList();

            var outDict = new Dictionary<int, Tuple<string, string>>();
            foreach (var res in results)
            {
                outDict[res.ID] = new Tuple<string, string>(res.Name, res.filePath);
            }

            return outDict;
        }


        public List<ChemicalProetoform> GetChemicalProteoforms(double FDR)
        {
            var entry_quiry = from h in _db.Hit
                              join c in _db.ChemicalProteoform on h.ChemicalProteoformId equals c.Id
                              join bio in _db.BiologicalProteoform on c.Id equals bio.ChemicalProteoformId
                              join I in _db.Isoform on  bio.IsoformId equals I.Id 
                             

                              join q1 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 0 } equals new { ID = q1.HitId, agg = q1.AggregationLevel }
                              join q2 in _db.GlobalQualitativeConfidence on new { ID = h.Id, agg = 2 } equals new { ID = q2.HitId, agg = q2.AggregationLevel }
                              where q1.GlobalQvalue < FDR && q2.GlobalQvalue < FDR

                              select new ChemicalProetoform {ID = c.Id,Sequence = I.Sequence, ModificationHash = c.ModificationHash, DBSequenceID = I.Id, CterminalModID = c.CTerminalModificationId, CterminalModSetID = c.CTerminalModificationSetId, NterminalModID = c.NTerminalModificationId, NterminalModSetID = c.NTerminalModificationSetId,StartIndex = bio.StartIndex, EndIndex = bio.EndIndex };

            return entry_quiry.ToList();

        }

        public void Dispose()
        {
            ((IDisposable)_db).Dispose();
        }
    }
    public class DBSequence
    {
        public int ID { get; set; }
        public string Accession { get; set; }
        public string  Sequence { get; set; }
        public string UniProtID { get; set; }
        public int TaxonID { get; set; }
        public string SciName { get; set; }
        public string Description { get; set; }


    }

    public class ChemicalProetoform
    {
        public int ID { get; set; }
        public int DBSequenceID { get; set; }
        public string? ModificationHash { get; set; }
        public string? NterminalModSetID { get; set; }

        public string? CterminalModSetID { get; set; }

        public int? NterminalModID { get; set; }

        public int? CterminalModID { get; set; }

        public string Sequence { get; set; }
        public int StartIndex { get; set; }

        public int EndIndex { get; set; }


    }
}
