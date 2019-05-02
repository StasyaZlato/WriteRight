using System;
using System.IO;
using System.Xml;

namespace ProjectStructure
{
    [Serializable]
    public class FormFile : FileOfProject
    {
        public FormFile() : base()
        {
        }

        public FormFile(int i)
        {

        }

        public FormFile(string name, int num) : base(name, num) { }

        //public void SaveToFile()
        //{
        //    using (FileStream fs = new FileStream(PathToFile, FileMode.OpenOrCreate))
        //    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default))
        //    {
        //        sw.Write(HtmlText);
        //    }
        //}
    }
}
