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

namespace WR.Fragments
{
    public class OpenedProjectFragment : Android.Support.V4.App.Fragment
    {
        static bool isFabOpened = false;
        FloatingActionButton fabMain, fabAddFile, fabAddFolder, fabAddForm;
        View fabMenu;
        TextView fabText;
        ListView foldersListView, filesListView;
        Dialog dialog, dialog1;
        View view;

        ImageButton backBtn;

        //elements of dialog
        TextView closeBtn;
        EditText nameOfSection, nameOfFile;
        Button acceptNewFolder;


        Project project;
        Section currentSection;
        public bool IsRoot = true;

        TextView closeBtnRename;
        EditText renameSection;
        Button acceptNewName;
        string getNewName = null;
        int listPosition;

        Button acceptNewFile;

        // тип создаваемого файла
        int type;

        string dir;


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

            return view;
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
                    CommitChanges();
                    break;
                case "Переименовать файл":
                    ShowPopUpRename(1);
                    break;
                case "Удалить файл":
                    currentSection.DeleteFile(listPosition);
                    filesListView.Adapter = new CustomViews.FilesListAdapter(currentSection.files);
                    CommitChanges();
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
                    CommitChanges();
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
                    CommitChanges();
                }
                catch (IncorrectNameOfFileException ex)
                {
                    Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                }
            }
        }

        private void CommitChanges()
        {

            var pathToXML = Path.Combine(dir, $"{project.Name}.xml");

            XmlSerializer xml = new XmlSerializer(typeof(Project), new Type[] { typeof(FileOfProject) });

            using (FileStream fs = new FileStream(pathToXML, FileMode.Create))
            {
                xml.Serialize(fs, project);
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
                foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
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
            closeBtn = dialog.FindViewById<TextView>(Resource.Id.TextViewClosePopUpNewFolder);
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
            closeBtn = dialog.FindViewById<TextView>(Resource.Id.TextViewClosePopUpNewFile);
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
            closeBtnRename = dialog1.FindViewById<TextView>(Resource.Id.TextViewClosePopUpRename);
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

        private void AcceptNewFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(nameOfFile.Text))
            {
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
                    CommitChanges();
                    dialog.Dismiss();
                }
                catch (IncorrectNameOfFileException)
                {
                    Toast.MakeText(this.Context, 
                        Resource.String.alertCreatingFileTitleMsg, ToastLength.Short).Show();
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
                    CommitChanges();
                    CloseFabMenu();
                    dialog.Dismiss();
                }
                catch (IncorrectNameOfSectionException)
                {
                    Toast.MakeText(this.Context,
                        Resource.String.alertCreatingFolderTitleMsg, ToastLength.Short).Show();
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
