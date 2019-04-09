using System;
using ProjectStructure;

namespace WR
{
    public class ProjectEventArgs : EventArgs
    {
        public Project project;

        public ProjectEventArgs(Project project)
        {
            this.project = project;
        }
    }
}