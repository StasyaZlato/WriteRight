using System;

namespace ProjectStructure
{
    [Serializable]
    public class TextProject : Section
    {
        public TextProject(string name) : base(name) { }

        public TextProject() : base("Текст") { }
    }
}
