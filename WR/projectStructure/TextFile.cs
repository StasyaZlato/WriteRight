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
            //string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), project.Name);

            //using (FileStream fs = new FileStream(PathToFile, FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(PathToFile, false))
            {
                sw.Write(HtmlText);
            }
        }
    }
}
