
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Xml.Serialization;
using System.IO;
using ProjectStructure;
using Converters;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace WR.Fragments
{
    public class ExportFragment : Android.Support.V4.App.Fragment
    {
        string path;
        Project project;
        View view;
        Spinner formatSpinner;
        ListView filesForExportListView;
        Button acceptExportBtn;
        List<TextFile> files = new List<TextFile>();
        List<TextFile> checkedFiles = new List<TextFile>();

        // string format;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            path = this.Activity.Intent.GetStringExtra("path");
            GetData(path);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.ExportFragment, container, false);

            formatSpinner = view.FindViewById<Spinner>(Resource.Id.formatSpinner);
            filesForExportListView = view.FindViewById<ListView>(Resource.Id.filesForExportListView);
            acceptExportBtn = view.FindViewById<Button>(Resource.Id.acceptExportDataBtn);

            GetAllFiles(project);

            filesForExportListView.Adapter = new CustomViews.FilesToExportListAdapter(files);

            filesForExportListView.ItemClick += FilesForExportListView_ItemClick;

            acceptExportBtn.Click += AcceptExportBtn_Click;

            return view;
        }

        void AcceptExportBtn_Click(object sender, EventArgs e)
        {
            switch (formatSpinner.SelectedItemId)
            {
                case 0:
                    // format = "fb2";
                    ConverterToFB2Book converter = new ConverterToFB2Book(project, checkedFiles);
                    Toast.MakeText(this.Context, "Сохранено в корневом каталоге", ToastLength.Short).Show();
                    break;
                case 1:
                    // format = "pdf";
                    string dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "times.ttf");

                    if (!File.Exists(dir))
                    {
                        var input = Resources.Assets.Open("times.ttf");
                        FileStream fs = new FileStream(dir, FileMode.Create);
                        input.CopyTo(fs);
                        fs.Close();
                        input.Close();
                    }

                    BaseFont font = BaseFont.CreateFont(dir, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    ConverterToPdf converterPDF = new ConverterToPdf(project, checkedFiles, font);
                    Toast.MakeText(this.Context, "Сохранено в корневом каталоге", ToastLength.Short).Show();
                    break;
                case 2:
                    // format = "doc";
                    break;
            }

        }


        void FilesForExportListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ImageView checkFile = e.View.FindViewById<ImageView>(Resource.Id.CheckBoxFileForExport);
            if (!checkedFiles.Exists((obj) =>
            {
                if (obj == files[e.Position])
                {
                    return true;
                }
                return false;
            }))
            {
                checkFile.SetImageResource(Resource.Drawable.check_box_outline);
                checkedFiles.Add(files[e.Position]);
            }
            else
            {
                checkFile.SetImageResource(Resource.Drawable.checkbox_blank_outline);
                checkedFiles.Remove(files[e.Position]);
            }

            ListView chosen = view.FindViewById<ListView>(Resource.Id.testLV);
            chosen.Adapter = new CustomViews.ChosenFilesListAdapter(checkedFiles);
        }


        public void GetData(string xml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Project), new Type[] { typeof(FileOfProject) });

            using (FileStream fs = new FileStream(xml, FileMode.Open))
            {
                project = (Project)xmlSerializer.Deserialize(fs);
            }
        }

        public void GetAllFiles(ProjectStructure.Section section)
        {
            if (section.ChildSections.Count > 0)
            {
                foreach (var item in section.ChildSections)
                {
                    GetAllFiles(item);
                }
            }
            if (section.files.Count > 0)
            {
                section.files.ForEach(x =>
                {
                    if (x is TextFile)
                    {
                        files.Add((TextFile)x);
                    }
                });
            }
        }
    }
}
