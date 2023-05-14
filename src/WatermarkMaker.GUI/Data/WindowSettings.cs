using System.Windows;
using System.Xml.Serialization;

namespace WatermarkMaker.Data
{
    [XmlRoot("WindowSettings")]
    public sealed class WindowSettings
    {
        [XmlElement("State")]
        public WindowState State { get; set; }

        [XmlElement("Left")]
        public double Left { get; set; }

        [XmlElement("Top")]
        public double Top { get; set; }

        [XmlElement("Height")]
        public double Height { get; set; }

        [XmlElement("Width")]
        public double Width { get; set; }
    }
}