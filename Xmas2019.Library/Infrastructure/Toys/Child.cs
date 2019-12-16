using System;
using System.Xml.Serialization;

namespace Xmas2019.Library.Infrastructure.Toys
{
    [Serializable]
    public class Child
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        public WishList WishList { get; set; }

        public Child()
        {

        }
    }
}
