using System;

namespace ProjectStructure
{
    public class DraftProject : Section
    {
        public DraftProject(string name) : base(name)
        {
        }
        public DraftProject() : base("Черновик") { }
    }
}
