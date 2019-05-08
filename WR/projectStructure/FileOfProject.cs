using System;
using System.Xml.Serialization;
using System.IO;

namespace ProjectStructure
{
    [Serializable]
    public class FileOfProject
    {
        public string Name { get; set; }
        public string PathInProject { get; set; }
        public string NameOfFile { get; set; }
        public string PathToFile { get; set; }


        public FileOfProject()
        {
        }

        public FileOfProject(string name, string path, int num)
        {
            Name = name;
            PathToFile = path;
            NameOfFile = $"{num}.xml";
        }

        public FileOfProject(string name, int num)
        {
            Name = name;
            NameOfFile = $"{num}.xml";
        }

        //public static bool operator ==(FileOfProject file, FileOfProject file2)
        //{
        //    return file.PathInProject == file2.PathInProject;
        //}

        //public static bool operator !=(FileOfProject file, FileOfProject file2)
        //{
        //    return file.PathInProject != file2.PathInProject;
        //}

        //public override bool Equals(object obj)
        //{
        //    return this == (FileOfProject)obj;
        //}
    }
}