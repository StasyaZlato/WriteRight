using System;

namespace ProjectStructure
{
    [Serializable]
    public class TextProject : Section
    {
        public TextProject(string name) : base(name) { }

        public TextProject() : base("Текст") { }

        public TextProject(object sender) : base(sender, "Текст") { }


        public TextProject(object sender, string name) : base(sender, name) { }

    }
}
