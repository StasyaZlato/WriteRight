using Android.App;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using ProjectStructure;
using System;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace WR
{
    [Activity(Label = "WriteRight", MainLauncher = true, Icon = "@mipmap/icon",
        Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        SupportToolbar toolbar;
        ListView leftMenu;
        MyActionBarDrawerToggle drawerToggle;
        DrawerLayout drawerLayout;
        int currentTitleOfActionBar = Resource.String.closeDrawer;
        SupportFragment currentFragment;
        CreateProjectFragment fragCreate;
        OpenExistingProjectFragment fragOpen;
        InfoFragment fragInfo;
        HelloFragment fragHello;
        OpenedProjectFragment fragOpened;
        //Stack<SupportFragment> stackOfFragments = new Stack<SupportFragment>();

        public event EventHandler<ProjectEventArgs> OnOpenCreatedProject;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.MainDrawer);
            leftMenu = FindViewById<ListView>(Resource.Id.LeftMenyListItems);
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbarMain);
            drawerToggle = new MyActionBarDrawerToggle(this, drawerLayout, Resource.String.openDrawer,
                Resource.String.closeDrawer);

            fragInfo = new InfoFragment();
            fragOpen = new OpenExistingProjectFragment();
            fragHello = new HelloFragment();
            fragCreate = new CreateProjectFragment();

            SetSupportActionBar(toolbar);

            drawerLayout.OpenDrawer(leftMenu);

            //добавляем фрагмент
            var transaction = SupportFragmentManager.BeginTransaction();

            transaction.Add(Resource.Id.mainScreenFragmentsContainer,
                fragOpen, "OpenFragment"); 
            transaction.Add(Resource.Id.mainScreenFragmentsContainer,
                 fragCreate, "CreateFragment");
            transaction.Add(Resource.Id.mainScreenFragmentsContainer,
                fragInfo, "InfoFragment");
            transaction.Add(Resource.Id.mainScreenFragmentsContainer,
                fragHello, "HelloFragment");
            //transaction.Add(Resource.Id.mainScreenFragmentsContainer,
                //fragOpened, "OpenedFragment");
            transaction.Hide(fragInfo);
            transaction.Hide(fragOpen);
            transaction.Hide(fragCreate);
            //transaction.Hide(fragOpened);
            transaction.Commit();

            currentFragment = fragHello;

#pragma warning disable CS0618 // Type or member is obsolete
            drawerLayout.SetDrawerListener(drawerToggle);
#pragma warning restore CS0618 // Type or member is obsolete

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            drawerToggle.SyncState();

            leftMenu.ItemClick += LeftMenu_ItemClick;

            drawerLayout.DrawerClosed += DrawerLayout_DrawerClosed;
            fragCreate.ProjectIsCreated += FragCreate_ProjectIsCreated;
        }

        private void FragCreate_ProjectIsCreated(object sender, ProjectEventArgs e)
        {
            Project project = e.project; 
            fragOpened = new OpenedProjectFragment();
            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mainScreenFragmentsContainer, fragOpened);
            transaction.Commit();
            OnOpenCreatedProject(this, new ProjectEventArgs(project));

        }

        private void ShowFragment(SupportFragment fragment)
        {
            var transaction = SupportFragmentManager.BeginTransaction();

            transaction.Hide(currentFragment);
            transaction.Show(fragment);
            //transaction.AddToBackStack(null);
            transaction.Commit();

            //stackOfFragments.Push(currentFragment);
            currentFragment = fragment;
        }


        //public override void OnBackPressed()
        //{
        //    if (SupportFragmentManager.BackStackEntryCount > 0)
        //    {
        //        SupportFragmentManager.PopBackStack();
        //        currentFragment = stackOfFragments.Pop();
        //    }
        //    else
        //    {
        //        base.OnBackPressed();
        //    }
        //}


        void DrawerLayout_DrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
            SupportActionBar.SetTitle(currentTitleOfActionBar);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }


        void LeftMenu_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int itemSelected = e.Position;
            switch (itemSelected)
            {
                case 0:
                    currentTitleOfActionBar = Resource.String.createProject;
                    ShowFragment(fragCreate);
                    break;
                case 1:
                    currentTitleOfActionBar = Resource.String.openProject;
                    ShowFragment(fragOpen);
                    break;
                case 2:
                    currentTitleOfActionBar = Resource.String.info;
                    ShowFragment(fragInfo);
                    break;
            }
            drawerLayout.CloseDrawer(leftMenu);
        }
    }
}

