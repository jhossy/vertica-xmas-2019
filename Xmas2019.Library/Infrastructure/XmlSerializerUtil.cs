﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Xmas2019.Library.Infrastructure
{
    public static class XmlSerializerUtil
    {
        public static T Deserialize<T>(XDocument doc)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            
            using (var reader = doc.Root.CreateReader())
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }

        public static XDocument Serialize<T>(T value)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            XDocument doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                xmlSerializer.Serialize(writer, value);
            }

            return doc;
        }
    }
}
