using System;

namespace ProjectsAndFiles
{
    public class Project : Section
    {
        public string Genre { get; set; }
        public string Theme { get; set; }

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
            Genre = genre;
            Theme = theme;
        }
    }
}
