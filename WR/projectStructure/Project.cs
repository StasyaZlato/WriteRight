using System;
using System.Xml.Serialization;
using System.IO;



namespace ProjectStructure
{
    [Serializable]
    public class Project : Section
    {
        public string Genre { get; set; }
        public string Theme { get; set; }

        public int CurrentFile = 1;

        public Project() { }

        public Project(string name) : base(name)
        {
            Genre = "Роман";
            Theme = "Повседневность";
            ChildSections.Add(new Section("Информация"));
            ChildSections.Add(new Section("Текст"));
            ChildSections.Add(new Section("Черновик"));
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

            TextFile synopsis = new TextFile("Синопсис",PathToFile, 0)
            {
                PathInProject = Path + "Синопсис",
            };
            files.Add(synopsis);
            synopsis.HtmlText = "<p>Синопсис - краткое содержание будущего произведения</p>";
            synopsis.SaveToFile();
        }
    }
}
