﻿using System;
using System.Xml.Serialization;
using System.IO;

namespace ProjectStructure
{
    [Serializable]
    public class Project : Section
    {
        public string Genre { get; set; }

        public string Theme { get; set; }

        public bool GlossaryExists
        {
            get
            {
                return files.Exists((obj) =>
                {
                    if (obj.Name == "Глоссарий" && obj is FormFile)
                    {
                        return true;
                    }
                    return false;
                });
            }
        }

        public User user;

        private int curFile;

        public int CurrentFile
        {
            get => curFile;
            set
            {
                if (Name == value.ToString())
                {
                    curFile = value + 1;
                }
                else
                {
                    curFile = value;
                }
            }
        }

        public Project() { }

        public Project(string name) : base(name)
        {
            Genre = "Роман";
            Theme = "Повседневность";
            ChildSections.Add(new Section("Информация"));
            ChildSections.Add(new Section("Текст"));
            ChildSections.Add(new Section("Черновик"));

            CurrentFile = 1;

            GetUser();
        }

        public Project(string name, string genre, string theme, bool text, bool draft, bool info) : base(name)
        {
            string PathToProject = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Name);
            string PathToFile = System.IO.Path.Combine(PathToProject, "0.xml");

            Genre = genre;
            Theme = theme;
            if (text)
            {
                ChildSections.Add(new Section(this, "Текст"));
            }
            if (draft)
            {
                ChildSections.Add(new Section(this, "Черновик"));
            }
            if (info)
            {
                ChildSections.Add(new Section(this, "Информация"));
            }

            TextFile synopsis = new TextFile("Синопсис", PathToFile, 0)
            {
                PathInProject = Path + "Синопсис",
            };
            files.Add(synopsis);
            synopsis.HtmlText = "<p>Синопсис - краткое содержание будущего произведения</p>";
            synopsis.SaveToFile();

            CurrentFile = 1;

            GetUser();
        }

        public void GetUser()
        {
            string userPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "user.xml");
            if (File.Exists(userPath))
            {
                XmlSerializer xml = new XmlSerializer(typeof(User));
                using (FileStream fs = new FileStream(userPath, FileMode.Open))
                {
                    user = (User)xml.Deserialize(fs);
                }
            }

            else
            {
                user = new User("Nastya", "Iva");
            }
        }

        public static Project GetData(string xml)
        {
            Project project;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Project), new Type[] { typeof(FileOfProject), typeof(User) });

            using (FileStream fs = new FileStream(xml, FileMode.Open))
            {
                project = (Project)xmlSerializer.Deserialize(fs);
            }
            return project;
        }

        public void CommitChanges()
        {
            string pathToXml = System.IO.Path.Combine(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Name), $"{Name}.xml");

            XmlSerializer xml = new XmlSerializer(typeof(Project), new Type[] { typeof(FileOfProject), typeof(User) });

            using (FileStream fs = new FileStream(pathToXml, FileMode.Create))
            {
                xml.Serialize(fs, this);
            }
        }
    }
}