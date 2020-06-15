using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace FutbolPlay.Functions
{
    public class FieldsConfiguration
    {
        [XmlElement("FieldConfiguration")]
        List<FieldConfiguration> Fields { get; set; }

        public FieldConfiguration this[string name, string table]
        {
            get
            {
                FieldConfiguration field = Fields.FirstOrDefault(f => f.Name.Equals(name) && f.Table.Equals(table));
                return field;
            }
        }
    }

    [Serializable]
    public class FieldConfiguration
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Table")]
        public string Table { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("Mandatory")]
        public bool Mandatory { get; set; }

        [XmlElement("Length")]
        public string Length { get; set; }
    }
}