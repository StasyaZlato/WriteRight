using System;
using System.IO;
using System.Collections.Generic;
using ProjectStructure;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Converters
{
    public class ConverterToTxt
    {
        private Project project;
        private List<TextFile> files;
        private List<string[]> fieldsOfGloss = null;



        public ConverterToTxt(Project project, List<TextFile> files)
        {
            this.project = project;
            this.files = files;
        }

        public ConverterToTxt(Project project, List<TextFile> files, FormFile gloss) : this(project, files)
        {
            fieldsOfGloss = gloss.fields;
        }


        private string AddChapters()
        {
            int i = 1;
            string output = $"{project.Name}\n{project.user.FirstName} {project.user.LastName}\n";
            foreach (var file in files)
            {
                using (FileStream fs = new FileStream(file.PathToFile, FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string text = sr.ReadToEnd();
                    Regex regex = new Regex(@"<.*?>");

                    text = text.Replace("<br>", "\n")
                                .Replace("&nbsp;", " ");
                    text = regex.Replace(text, string.Empty);
                    output = output + $"{i++}. {file.Name}\n" + text;
                }
            }
            if (fieldsOfGloss != null)
            {
                output += "\nГлоссарий\n";
                for (int j = 0; j < fieldsOfGloss.Count; j++)
                {
                    output += $"{fieldsOfGloss[j][0]} - {fieldsOfGloss[j][1]}\n";
                }
            }
            return output;
        }


        public async Task CreateTxtAsync()
        {
            await Task.Run(() => CreateTxt());
        }

        private void CreateTxt()
        {
            string path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, $"{project.Name}.txt");

            int i = 1;
            while (File.Exists(path))
            {
                string newFileName = $"{project.Name}({i++}).txt";
                path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, newFileName);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default))
            {
                string text = AddChapters();
                sw.Write(text);
            }
        }
    }
}
