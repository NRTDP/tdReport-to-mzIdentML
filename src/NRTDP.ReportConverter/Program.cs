using System;

namespace NRTDP.ReportConverter
{
    public sealed class MzidmlWriter: IDisposable
    {
        private XmlWriter _writer;

        public MzidmlWriter(Stream stream, Encoding encoding)
        {
            _writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = encoding, Indent = true });
        }
        public static void ConvertToMzId(string TDReport,double FDR)
        {
            string tempFilePath = Path.GetTempFileName();

            using (FileStream stream = File.Create(tempFilePath))
            using (MzmlWriter writer = new MzmlWriter(stream, Encoding.ASCII))
            {

            }
        }
        private void WriteCVParam(string accession, string name, string value = "", string unitRef = "", string unitAccession = "", string unitName = "")
        {
            this.WriteStartElement("cvParam");
            this.WriteAttributeString("cvRef", "MS");
            this.WriteAttributeString("accession", accession);
            this.WriteAttributeString("name", name);
            this.WriteAttributeString("value", value);

            if (!string.IsNullOrEmpty(unitRef))
            {
                this.WriteAttributeString("unitCvRef", unitRef);
                this.WriteAttributeString("unitAccession", unitAccession);
                this.WriteAttributeString("unitName", unitName);
            }

            this.WriteEndElement();
        }

        private void WriteAttributeString(string localName, string value)
        {
            _writer.WriteAttributeString(localName, value);
        }
        private void WriteAttributeString(string prefix, string localName, string ns, string value)
        {
            _writer.WriteAttributeString(prefix, localName, ns, value);
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
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
