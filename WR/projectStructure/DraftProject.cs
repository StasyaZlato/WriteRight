using System;

namespace ProjectStructure
{
    [Serializable]
    public class DraftProject : Section
    {
        public DraftProject(string name) : base(name)
        {
        }
        public DraftProject() : base("Черновик") { }

        public DraftProject(object sender) : base(sender, "Черновик") { }


        public DraftProject(object sender, string name) : base(sender, name) { }

    }
}
