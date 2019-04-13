using System;
using System.Collections.Generic;
using Android.OS;
using System.Xml.Serialization;

namespace ProjectStructure
{
    [Serializable]
    [XmlInclude(typeof(Project))]
    public class Section 
    {
        [XmlElement("Text", typeof(TextProject))]
        [XmlElement("Draft", typeof(DraftProject))]
        [XmlElement("Forms", typeof(FormProject))]
        public List<Section> ChildSections = new List<Section>();

        [XmlElement("Form", typeof(FormOfProject))]
        public List<FileOfProject> files = new List<FileOfProject>();

        public string Name { get; set; }
        public string Path { get; set; }

        public DateTime Created { get; set; }

        //для сериализации
        public Section() { }

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

        public void AddSection(string name, string type)
        {
            if (ChildSections.Exists(x => x.Name == name))
            {
                throw new IncorrectNameOfSectionException($"Раздел с именем {name} " +
                    "уже существует в данной директории!");
            }
            if (type == "Форма")
            {
                ChildSections.Add(new FormProject(this, name));
            }
            else
            {
                ChildSections.Add(new TextProject(this, name));
            }

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
