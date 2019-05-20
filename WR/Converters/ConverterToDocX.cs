using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NPOI.XWPF.UserModel;
using ProjectStructure;

namespace Converters
{
    public class ConverterToDocX : Converter
    {
        private XWPFDocument doc;

        public ConverterToDocX(Project project, List<TextFile> files) : base(project, files) { }

        public ConverterToDocX(Project project, List<TextFile> files, FormFile gloss) : base(project, files, gloss) { }

        private void AddChapters()
        {
            Regex regex = new Regex(@"<.*?>", RegexOptions.IgnoreCase);
            for (int j = 0; j < files.Count; j++)
            {
                TextFile file = files[j];
                using (FileStream fs = new FileStream(file.PathToFile, FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string text = sr.ReadToEnd();

                    text = text.Replace("&nbsp;", " ");

                    XWPFParagraph fp = doc.CreateParagraph();
                    if (j != 0)
                    {
                        fp.IsPageBreak = true;
                    }
                    fp.Alignment = ParagraphAlignment.CENTER;

                    XWPFRun title = fp.CreateRun();
                    title.SetText($"{j + 1}. {file.Name}");
                    title.IsBold = true;
                    title.FontSize = 14;

                    string[] paragraphs = text.Split("<br>");

                    for (int i = 0; i < paragraphs.Length; i++)
                    {
                        paragraphs[i] = regex.Replace(paragraphs[i], string.Empty);

                        XWPFParagraph p = doc.CreateParagraph();
                        p.Alignment = ParagraphAlignment.LEFT;

                        XWPFRun r1 = p.CreateRun();
                        r1.FontSize = 14;
                        r1.SetText(paragraphs[i]);
                    }
                }
            }
        }

        public async Task CreateDocXAsync()
        {
            await Task.Run(() => CreateDocX());
        }

        private void CreateDocX()
        {
            string path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, $"{project.Name}.docx");

            int i = 1;
            while (File.Exists(path))
            {
                string newFileName = $"{project.Name}({i++}).docx";
                path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, newFileName);
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                doc = new XWPFDocument();

                XWPFParagraph p1 = doc.CreateParagraph();
                p1.Alignment = ParagraphAlignment.CENTER;
                XWPFRun r1 = p1.CreateRun();
                r1.SetText(project.Name);
                r1.IsBold = true;
                r1.FontSize = 16;

                XWPFParagraph p2 = doc.CreateParagraph();
                p2.Alignment = ParagraphAlignment.CENTER;
                XWPFRun r2 = p2.CreateRun();

                r2.SetText($"{project.user.FirstName} {project.user.LastName}");
                r2.IsBold = true;
                r2.FontSize = 14;

                AddChapters();

                AddFields();

                doc.Write(fs);
                doc.Close();
            }
        }

        private void AddFields()
        {
            if (fieldsOfGloss != null)
            {
                XWPFParagraph gp = doc.CreateParagraph();
                gp.IsPageBreak = true;
                gp.Alignment = ParagraphAlignment.CENTER;

                XWPFRun title = gp.CreateRun();
                title.SetText($"Глоссарий");
                title.IsBold = true;
                title.FontSize = 16;

                for (int i = 0; i < fieldsOfGloss.Count; i++)
                {
                    XWPFParagraph p = doc.CreateParagraph();
                    p.Alignment = ParagraphAlignment.LEFT;

                    XWPFRun r1 = p.CreateRun();
                    r1.FontSize = 14;
                    r1.SetText($"{fieldsOfGloss[i][0]} - {fieldsOfGloss[i][1]}");
                }
            }
        }
    }
}