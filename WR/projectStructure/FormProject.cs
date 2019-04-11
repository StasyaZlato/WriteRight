using System;

namespace ProjectStructure
{
    [Serializable]
    public class FormProject : Section
    {
        public FormProject(string name) : base(name) { }

        public FormProject() : base("Информация") { }
    }
}
