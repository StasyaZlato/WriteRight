using System;
using ProjectStructure;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

using System.Threading.Tasks;

namespace Converters
{
    public class ConverterToFB2Book
    {
        string genre, authorFN, authorLN;
        Project project;
        List<TextFile> files;

        XDocument fb2 = new XDocument();

        public ConverterToFB2Book(Project project, List<TextFile> files)
        {
            this.project = project;
            this.files = files;
            authorFN = project.user.FirstName;
            authorLN = project.user.LastName;

            if (string.IsNullOrEmpty(project.Theme))
            {
                genre = project.Genre;
            }
            else
            {
                genre = project.Theme;
            }
        }

        private void CreateXml()
        {
            XDeclaration xd = new XDeclaration("1.0", "utf-8", "yes");

            XElement description, titleInfo, documentInfo, body, publishInfo;

            XNamespace xNamespace = "http://www.gribuser.ru/xml/fictionbook/2.0";

            titleInfo = new XElement("title-info",
                new XElement("genre", genre),
                new XElement("author",
                    new XElement("first-name", authorFN),
                    new XElement("last-name", authorLN)),
                new XElement("book-title", project.Name),
                new XElement("annotation", string.Empty),
                new XElement("keywords", string.Empty),
                new XElement("date",
                    new XAttribute("value", DateTime.Today.ToShortDateString()),
                    DateTime.Today.ToShortDateString()),
                new XElement("coverpage", string.Empty),
                new XElement("lang", "ru"));

            documentInfo = new XElement("document-info",
                new XElement("author",
                    new XElement("first-name", authorFN),
                    new XElement("last-name", authorLN),
                    new XElement("nickname")),
                new XElement("program-used", "WriteRight"),
                new XElement("date",
                    new XAttribute("value", DateTime.Today.ToShortDateString()),
                    DateTime.Today.ToShortDateString()),
                new XElement("src-url", string.Empty),
                new XElement("src-orl", string.Empty),
                new XElement("id", string.Empty),
                new XElement("version", "1.0"));

            publishInfo = new XElement("publish-info", string.Empty);

            description = new XElement("description",
                titleInfo,
                documentInfo,
                publishInfo);

            body = new XElement("body",
                new XElement("title",
                    new XElement("p", $"{authorFN} {authorLN}"),
                    new XElement("empty-line"),
                    new XElement("p", project.Name)));

            body = FillBodyWithChapters(body);

            fb2 = new XDocument(
                xd,
                new XElement(xNamespace + "FictionBook",
                    new XAttribute(XNamespace.Xmlns + "l", "http://www.w3.org/1999/xlink"),
                    new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"),
                    new XAttribute("xmlns", "http://www.gribuser.ru/xml/fictionbook/2.0"),
                    description,
                    body));
        }

        private XElement FillBodyWithChapters(XElement body)
        {
            foreach (var file in files)
            {
                using (FileStream fs = new FileStream(file.PathToFile, FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    string text = sr.ReadToEnd();

                    //text = ProcessHtml(text);

                    XElement[] section = ProcessHtml(text);

                    body.Add(new XElement("section",
                        new XElement("title", file.Name),
                        section));
                }
            }
            return body;
        }

        private XElement[] ProcessHtml(string text)
        {
            Regex regex = new Regex(@"<div.*?>", RegexOptions.IgnoreCase);

            text = regex.Replace(text, string.Empty);

            regex = new Regex(@"<h[123].*?>", RegexOptions.IgnoreCase);

            text = regex.Replace(text, "<subtitle>");

            regex = new Regex(@"</h[123]>", RegexOptions.IgnoreCase);
            text = regex.Replace(text, "</subtitle>");


            text = text.Replace("<b>", "<strong>")
                        .Replace("</b>", "</strong>")
                        .Replace("<i>", "<emphasis>")
                        .Replace("</i>", "</emphasis>")
                        .Replace("</div>", string.Empty)
                        .Replace("<u>", string.Empty)
                        .Replace("</u>", string.Empty)
                        .Replace("&nbsp;", " "); // подчеркивание отсутствует в fb2 :(


            string[] textLines = text.Split("<br>"); // "умный" редактор вместо абзацев расставляет переносы

            textLines = Array.ConvertAll(textLines, x =>
            {
                if (!string.IsNullOrEmpty(x))
                {
                    return $"<p>{x}</p>";
                }
                return "<empty-line/>";
            });

            XElement[] elements = Array.ConvertAll(textLines, x => XElement.Parse(x));

            return elements;
        }

        public async Task CreateFB2Async()
        {
            await Task.Run(() => CreateFB2());
        }

        private void CreateFB2()
        {
            CreateXml();

            string path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, $"{project.Name}.fb2");

            int i = 1;
            while (File.Exists(path))
            {
                string newFileName = $"{project.Name}({i++}).fb2";
                path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, newFileName);
            }
            fb2.Save(path);
        }

    }
}
