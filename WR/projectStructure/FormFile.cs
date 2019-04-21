using System;
using System.IO;

namespace ProjectStructure
{
    [Serializable]
    public class FormFile : FileOfProject
    {
        public FormFile()
        {
        }

        public FormFile(string name, string path, int num) : base(name, path, num) { }

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
