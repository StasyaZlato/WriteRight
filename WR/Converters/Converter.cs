using System.Collections.Generic;
using ProjectStructure;

namespace Converters
{
    public class Converter
    {
        public List<TextFile> files;
        public Project project;
        public List<string[]> fieldsOfGloss = null;

        public Converter(Project project, List<TextFile> files)
        {
            this.project = project;
            this.files = files;
        }

        public Converter(Project project, List<TextFile> files, FormFile gloss) : this(project, files)
        {
            if (gloss != null)
            {
                fieldsOfGloss = gloss.fields;
            }
        }
    }
}