using System.Xml.Serialization;

namespace WatermarkMaker.Data
{
    [XmlRoot("SessionSettings")]
    public sealed class SessionSettings
    {
        [XmlElement("Version")]
        public int Version { get; set; } = 1;

        [XmlElement("Application")]
        public WindowSettings Window { get; set; } = new();

        [XmlElement("WatermarkFile")]
        public string WatermarkFilePath { get; set; } = string.Empty;

        [XmlElement("InputFolder")]
        public string InputFolderPath { get; set; } = string.Empty;

        [XmlElement("OutputFolder")]
        public string OutputFolderPath { get; set; } = string.Empty;

        [XmlElement("Proportion")]
        public double Proportion { get; set; }

        [XmlElement("RightOffset")]
        public double RightOffset { get; set; }

        [XmlElement("BottomOffset")]
        public double BottomOffset { get; set; }

        [XmlElement("BrowseInitialFolder")]
        public string BrowseInitialFolder { get; set; } = string.Empty;
    }
}