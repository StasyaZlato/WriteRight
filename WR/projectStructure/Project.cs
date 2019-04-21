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
            ChildSections.Add(new TextProject());
            ChildSections.Add(new DraftProject());
            ChildSections.Add(new FormProject());

        }

        public Project(string name, string genre, string theme, bool text, bool draft, bool info) : base(name)
        {
            string PathToProject = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Name);
            string PathToFile = System.IO.Path.Combine(PathToProject, "0.xml");

            Genre = genre;
            Theme = theme;
            if (text)
            {
                ChildSections.Add(new TextProject(this));
            }
            if (draft)
            {
                ChildSections.Add(new DraftProject(this));
            }
            if (info)
            {
                ChildSections.Add(new FormProject(this));
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
