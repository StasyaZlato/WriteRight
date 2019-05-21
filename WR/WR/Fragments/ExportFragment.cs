using System;
using System.Collections.Generic;
using System.IO;
using Android.OS;
using Android.Views;
using Android.Widget;
using Converters;
using iTextSharp.text.pdf;
using ProjectStructure;

namespace WR.Fragments
{
    public class ExportFragment : Android.Support.V4.App.Fragment
    {
        private string path;
        private Project project;

        private View view;
        private Spinner formatSpinner;
        private ListView filesForExportListView;
        private Button acceptExportBtn;

        private List<TextFile> files = new List<TextFile>();
        private List<TextFile> checkedFiles = new List<TextFile>();

        private CheckBox glossaryCB;
        private TextView glossaryTV;

        private bool glossary;

        private FormFile gloss = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            path = this.Activity.Intent.GetStringExtra("path");
            project = Project.GetData(path);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.ExportFragment, container, false);

            formatSpinner = view.FindViewById<Spinner>(Resource.Id.formatSpinner);
            filesForExportListView = view.FindViewById<ListView>(Resource.Id.filesForExportListView);
            acceptExportBtn = view.FindViewById<Button>(Resource.Id.acceptExportDataBtn);
            glossaryCB = view.FindViewById<CheckBox>(Resource.Id.glossaryCheckBox);
            glossaryTV = view.FindViewById<TextView>(Resource.Id.glossaryTV);

            glossary = project.GlossaryExists;

            if (glossary)
            {
                glossaryCB.Visibility = ViewStates.Visible;
                glossaryTV.Visibility = ViewStates.Gone;

                gloss = (FormFile)project.files[project.files.FindIndex((obj) =>
                {
                    if (obj.Name == "Глоссарий" && obj is FormFile)
                    {
                        return true;
                    }
                    return false;
                })];

                gloss.ReadFromFile();
            }
            else
            {
                glossaryCB.Visibility = ViewStates.Gone;
                glossaryTV.Visibility = ViewStates.Visible;
            }

            GetAllFiles(project);

            filesForExportListView.Adapter = new CustomViews.FilesToExportListAdapter(files);

            filesForExportListView.ItemClick += FilesForExportListView_ItemClick;

            acceptExportBtn.Click += AcceptExportBtn_Click;

            return view;
        }

        private async void AcceptExportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                switch (formatSpinner.SelectedItemId)
                {
                    case 0:
                        // format = "fb2";
                        await new ConverterToFB2Book(project, checkedFiles, gloss).CreateFB2Async();
                        Toast.MakeText(this.Context, "Сохранено в корневом каталоге", ToastLength.Short).Show();
                        break;
                    case 1:
                        // format = "pdf";

                        // проблема itextsharp в том, что его внутренние шрифты не поддерживают 
                        // русский язык. Вообще. Даже те, которые в обычных условиях такой разборчивостью
                        // не страдают. Поэтому приходится подгружать свой шрифт и ставить русскую кодировку
                        // уже ему. itextsharp требует путь к шрифту, поэтому просто забрать его из 
                        // папки assets не выйдет, приходится копировать на устройство.
                        string fontPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "times.ttf");

                        if (!File.Exists(fontPath))
                        {
                            var input = Resources.Assets.Open("times.ttf");
                            FileStream fs = new FileStream(fontPath, FileMode.Create);
                            input.CopyTo(fs);
                            fs.Close();
                            input.Close();
                        }

                        BaseFont font = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        await new ConverterToPdf(project, checkedFiles, font, gloss).CreatePDFAsync();

                        Toast.MakeText(this.Context, "Сохранено в корневом каталоге", ToastLength.Short).Show();
                        break;
                    case 2:
                        // format = "docx";
                        await new ConverterToDocX(project, checkedFiles, gloss).CreateDocXAsync();
                        Toast.MakeText(this.Context, "Сохранено в корневом каталоге", ToastLength.Short).Show();
                        break;
                    case 3:
                        // format = "txt"
                        await new ConverterToTxt(project, checkedFiles, gloss).CreateTxtAsync();
                        Toast.MakeText(this.Context, "Сохранено в корневом каталоге", ToastLength.Short).Show();
                        break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Toast.MakeText(this.Context, "Кажется, ваше устройство не хочет давать доступ к памяти:(", ToastLength.Short).Show();
            }

        }

        private void FilesForExportListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
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