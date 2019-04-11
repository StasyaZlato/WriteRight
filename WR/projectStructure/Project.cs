using System;


namespace ProjectStructure
{
    [Serializable]
    public class Project : Section
    {
        public string Genre { get; set; }
        public string Theme { get; set; }

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
            Genre = genre;
            Theme = theme;
            if (text)
            {
                ChildSections.Add(new TextProject());
            }
            if (draft)
            {
                ChildSections.Add(new DraftProject());
            }
            if (info)
            {
                ChildSections.Add(new FormProject());
            }
        }
    }
}
