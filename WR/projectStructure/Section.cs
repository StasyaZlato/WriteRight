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
        [XmlElement("Section", typeof(Section))]
        public List<Section> ChildSections = new List<Section>();

        [XmlElement("FormFile", typeof(FormFile))]
        [XmlElement("TextFile", typeof(TextFile))]
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

        public void RenameSection(int id, string name)
        {
            if (ChildSections.Exists(x => x.Name == name))
            {
                throw new IncorrectNameOfSectionException($"Раздел с именем {name} " +
                    "уже существует в данной директории!");
            }

            ChildSections[id].Name = string.IsNullOrEmpty(name) ? throw new IncorrectNameOfFileException("Имя не указано") : name;
            ChildSections[id].Path = this.Path + name + "\\";
        }

        public void AddSection(string name)
        {
            if (ChildSections.Exists(x => x.Name == name))
            {
                throw new IncorrectNameOfSectionException($"Раздел с именем {name} " +
                    "уже существует в данном разделе!");
            }

            ChildSections.Add(new Section(this, name));

        }
        
        public void DeleteSection(int id)
        {
            ChildSections.RemoveAt(id);
        }

        public void AddFile(string name, int num, string type)
        {
            if (files.Exists(x => x.Name == name))
            {
                throw new IncorrectNameOfFileException($"Файл {name} уже существует в данном разделе");
            }
            if (name == "Глоссарий" && type != "form")
            {
                throw new IncorrectNameOfFileException("Имя зарезервировано");
            }

            if (type == "form")
            {
                FormFile f = new FormFile(name, num) { PathInProject = Path + name };
                files.Add(f);
            }
            else
            {
                TextFile t = new TextFile(name, num) { PathInProject = Path + name };
                files.Add(t);
                t.SaveToFile();
            }
        }

        public void AddFile(FileOfProject file)
        {
            if (files.Exists(x => x.Name == file.Name))
            {
                throw new IncorrectNameOfFileException($"Файл {file.Name} уже существует в данном разделе");
            }
            if (file.Name == "Глоссарий" && file is TextFile)
            {
                throw new IncorrectNameOfFileException("Имя зарезервировано");
            }
            file.PathInProject = this.Path + file.Name;
            files.Add(file);
        }

        public void RenameFile(int id, string newName)
        {
            if (files.Exists(x => x.Name == newName))
            {
                throw new IncorrectNameOfFileException($"Файл с именем {newName} " +
                    "уже существует в данной директории!");
            }
            if (newName == "Глоссарий" && files[id] is TextFile)
            {
                throw new IncorrectNameOfFileException("Имя зарезервировано");
            }

            files[id].Name = string.IsNullOrEmpty(newName) ? throw new IncorrectNameOfFileException("Имя не указано") : newName;
            files[id].PathInProject = this.Path + newName;
        }

        public void DeleteFile(int id)
        {
            files.RemoveAt(id);
        }
    }
}
