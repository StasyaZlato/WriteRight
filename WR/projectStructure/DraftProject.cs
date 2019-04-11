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
    }
}
