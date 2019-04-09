
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

namespace WR
{
    public class OpenedProjectFragment : Android.Support.V4.App.Fragment
    {   
        static bool isFabOpened = false;
        FloatingActionButton fabMain, fabAddFile, fabAddFolder;
        View fabMenu;
        TextView fabText;
        Project project;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ProjectOpenedFragment, container, false);


            fabMain = view.FindViewById<FloatingActionButton>(Resource.Id.mainActionBtnAddSth);
            fabAddFile = view.FindViewById<FloatingActionButton>(Resource.Id.addFileActionButton);
            fabAddFolder = view.FindViewById<FloatingActionButton>(Resource.Id.addSectionActionButton);
            fabMenu = view.FindViewById<View>(Resource.Id.bg_fabMenu);
            fabText = view.FindViewById<TextView>(Resource.Id.textViewFAB);


            fabMain.Click += FabMain_Click;
            fabAddFile.Click += FabAddFile_Click;
            fabAddFolder.Click += FabAddFolder_Click;
            fabMenu.Click += (sender, e) => CloseFabMenu();

            ((MainActivity)Activity).OnOpenCreatedProject += Handle_OnOpenCreatedProject;

            fabText.Text = project.Name;
            return view;
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
            fabText.Text = "Раздел добавлен";
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

        void Handle_OnOpenCreatedProject(object sender, ProjectEventArgs e)
        {
            project = e.project;
        }

    }

}
