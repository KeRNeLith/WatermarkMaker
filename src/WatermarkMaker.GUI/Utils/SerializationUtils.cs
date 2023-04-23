using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace WatermarkMaker.Utils
{
    internal static class SerializationUtils
    {
        private static readonly XmlWriterSettings XmlSettings = new()
        {
            Indent = true,
            IndentChars = "    "
        };

        public static void SerializeToXml<T>(this T obj, string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var streamWriter = new StreamWriter(filePath);
            using XmlWriter xmlWriter = XmlWriter.Create(streamWriter, XmlSettings);
            serializer.Serialize(xmlWriter, obj);
        }

        public static T? DeserializeFromXml<T>(string filePath)
            where T : class
        {
            if (!File.Exists(filePath))
                return null;

            var serializer = new XmlSerializer(typeof(T));
            using var streamReader = new StreamReader(filePath);
            return serializer.Deserialize(streamReader) as T;
        }
    }
}