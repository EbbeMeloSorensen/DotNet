using System.Xml.Serialization;

namespace Craft.DataStructures.IO.gml
{
    public class Point
    {
        [XmlAttribute(AttributeName = "srsName")]
        public string SrsName { get; set; }

        [XmlAttribute(AttributeName = "id", Namespace = "http://www.opengis.net/gml/3.2")]
        public string Id { get; set; }

        [XmlElement(ElementName = "pos")] 
        public string Position { get; set; }
    }
}
