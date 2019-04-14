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

namespace WR.Fragments
{
    public class OpenedProjectFragment : Android.Support.V4.App.Fragment
    {
        static bool isFabOpened = false;
        FloatingActionButton fabMain, fabAddFile, fabAddFolder;
        View fabMenu;
        TextView fabText;
        ListView foldersListView;
        Dialog dialog, dialog1;
        View view;

        ImageButton backBtn;


        //elements of dialog
        TextView closeBtn;
        EditText nameOfSection;
        RadioGroup formOrFileRG;
        Button acceptNewFolder;


        Project project;
        Section currentSection;
        public bool IsRoot = true;

        TextView closeBtnRename;
        EditText renameSection;
        Button acceptNewName;
        string getNewName = null;
        int listPosition;




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
            fabMenu = view.FindViewById<View>(Resource.Id.bg_fabMenu);
            fabText = view.FindViewById<TextView>(Resource.Id.textViewFAB);
            foldersListView = view.FindViewById<ListView>(Resource.Id.listOfFoldersMain);
            backBtn = view.FindViewById<ImageButton>(Resource.Id.backButtonNavigation);

            currentSection = project;

            RegisterForContextMenu(foldersListView);

            foldersListView.ItemClick += FoldersListView_ItemClick;

            foldersListView.ItemClick += OnItemClicked;
            foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);

            dialog = new Dialog(Context);
            dialog1 = new Dialog(Context);

            fabMain.Click += FabMain_Click;
            fabAddFile.Click += FabAddFile_Click;
            fabAddFolder.Click += FabAddFolder_Click;
            fabMenu.Click += (sender, e) => CloseFabMenu();

            backBtn.Click += BackBtnPressedHandler;

            ((Activities.OpenProjectActivity)this.Activity).SupportActionBar.Title = project.Name;

            return view;
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            menu.Add(Resource.String.ContextMenuRename);
            menu.Add(Resource.String.ContextMenuDelete);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            listPosition = info.Position;
            //int index = item.ItemId;
            switch (item.ToString())
            {
                case "Переименовать":
                    ShowPopUpRename();
                    break;
                case "Удалить":
                    currentSection.DeleteSection(listPosition);
                    foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
                    CommitChanges();
                    break;
                default:
                    break;
            }
            return base.OnContextItemSelected(item);
        }

        private void ShowPopUpRename()
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

            acceptNewName.Click += AcceptNewName_Click;

            dialog1.Show();
        }

        void AcceptNewName_Click(object sender, EventArgs e)
        {
            if (renameSection.Text != null)
            {
                getNewName = renameSection.Text;
                currentSection.RenameSection(listPosition, getNewName);
                foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
                dialog1.Dismiss();
                CommitChanges();
            }
        }

        void CommitChanges()
        {
            string dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), project.Name);

            var pathToXML = Path.Combine(dir, $"{project.Name}.xml");

            XmlSerializer xml = new XmlSerializer(typeof(Project), new Type[] { typeof(FileOfProject) });

            using (FileStream fs = new FileStream(pathToXML, FileMode.Create))
            {
                xml.Serialize(fs, project);
            }
        }

        void BackBtnPressedHandler(object sender, EventArgs e)
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
            }
            else
            {
                IsRoot = true;
                Toast toast = Toast.MakeText(this.Activity, "Упс! Уже в корневом каталоге)", Android.Widget.ToastLength.Short);
                toast.Show();
            }

        }

        void FoldersListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int id = e.Position;
            currentSection = currentSection.ChildSections[id];
            ((Activities.OpenProjectActivity)this.Activity).SupportActionBar.Title = currentSection.Path;
            foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
            IsRoot = false;
        }


        private void ShowPopUp()
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
            formOrFileRG = dialog.FindViewById<RadioGroup>(Resource.Id.FormOrFileRadioGroupFolder);
            acceptNewFolder = dialog.FindViewById<Button>(Resource.Id.AcceptNewFolderBtn);

            closeBtn.Click += (sender, e) =>
            {
                dialog.Dismiss();
                CloseFabMenu();
            };

            acceptNewFolder.Click += AcceptNewFolder_Click;

            dialog.Show();
        }

        void AcceptNewFolder_Click(object sender, EventArgs e)
        {
            string selectedType = GetValueFromRadioGroup();
            if (nameOfSection.Text != null)
            {
                try
                {
                    currentSection.AddSection(nameOfSection.Text, selectedType);
                    CommitChanges();
                    CloseFabMenu();
                    dialog.Dismiss();
                }
                catch (IncorrectNameOfSectionException ex)
                {
                    Android.Support.V7.App.AlertDialog.Builder alertBuilder =
                        new Android.Support.V7.App.AlertDialog.Builder(
                            new ContextThemeWrapper(this.Activity, Resource.Style.Theme_AppCompat_Light));
                    alertBuilder.SetTitle(Resource.String.alertCreatingSectionTitle);
                    alertBuilder.SetMessage(ex.Message);
                    alertBuilder.SetNeutralButton(Resource.String.alertNeutralBTN, (senderAlert, args) =>
                    {
                        ShowPopUp();
                    });
                    Dialog dialogError = alertBuilder.Create();
                    dialogError.Show();
                }
            }
        }

        private string GetValueFromRadioGroup()
        {
            return dialog.FindViewById<RadioButton>(formOrFileRG.CheckedRadioButtonId).Text;
        }

        private void OnItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            return;
        }

        void FabMain_Click(object sender, EventArgs e)
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

        void ShowFabMenu()
        {
            isFabOpened = true;
            fabAddFile.Visibility = ViewStates.Visible;
            fabAddFolder.Visibility = ViewStates.Visible;
            fabMenu.Visibility = ViewStates.Visible;

            fabMain.Animate().Rotation(135f);
            fabMenu.Animate().Alpha(1f);
            fabAddFile.Animate()
                .TranslationY(-Resources.GetDimension(Resource.Dimension.standard_100))
                .Rotation(0f);

            fabAddFolder.Animate()
                .TranslationY(-Resources.GetDimension(Resource.Dimension.standard_55))
                .Rotation(0f);
        }

        void CloseFabMenu()
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
                fabAddFile, fabAddFolder));
        }

        void FabAddFile_Click(object sender, EventArgs e)
        {
            fabText.Text = "Файл добавлен";
        }

        void FabAddFolder_Click(object sender, EventArgs e)
        {
            ShowPopUp();
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

        public void Handle_OnOpenCreatedProject(object sender, ProjectEventArgs e)
        {
            project = e.project;
        }
    }

}
