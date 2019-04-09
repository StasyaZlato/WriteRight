using System;
using System.Collections.Generic;
using Android.OS;

namespace ProjectStructure
{

    public class Section 
    {
        public List<Section> ChildSections = new List<Section>();
        public List<FileOfProject> files = new List<FileOfProject>();

        public string Name { get; set; }
        public string Path { get; set; }

        public DateTime Created { get; set; }

        //конструктор для корневого каталога
        public Section(string name)
        {
            Name = name;
            Path = Name + "\\";
            Created = DateTime.Now;
        }

        //конструктор для остальных каталогов, в качестве Sender родительский каталог
        public Section(object sender, string name)
        {
            Name = name;
            Path = ((Section)sender).Path + Name + "\\";
            Created = DateTime.Now;
        }

        public void AddSection(string name)
        {
            if (ChildSections.Exists(x => x.Name == name))
            {
                throw new IncorrectNameOfSectionException($"Раздел с именем {name} " +
                    "уже существует в данной директории!");
            }
            ChildSections.Add(new Section(this, name));
        }

        public void DeleteSection(int id)
        {
            ChildSections.RemoveAt(id);
        }

        public void AddFile(string name)
        {

        }

        public void DeleteFile()
        {

        }
    }
}
