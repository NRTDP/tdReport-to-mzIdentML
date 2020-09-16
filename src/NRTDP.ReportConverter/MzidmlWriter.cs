using SQLitePCL;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace NRTDP.ReportConverter
{
    public sealed class MzidmlWriter : IDisposable
    {
        private XmlWriter _writer;
        private FileInfo _TDReport;

        public MzidmlWriter(Stream stream, Encoding encoding)
        {
            _writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = encoding, Indent = true });
        }
        public static void ConvertToMzId(string TDReport, string outputPath, double FDR = 0.05)
        {
            var inputFileInfo = new FileInfo(TDReport);

            var _db = new OpenTDReport(inputFileInfo.FullName);

            // Write the opening and short xml with a single stream 
            using (FileStream stream = File.Create(outputPath))
            using (MzidmlWriter writer = new MzidmlWriter(stream, Encoding.ASCII))
            {


                writer.WriteStartDoc();
                writer.WriteMzIDStartElement(inputFileInfo.Name);
                writer.WriteMzIDCVList();
                writer.WriteAnalysisSoftwareList();
                writer.WriteProviderAndAuditCollection();
                writer.WriteSequenceCollection(_db, FDR);
                writer.WriteAnalysisCollection(_db);
            }




        }
        private void WriteCVParam(string accession, string name, string value = "", string unitRef = "", string unitAccession = "", string unitName = "")
        {
            this.WriteStartElement("cvParam");
            this.WriteAttributeString("cvRef", "PSI-MS");
            this.WriteAttributeString("accession", accession);
            this.WriteAttributeString("name", name);

            if (!string.IsNullOrEmpty(value))
                this.WriteAttributeString("value", value);

            if (!string.IsNullOrEmpty(unitRef))
            {
                this.WriteAttributeString("unitCvRef", unitRef);
                this.WriteAttributeString("unitAccession", unitAccession);
                this.WriteAttributeString("unitName", unitName);
            }

            this.WriteEndElement();
        }
        public void WriteMzIDStartElement(string name)
        {
            this.WriteStartElement("MzIdentML", "http://psi.hupo.org/ms/mzml");
            this.WriteAttributeString("id", name);
            this.WriteAttributeString("version", "1.1.0");
            this.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://psidev.info/psi/pi/mzIdentML/1.1 ../../schema/mzIdentML1.1.0.xsd");
            this.WriteAttributeString("creationDate", $"{DateTime.Now}");
        }

        public void WriteAnalysisSoftwareList()
        {
            this.WriteStartElement("AnalysisSoftwareList");

            this.WriteStartElement("AnalysisSoftware");
            this.WriteAttributeString("id", "AS_TDPortal");
            this.WriteAttributeString("name", "TDPortal");
            this.WriteAttributeString("version", "4.0.0");
            this.WriteAttributeString("uri", "http://www.kelleher.northwestern.edu/");

            this.WriteStartElement("ContactRole");
            this.WriteAttributeString("contact_ref", "ORG_NU");
            this.WriteStartElement("Role");
            this.WriteCVParam("MS:1001267", "software vendor");
            this.WriteEndElement(); //end Role
            this.WriteEndElement(); //end contact role

            this.WriteStartElement("SoftwareName");
            this.WriteCVParam("MS:100XXXX", "TDPortal"); //get Accession#
            this.WriteEndElement(); //end SoftwareName

            //TODO: Add Customizations? its optional Free text 
            //_writer.WriteString(Customizations);

            this.WriteEndElement(); //end AnalysisSoftware
            this.WriteEndElement(); //end AnalysisSoftwareList




        }

        public void WriteAnalysisCollection(OpenTDReport db)
        {
            var rawFiles = db.GetDataFiles();
            var resultSets = db.GetResultSets();
            this.WriteStartElement("AnalysisCollection");
            foreach (var ResultSet in resultSets)
            {
                //one for each result set?
                
                this.WriteStartElement("SpectrumIdentification");
                this.WriteAttributeString("id", $"SI_{ResultSet.Key}");
                this.WriteAttributeString("spectrumIdentificationProtocol_ref", $"SIP_{ResultSet.Key}");
                this.WriteAttributeString("spectrumIdentificationList_ref", $"SIL_{ResultSet.Key}");
                //this.WriteAttributeString("activityDate", $"{DateTime.Now}");

                
                // is this a list of all rawfiles?

                foreach (var raw in rawFiles)
                {
                    this.WriteStartElement("InputSpectra");
                    this.WriteAttributeString("spectraData_ref", $"SD_{raw.Key}");
                    this.WriteEndElement();


                }
                this.WriteStartElement("SearchDatabaseRef");
                this.WriteAttributeString("searchDatabase_ref", "db1");
                this.WriteEndElement();

this.WriteEndElement();
            }
            this.WriteEndElement();
        }

        public void WriteSequenceCollection(OpenTDReport db, double FDR)
        {

            this.WriteStartElement("SequenceCollection");
            var isofroms = db.GetDBSequences(0.05);

            foreach (var isform in isofroms)
            {
                WriteSingleDBSequence(isform.ID, isform.Accession, isform.Sequence, "db1", isform.Description, isform.TaxonID, isform.SciName);
            }


            var peptides = db.GetChemicalProteoforms(0.05);


            //Write Peptides
            foreach (var peptide in peptides)
            {
                this.WriteStartElement("Peptide");
                this.WriteAttributeString("id", $"Chem_{peptide.ID}");
                this.WriteStartElement("PeptideSequence");
                _writer.WriteString(peptide.Sequence);
                this.WriteEndElement();
                this.WriteEndElement();
            }

            foreach (var peptide in peptides)
            {
                char pre = '-';
                char post = '-';
                if (peptide.StartIndex != 0)
                {
                    pre = peptide.Sequence[peptide.StartIndex - 1];
                }

                if (peptide.EndIndex != peptide.Sequence.Length - 1)
                {
                    post = peptide.Sequence[peptide.Sequence.Length - 1];
                }

                this.WriteStartElement("PeptideEvidence");
                this.WriteAttributeString("id", $"PE_Chem_{peptide.ID}_ISO_{peptide.DBSequenceID}");
                this.WriteAttributeString("dBSequence_ref", $"{peptide.DBSequenceID}");
                this.WriteAttributeString("peptide_ref", $"{peptide.ID}");
                this.WriteAttributeString("start", $"{peptide.StartIndex}");
                this.WriteAttributeString("end", $"{peptide.EndIndex}");
                this.WriteAttributeString("pre", $"{pre}");
                this.WriteAttributeString("post", $"{post}");
                this.WriteAttributeString("isDecoy", $"false");
                this.WriteEndElement();
            }


            this.WriteEndElement();



        }

        public void WriteSingleDBSequence(int ID, string accession, string sequence, string searchDBRef, string proteinDescription, int taxID, string sciName)
        {
            this.WriteStartElement("DBSequence");
            this.WriteAttributeString("id", $"ISO_{ID}");
            this.WriteAttributeString("length", $"{sequence.Length}");
            this.WriteAttributeString("searchDatabase_ref", searchDBRef);
            this.WriteAttributeString("accession", accession);
            this.WriteStartElement("Seq");
            _writer.WriteString(sequence);
            this.WriteEndElement();
            this.WriteCVParam("MS:1001088", "protein description", proteinDescription);
            this.WriteCVParam("MS:1001469", "taxonomy: scientific name", sciName);
            this.WriteCVParam("MS:1001467", "taxonomy: NCBI TaxID", $"{taxID}");

            this.WriteEndElement();
        }

        public void WriteProviderAndAuditCollection()
        {

            //provider section
            this.WriteStartElement("Provider");
            this.WriteAttributeString("id", "PROVIDER");

            this.WriteStartElement("ContactRole");
            this.WriteAttributeString("contact_ref", "PERSON_DOC_OWNER");
            this.WriteStartElement("Role");
            this.WriteCVParam("MS:1001271", "researcher");
            this.WriteEndElement(); //end Role
            this.WriteEndElement(); //end contact role
            this.WriteEndElement(); //end Provider


            //Audit Collection - Could take user name - stick with Neil for now
            this.WriteStartElement("AuditCollection");
            this.WriteStartElement("Person");
            this.WriteAttributeString("id", "");
            this.WriteStartElement("Affiliation");
            this.WriteAttributeString("organization_ref", "ORG_NU");
            this.WriteEndElement(); //end 
            this.WriteEndElement(); //end person

            this.WriteStartElement("Person");
            this.WriteAttributeString("id", "PERSON_DOC_OWNER");
            this.WriteAttributeString("firstName", "Neil");
            this.WriteAttributeString("lastName", "Kelleher");
            this.WriteStartElement("Affiliation");
            this.WriteAttributeString("organization_ref", "ORG_DOC_OWNER");
            this.WriteEndElement();
            this.WriteEndElement(); //end person

            this.WriteStartElement("Organization");
            this.WriteAttributeString("id", "ORG_NU");
            this.WriteAttributeString("name", "NorthWestern University");
            this.WriteEndElement(); //end Organization

            this.WriteStartElement("Organization");
            this.WriteAttributeString("id", "ORG_DOC_OWNER");


            this.WriteEndElement(); //end Organization

            this.WriteEndElement(); //end AuditCollection

        }


        public void WriteMzIDCVList()
        {
            this.WriteStartElement("cvList");
            this.WriteAttributeString("count", "3");

            this.WriteStartElement("cv");
            this.WriteAttributeString("id", "PSI-MS");
            this.WriteAttributeString("fullName", "Proteomics Standards Initiative Mass Spectrometry Vocabularies");
            this.WriteAttributeString("version", "2.25.0");
            this.WriteAttributeString("uri", "http://psidev.cvs.sourceforge.net/viewvc/*checkout*/psidev/psi/psi-ms/mzML/controlledVocabulary/psi-ms.obo");
            this.WriteEndElement();


            this.WriteStartElement("cv");
            this.WriteAttributeString("id", "UNIMOD");
            this.WriteAttributeString("fullName", "UNIMOD");
            this.WriteAttributeString("version", "18:03:2011");
            this.WriteAttributeString("uri", "http://www.unimod.org/obo/unimod.obo");
            this.WriteEndElement();

            this.WriteStartElement("cv");
            this.WriteAttributeString("id", "UO");
            this.WriteAttributeString("fullName", "UNIT-ONTOLOGY");
            this.WriteAttributeString("uri", "http://obo.cvs.sourceforge.net/*checkout*/obo/obo/ontology/phenotype/unit.obo");
            this.WriteEndElement();


            this.WriteEndElement();
        }
        private void WriteStartDoc()
        {
            _writer.WriteStartDocument();
        }
        private void WriteAttributeString(string localName, string value)
        {
            _writer.WriteAttributeString(localName, value);
        }
        private void WriteAttributeString(string prefix, string localName, string ns, string value)
        {
            _writer.WriteAttributeString(prefix, localName, ns, value);
        }
        public void WriteMzIDEndElement()
        {
            this.WriteEndElement();
        }
        private void WriteStartElement(string localName)
        {
            _writer.WriteStartElement(localName);
        }
        private void WriteStartElement(string localName, string ns)
        {
            _writer.WriteStartElement(localName, ns);
        }
        private void WriteElementString(string localName, string value)
        {
            _writer.WriteElementString(localName, value);
        }
        private void WriteEndElement()
        {
            _writer.WriteEndElement();
        }

        public void Flush() => _writer?.Flush();

        public void Dispose() => _writer?.Dispose();

    }
}
