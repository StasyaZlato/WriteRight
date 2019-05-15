using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Animation;
using ProjectStructure;
using Newtonsoft.Json;

using NPOI.POIFS.FileSystem;
using NPOI.XWPF.UserModel;
using NPOI.XWPF;
using NPOI.XWPF.Extractor;

using System.Threading.Tasks;

namespace WR.Fragments
{
    public class OpenedProjectFragment : Android.Support.V4.App.Fragment
    {
        public event EventHandler FilePicked;

        static bool isFabOpened = false;
        FloatingActionButton fabMain, fabAddFile, fabAddFolder, fabAddForm;
        View fabMenu;
        TextView fabText;
        ListView foldersListView, filesListView;
        Dialog dialog, dialog1;
        View view;

        ImageButton backBtn;

        //elements of dialog
        ImageButton closeBtn;
        EditText nameOfSection, nameOfFile;
        Button acceptNewFolder;


        Project project;
        Section currentSection;
        public bool IsRoot = true;

        ImageButton closeBtnRename;
        EditText renameSection;
        Button acceptNewName;
        string getNewName = null;
        int listPosition;

        Button acceptNewFile;

        // тип создаваемого файла
        int type;

        string dir;

        string text;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.ProjectOpenedFragment, container, false);

            fabMain = view.FindViewById<FloatingActionButton>(Resource.Id.mainActionBtnAddSth);
            fabAddFile = view.FindViewById<FloatingActionButton>(Resource.Id.addFileActionButton);
            fabAddFolder = view.FindViewById<FloatingActionButton>(Resource.Id.addSectionActionButton);
            fabAddForm = view.FindViewById<FloatingActionButton>(Resource.Id.addFormActionButton);
            fabMenu = view.FindViewById<View>(Resource.Id.bg_fabMenu);
            fabText = view.FindViewById<TextView>(Resource.Id.textViewFAB);
            foldersListView = view.FindViewById<ListView>(Resource.Id.listOfFoldersMain);
            filesListView = view.FindViewById<ListView>(Resource.Id.listOfFilesMain);
            backBtn = view.FindViewById<ImageButton>(Resource.Id.backButtonNavigation);

            dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), project.Name);

            currentSection = project;

            RegisterForContextMenu(foldersListView);
            RegisterForContextMenu(filesListView);

            foldersListView.ItemClick += FoldersListView_ItemClick;
            foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);

            filesListView.ItemClick += FilesListView_ItemClick;
            filesListView.Adapter = new CustomViews.FilesListAdapter(currentSection.files);

            dialog = new Dialog(Context);
            dialog1 = new Dialog(Context);

            fabMain.Click += FabMain_Click;
            fabAddFile.Click += FabAddFile_Click;
            fabAddFolder.Click += FabAddFolder_Click;
            fabAddForm.Click += FabAddForm_Click;
            fabMenu.Click += (sender, e) => CloseFabMenu();

            backBtn.Click += BackBtnPressedHandler;

            ((Activities.OpenProjectActivity)this.Activity).SupportActionBar.Title = project.Name;

            ((Activities.OpenProjectActivity)this.Activity).importBtn.Click += ImportBtn_Click;

            FilePicked += OpenedProjectFragment_FilePicked;

            ((Activities.OpenProjectActivity)this.Activity).SupportActionBar.Title = project.Path;

            return view;
        }

        void OpenedProjectFragment_FilePicked(object sender, EventArgs e)
        {
            //string name = Path.GetFileNameWithoutExtension(path);
            string name = "импортированный файл";
            string newName = name;

            int i = 1;
            while (currentSection.files.Exists(x => x.Name == newName))
            {
                newName = $"{name}{i++}";
            }

            TextFile file;
            try
            {
                string pathToFile = System.IO.Path.Combine(
                    System.IO.Path.Combine(
                        System.Environment.GetFolderPath(
                            System.Environment.SpecialFolder.MyDocuments),
                        project.Name),
                    $"{project.CurrentFile}.xml");

                file = new TextFile(newName, pathToFile, project.CurrentFile++)
                {
                    PathInProject = currentSection.Path + newName,
                    HtmlText = text
                };

                file.SaveToFile();

                currentSection.AddFile(file);
                project.CommitChanges();
                filesListView.Adapter = new CustomViews.FilesListAdapter(currentSection.files);
                Toast.MakeText(this.Activity, "Файл импортирован", ToastLength.Short).Show();
            }
            catch (IncorrectNameOfFileException ex)
            {
                Toast.MakeText(this.Context,
                    ex.Message, ToastLength.Short).Show();
            }
        }


        void ImportBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionGetContent);
            intent.SetType("text/plain");
            intent.AddCategory(Intent.CategoryOpenable);
            StartActivityForResult(Intent.CreateChooser(intent, "Select a file"), 0);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 0)
            {
                if (resultCode == -1)
                {
                    string uri = data.DataString;
                    if (!uri.EndsWith(".txt") && !uri.EndsWith(".docx"))
                    {
                        Toast.MakeText(this.Activity, "Возможен импорт только txt и docx файлов!", ToastLength.Short).Show();
                        return;
                    }
                    else if (uri.EndsWith(".docx"))
                    {
                        Android.Net.Uri uris = Android.Net.Uri.FromParts(data.Data.Scheme, data.Data.SchemeSpecificPart, data.Data.Fragment);
                        using (Stream input = Activity.ContentResolver.OpenInputStream(data.Data))
                        {
                            Toast.MakeText(this.Activity, "Подождите, пока файл обрабатывается...", ToastLength.Short).Show();
                            XWPFDocument document = new XWPFDocument(input);
                            XWPFWordExtractor extractor = new XWPFWordExtractor(document);
                            text = extractor.Text;
                            document.Close();
                        }
                    }
                    else
                    {
                        Android.Net.Uri uris = Android.Net.Uri.FromParts(data.Data.Scheme, data.Data.SchemeSpecificPart, data.Data.Fragment);
                        using (Stream input = Activity.ContentResolver.OpenInputStream(data.Data))
                        using (StreamReader sw = new StreamReader(input))
                        {
                            text = sw.ReadToEnd();
                        }
                    }
                    FilePicked?.Invoke(this, new EventArgs());
                }
            }
        }

        void FilesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int id = e.Position;
            string fullpath = Path.Combine(dir, currentSection.files[id].NameOfFile);

            if (currentSection.files[id] is TextFile)
            {
                Intent intent = new Intent(this.Activity, typeof(Activities.EditorActivity));

                intent.PutExtra("path", fullpath);

                StartActivity(intent);
            }

            if (currentSection.files[id] is FormFile)
            {
                Intent intent = new Intent(this.Activity, typeof(Activities.FormEditorActivity));

                intent.PutExtra("path", fullpath);
                intent.PutExtra("form", JsonConvert.SerializeObject(currentSection.files[id]));

                StartActivity(intent);
            }
        }


        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            switch (v.Id)
            {
                case Resource.Id.listOfFoldersMain:
                    menu.Add(Resource.String.ContextMenuRename);
                    menu.Add(Resource.String.ContextMenuDelete);
                    break;
                case Resource.Id.listOfFilesMain:
                    menu.Add(Resource.String.ContextMenuRenameFile);
                    menu.Add(Resource.String.ContextMenuDeleteFile);
                    break;
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            listPosition = info.Position;
            switch (item.ToString())
            {
                case "Переименовать раздел":
                    ShowPopUpRename(0);
                    break;
                case "Удалить раздел":
                    currentSection.DeleteSection(listPosition);
                    foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
                    project.CommitChanges();
                    break;
                case "Переименовать файл":
                    ShowPopUpRename(1);
                    break;
                case "Удалить файл":
                    currentSection.DeleteFile(listPosition);
                    filesListView.Adapter = new CustomViews.FilesListAdapter(currentSection.files);
                    project.CommitChanges();
                    break;
                default:
                    break;
            }
            return base.OnContextItemSelected(item);
        }


        private void AcceptNewNameFolder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(renameSection.Text))
            {
                getNewName = renameSection.Text;
                try
                {
                    currentSection.RenameSection(listPosition, getNewName);
                    foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
                    dialog1.Dismiss();
                    project.CommitChanges();
                }
                catch (IncorrectNameOfSectionException ex)
                {
                    Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                }

            }
        }

        private void AcceptNewNameFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(renameSection.Text))
            {
                getNewName = renameSection.Text;
                try
                {
                    currentSection.RenameFile(listPosition, getNewName);
                    filesListView.Adapter = new CustomViews.FilesListAdapter(currentSection.files);
                    dialog1.Dismiss();
                    project.CommitChanges();
                }
                catch (IncorrectNameOfFileException ex)
                {
                    Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                }
            }
        }



        private void BackBtnPressedHandler(object sender, EventArgs e)
        {
            if (IsRoot == false)
            {
                string[] path = currentSection.Path.Split('\\');
                if (path.Length == 2)
                {
                    IsRoot = true;
                }
                Section tempSection = project;
                for (int i = 1; i < path.Length - 2; i++)
                {
                    tempSection.ChildSections.ForEach(x =>
                    {
                        if (x.Name == path[i])
                        {
                            tempSection = x;
                            return;
                        }
                    });
                }
                currentSection = tempSection;
                ((Activities.OpenProjectActivity)this.Activity).SupportActionBar.Title = currentSection.Path;
                ((Activities.OpenProjectActivity)this.Activity).currentTitleOfActionBar = currentSection.Path; foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
                filesListView.Adapter = new CustomViews.FilesListAdapter(currentSection.files);
            }
            else
            {
                IsRoot = true;
                Toast toast = Toast.MakeText(this.Activity, "Упс! Уже в корневом каталоге)", Android.Widget.ToastLength.Short);
                toast.Show();
            }
        }

        private void FoldersListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int id = e.Position;
            currentSection = currentSection.ChildSections[id];
            ((Activities.OpenProjectActivity)this.Activity).SupportActionBar.Title = currentSection.Path;
            ((Activities.OpenProjectActivity)this.Activity).currentTitleOfActionBar = currentSection.Path;

            foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
            filesListView.Adapter = new CustomViews.FilesListAdapter(currentSection.files);

            IsRoot = false;
        }


        private void ShowPopUpFolder()
        {
            // the exception appears if the dialog has been created earlier
            try
            {
                dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            }
            catch (Android.Util.AndroidRuntimeException) { }

            dialog.SetContentView(Resource.Layout.CustomPopUpAddingFolder);
            closeBtn = dialog.FindViewById<ImageButton>(Resource.Id.TextViewClosePopUpNewFolder);
            nameOfSection = dialog.FindViewById<EditText>(Resource.Id.EditTextNameOfNewFolder);
            acceptNewFolder = dialog.FindViewById<Button>(Resource.Id.AcceptNewFolderBtn);

            closeBtn.Click += (sender, e) =>
            {
                dialog.Dismiss();
                CloseFabMenu();
            };

            acceptNewFolder.Click += AcceptNewFolder_Click;

            dialog.Show();
        }

        private void ShowPopUpFile()
        {
            // the exception appears if the dialog has been created earlier
            try
            {
                dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            }
            catch (Android.Util.AndroidRuntimeException) { }

            dialog.SetContentView(Resource.Layout.CustomPopUpAddingFile);
            closeBtn = dialog.FindViewById<ImageButton>(Resource.Id.TextViewClosePopUpNewFile);
            nameOfFile = dialog.FindViewById<EditText>(Resource.Id.EditTextNameOfNewFile);
            acceptNewFile = dialog.FindViewById<Button>(Resource.Id.AcceptNewFileBtn);

            closeBtn.Click += (sender, e) =>
            {
                dialog.Dismiss();
                CloseFabMenu();
            };

            acceptNewFile.Click += AcceptNewFile_Click;

            dialog.Show();
        }

        private void ShowPopUpRename(int num)
        {
            // the exception appears if the dialog has been created earlier
            try
            {
                dialog1.RequestWindowFeature((int)WindowFeatures.NoTitle);
            }
            catch (Android.Util.AndroidRuntimeException) { }

            dialog1.SetContentView(Resource.Layout.CustomPopUpRenameFolder);
            closeBtnRename = dialog1.FindViewById<ImageButton>(Resource.Id.TextViewClosePopUpRename);
            renameSection = dialog1.FindViewById<EditText>(Resource.Id.RenameSectionTE);
            acceptNewName = dialog1.FindViewById<Button>(Resource.Id.AcceptNewName);

            closeBtnRename.Click += (sender, e) =>
            {
                dialog1.Dismiss();
                CloseFabMenu();
            };

            switch (num)
            {
                case 0:
                    acceptNewName.Click -= AcceptNewNameFile_Click;
                    acceptNewName.Click += AcceptNewNameFolder_Click;
                    break;
                case 1:
                    acceptNewName.Click -= AcceptNewNameFolder_Click;
                    acceptNewName.Click += AcceptNewNameFile_Click;
                    break;
                default:
                    break;
            }
            dialog1.Show();
        }

        private bool CheckInvalidFileName(string filename)
        {
            return filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
        }

        private void AcceptNewFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(nameOfFile.Text))
            {
                if (CheckInvalidFileName(nameOfFile.Text))
                {
                    Toast.MakeText(this.Activity, "Название содержит недопустимые символы", ToastLength.Short);
                    return;
                }
                FileOfProject file;
                try
                {
                    string pathToFile = System.IO.Path.Combine(
                        System.IO.Path.Combine(
                            System.Environment.GetFolderPath(
                                System.Environment.SpecialFolder.MyDocuments),
                            project.Name),
                        $"{project.CurrentFile}.xml");

                    if (type == 0)
                    {
                        file = new TextFile(nameOfFile.Text, pathToFile, project.CurrentFile++);
                    }
                    else
                    {
                        file = new FormFile(nameOfFile.Text, pathToFile, project.CurrentFile++);
                    }
                    file.PathInProject = currentSection.Path + nameOfFile.Text;
                    currentSection.AddFile(file);
                    project.CommitChanges();
                    dialog.Dismiss();
                }
                catch (IncorrectNameOfFileException ex)
                {
                    Toast.MakeText(this.Context,
                        ex.Message, ToastLength.Short).Show();
                }
                finally
                {
                    CloseFabMenu();
                }
            }
        }

        private void AcceptNewFolder_Click(object sender, EventArgs e)
        {
            if (nameOfSection.Text != null)
            {
                try
                {
                    currentSection.AddSection(nameOfSection.Text);
                    project.CommitChanges();
                    CloseFabMenu();
                    dialog.Dismiss();
                }
                catch (IncorrectNameOfSectionException ex)
                {
                    Toast.MakeText(this.Context,
                        ex.Message, ToastLength.Short).Show();
                }
            }
        }

        private void FabMain_Click(object sender, EventArgs e)
        {
            if (!isFabOpened)
            {
                ShowFabMenu();
            }
            else
            {
                CloseFabMenu();
            }
        }

        private void ShowFabMenu()
        {
            isFabOpened = true;
            fabAddFile.Visibility = ViewStates.Visible;
            fabAddFolder.Visibility = ViewStates.Visible;
            fabMenu.Visibility = ViewStates.Visible;
            fabAddForm.Visibility = ViewStates.Visible;

            fabMain.Animate().Rotation(135f);
            fabMenu.Animate().Alpha(1f);
            fabAddFile.Animate()
                .TranslationY(-Resources.GetDimension(Resource.Dimension.standard_100))
                .Rotation(0f);

            fabAddFolder.Animate()
                .TranslationY(-Resources.GetDimension(Resource.Dimension.standard_55))
                .Rotation(0f);

            fabAddForm.Animate()
                .TranslationY(-Resources.GetDimension(Resource.Dimension.standard_145))
                .Rotation(0f);
        }

        private void CloseFabMenu()
        {
            isFabOpened = false;

            fabAddFile.Visibility = ViewStates.Visible;
            fabAddFolder.Visibility = ViewStates.Visible;
            fabMenu.Visibility = ViewStates.Visible;

            fabMain.Animate().Rotation(0f);
            fabMenu.Animate().Alpha(0f);
            fabAddFile.Animate()
                .TranslationY(0f)
                .Rotation(90f);

            fabAddFolder.Animate()
                .TranslationY(0f)
                .Rotation(90f).SetListener(new FabAnimatorListener(fabMenu,
                fabAddFile, fabAddFolder, fabAddForm));

            fabAddForm.Animate()
                .TranslationY(0f)
                .Rotation(90f).SetListener(new FabAnimatorListener(fabMenu,
                fabAddFile, fabAddFolder, fabAddForm));
        }

        private void FabAddFile_Click(object sender, EventArgs e)
        {
            type = 0;
            ShowPopUpFile();
        }

        private void FabAddForm_Click(object sender, EventArgs e)
        {
            type = 1;
            ShowPopUpFile();
        }

        private void FabAddFolder_Click(object sender, EventArgs e)
        {
            ShowPopUpFolder();
        }

        private class FabAnimatorListener : Java.Lang.Object, Animator.IAnimatorListener
        {
            View[] viewsToHide;

            public FabAnimatorListener(params View[] views)
            {
                viewsToHide = views;
            }

            public void OnAnimationCancel(Animator animation)
            {
            }

            public void OnAnimationEnd(Animator animation)
            {
                if (!isFabOpened)
                {
                    foreach (var view in viewsToHide)
                    {
                        view.Visibility = ViewStates.Gone;
                    }
                }
            }

            public void OnAnimationRepeat(Animator animation)
            {
            }

            public void OnAnimationStart(Animator animation)
            {
            }
        }

        public void Handle_OnOpenCreatedProject(object sender, CustomEventArgs.ProjectEventArgs e)
        {
            project = e.project;
        }
    }

}
