using System;
using System.Xml.Serialization;

namespace Xmas2019.Library.Infrastructure.Toys
{
    [Serializable]
    public class Toy
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        public Toy()
        {

        }
    }
}
