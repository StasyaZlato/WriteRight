using System;
using System.Collections.Generic;
using System.Xml.Linq;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

using ProjectStructure;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using XamiTextSharpLGPL.Droid;


namespace Converters
{
    public class ConverterToPdf
    {
        private List<TextFile> files;
        private Project project;

        private Document pdf;
        private PdfWriter pdfWriter;
        private Font regular, bold;

        public ConverterToPdf(Project project, List<TextFile> files, BaseFont bf)
        {
            this.project = project;
            this.files = files;
            
            BaseFont bfTimes = bf; 

            regular = new Font(bfTimes, 14, Font.NORMAL, Color.BLACK);
            bold = new Font(bfTimes, 14, Font.BOLD);
        }

        private List<Chapter> AddChapters()
        {
            List<Chapter> chapters = new List<Chapter>();
            int i = 1;
            foreach (var file in files)
            {
                using (FileStream fs = new FileStream(file.PathToFile, FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string text = sr.ReadToEnd();
                    Regex regex = new Regex(@"<.*?>");

                    text = text.Replace("&nbsp;", " ");

                    Paragraph titleSection = new Paragraph(file.Name, bold);
                    titleSection.Alignment = Element.ALIGN_CENTER;
                    Chapter chapter = new Chapter(titleSection, i++);

                    string[] paragraphs = text.Split("<br>");
                    foreach (var par in paragraphs)
                    {
                        Paragraph paragraph = new Paragraph(regex.Replace(par, string.Empty), regular);
                        chapter.Add(paragraph);
                    }

                    chapters.Add(chapter);
                }
            }
            return chapters;
        }

        public async Task CreatePDFAsync()
        {
            await Task.Run(() => CreatePDF());
        }

        private void CreatePDF()
        {
            string path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, $"{project.Name}.pdf");

            int i = 1;
            while (File.Exists(path))
            {
                string newFileName = $"{project.Name}({i++}).pdf";
                path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, newFileName);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                pdf = new Document(PageSize.A4, 25, 25, 30, 30);

                pdfWriter = PdfWriter.GetInstance(pdf, fs);

                pdf.Open();

                pdf.AddAuthor($"{project.user.FirstName} {project.user.LastName}");
                pdf.AddTitle(project.Name);
                pdf.AddCreator("WriteRight");

                List<Chapter> chapters = AddChapters();

                foreach (var chapter in chapters)
                {
                    pdf.Add(chapter);
                }

                pdf.Close();
            }
        }

    }
}
