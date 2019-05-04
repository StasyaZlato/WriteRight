using System;
using System.Xml.Serialization;
using System.IO;

namespace ProjectStructure
{
    [Serializable]
    public class TextFile : FileOfProject
    {
        [XmlIgnore]
        public string HtmlText { get; set; }

        public TextFile()
        {
        }

        public TextFile(string name, string path, int num) : base(name, path, num) 
        {
            SaveToFile();
        }

        public TextFile(string name, int num) : base(name, num) 
        {
            SaveToFile();
        }

        public void SaveToFile()
        {
            using (StreamWriter sw = new StreamWriter(PathToFile, false))
            {
                sw.Write(HtmlText);
            }
        }
    }
}
