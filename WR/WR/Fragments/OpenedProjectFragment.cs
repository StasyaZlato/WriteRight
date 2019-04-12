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
        Dialog dialog;
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


        public override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            //((MainActivity)Activity).OnOpenCreatedProject += Handle_OnOpenCreatedProject;


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

            foldersListView.ItemClick += FoldersListView_ItemClick;

            foldersListView.ItemClick += OnItemClicked;
            foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);

            dialog = new Dialog(Context);

            fabMain.Click += FabMain_Click;
            fabAddFile.Click += FabAddFile_Click;
            fabAddFolder.Click += FabAddFolder_Click;
            fabMenu.Click += (sender, e) => CloseFabMenu();

            backBtn.Click += BackBtnPressedHandler;


            fabText.Text = project.Name;
            return view;
        }

        void BackBtnPressedHandler(object sender, EventArgs e)
        {
            if (IsRoot = false || currentSection.parent != null)
            {
                currentSection = currentSection.parent;
                foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
                fabText.Text = currentSection.Path;
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
            currentSection = project.ChildSections[id];
            foldersListView.Adapter = new CustomViews.FoldersListAdapter(currentSection.ChildSections);
            fabText.Text = currentSection.Path;
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
                    project.AddSection(nameOfSection.Text, selectedType);
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
