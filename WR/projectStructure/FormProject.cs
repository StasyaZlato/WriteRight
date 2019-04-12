using System;

namespace ProjectStructure
{
    [Serializable]
    public class FormProject : Section
    {
        public FormProject(string name) : base(name) { }

        public FormProject() : base("Информация") { }

        public FormProject(object sender) : base(sender, "Информация") { }


        public FormProject(object sender, string name) : base (sender, name) { }
    }
}
