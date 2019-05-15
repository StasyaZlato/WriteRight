using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ProjectStructure
{
    [Serializable]
    public class FormFile : FileOfProject
    {
        [XmlIgnore]
        public List<string[]> fields = new List<string[]>();

        public FormFile() : base()
        {
        }

        public FormFile(string name, string path, int num) : base(name, path, num)
        {
            SaveToFile();
        }


        public FormFile(string name, int num) : base(name, num) { }

        public void SaveToFile()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("root");

            foreach (var item in fields)
            {
                    root.Add(new XElement(
                    "field",
                    new XAttribute("value", item[1]),
                    new XAttribute("name", item[0])));
            }
            doc.Add(root);
            doc.Save(PathToFile);
        }

        public void ReadFromFile()
        {
            fields = new List<string[]>();
            XDocument doc = XDocument.Load(PathToFile);
            foreach (XElement el in doc.Root.Elements())
            {
                string name = el.Attribute("name").Value;
                string value = el.Attribute("value").Value;

                fields.Add(new string[2] { name, value });
            }
        }
    }
}
