using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.ComponentModel.DataAnnotations;


namespace NRTDP.ReportConverter
{
    public class ReadTDReport : DbContext
    {
        private string _tDReport;
        public ReadTDReport(string TDReport = "C:\\Data\\Golden\\TDReports\\golden_TDPortal31.tdReport")
        {
            _tDReport = TDReport;

        }
        public DbSet<Hit> Hit { get; set; }
        public DbSet<HitScore> HitScore { get; set; }
        public DbSet<DecoyScore> DecoyScore { get; set; }
        public DbSet<LocalQualitativeConfidence> LocalQualitativeConfidence { get; set; }
        public DbSet<GlobalQualitativeConfidence> GlobalQualitativeConfidence { get; set; }
        public DbSet<ChemicalProteoform> ChemicalProteoform { get; set; }

        public DbSet<Entry> Entry { get; set; }
        public DbSet<Isoform> Isoform { get; set; }

        public DbSet<BiologicalProteoform> BiologicalProteoform { get; set; }

        public DbSet<ResultSetToScoreType> ResultSetToScoreType { get; set; }
        public DbSet<Taxon> Taxon { get; set; }

        public DbSet<DataFile> DataFile { get; set; }

        public DbSet<ResultSet> ResultSet { get; set; }

        public DbSet<AminoAcid> AminoAcid { get; set; }

        public DbSet<ResultParameter> ResultParameter { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)

            => options.UseSqlite($"Data Source={_tDReport}");



    }

    public class ResultParameter
    {
        [Key]
        public int Id { get; set; }
        public string? GroupName { get; set; }
        public string  Name { get; set; }
        public string Value { get; set; }
        public int? ResultSetId { get; set; }

    }
    public class AminoAcid
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string ExtendedSymbol { get; set; }
        public double MonoisotopicMass { get; set; }
        public double AverageMass { get; set; }
        public string Formula { get; set; }

    }

    public class Hit
    {
        public int Id { get; set; }
        public double ScoreForDecoy { get; set; }
        public double ObservedPrecursorMass { get; set; }
        public int ObservedPrecursorMassType { get; set; }
        public int ResultSetId { get; set; }
        public int DataFileId { get; set; }

        public int ChemicalProteoformId { get; set; }


    }

    public class HitScore
    {
        [Key]
        public int HitId { get; set; }

        public int ScoreTypeId { get; set; }
        public double Value { get; set; }
    }
    public class DecoyScore
    {
        [Key]
        public int Id { get; set; }

        public double Score { get; set; }
        public int ResultSetId { get; set; }
        public int AggregationLevel { get; set; }
    }
    public class LocalQualitativeConfidence
    {
        [Key]
        public int Id { get; set; }
        public int AggregationLevel { get; set; }
        public int ExternalId { get; set; }
        public int HitId { get; set; }
        public double Pvalue { get; set; }
        public double Qvalue { get; set; }
        public double PvalueCharacterized { get; set; }
        public double QvalueCharacterized { get; set; }

    }

    public class ChemicalProteoform
    {
        [Key]
        public int Id { get; set; }
        public double MonoisotopicMass { get; set; }
        public double AverageMass { get; set; }
        public int? NTerminalModificationId { get; set; }
        public int? CTerminalModificationId { get; set; }
        public string? NTerminalModificationSetId { get; set; }
        public string? CTerminalModificationSetId { get; set; }
        public string? ModificationHash { get; set; }
        public string Sequence { get; set; }



    }
    public class Entry
    {
        [Key]
        public int Id { get; set; }
        public string UniProtId { get; set; }
        public string AccessionNumber { get; set; }
        public string Description { get; set; }
        public int TaxonId { get; set; }
        public double PriorWeight { get; set; }

    }

    public class GlobalQualitativeConfidence
    {
        [Key]
        public int Id { get; set; }
        public int AggregationLevel { get; set; }
        public int ExternalId { get; set; }
        public double? GlobalQvalue { get; set; }
        public double? GlobalQvalueCharacterized { get; set; }
        public int HitId { get; set; }

    }
    public class Isoform
    {
        [Key]
        public int Id { get; set; }
        public string AccessionNumber { get; set; }
        public string Description { get; set; }
        public bool IsSubsequence { get; set; }
        public double PriorWeight { get; set; }
        public string Sequence { get; set; }
        public int EntryId { get; set; }
    }
    public class BiologicalProteoform
    {
        [Key]
        public int Id { get; set; }
        public int ProteoformRecordNum { get; set; }
        public bool IsEndogenousCleavage { get; set; }
        public string Description { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int IsoformId { get; set; }
        public int ChemicalProteoformId { get; set; }


    }
    public class ResultSetToScoreType
    {
        [Key]
        public int ResultSetId { get; set; }

        public int ScoreTypeId { get; set; }
        public int IsDefault { get; set; }
        public int IsVisible { get; set; }
    }
    public class Taxon
    {
        [Key]
        public int Id { get; set; }
        public string ScientificName { get; set; }
    }

    public class DataFile
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }

        public DateTime CreationDate { get; set; }
        public string Creator { get; set; }

        public string Description { get; set; }

    }

    public class ResultSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

    }
}
