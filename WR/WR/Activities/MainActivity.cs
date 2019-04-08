using Android.App;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using ProjectsAndFiles;
using System;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace WR
{
    [Activity(Label = "WriteRight", MainLauncher = true, Icon = "@mipmap/icon",
        Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        SupportToolbar toolbar;
        List<string> leftMenuItems = new List<string>();
        MenuListViewAdapter leftMenyItemsAdapter;
        ListView leftMenu;
        MyActionBarDrawerToggle drawerToggle;
        DrawerLayout drawerLayout;
        int currentTitleOfActionBar = Resource.String.closeDrawer;
        SupportFragment currentFragment;
        CreateProjectFragment fragCreate;
        OpenExistingProjectFragment fragOpen;
        InfoFragment fragInfo;
        HelloFragment fragHello;
        //Stack<SupportFragment> stackOfFragments = new Stack<SupportFragment>();


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

            leftMenuItems.AddRange(new string[] { "Создать новый проект",
                "Открыть существующий проект", "Информация" });
            leftMenyItemsAdapter = new MenuListViewAdapter(this, leftMenuItems);
            leftMenu.Adapter = leftMenyItemsAdapter;

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
            transaction.Hide(fragInfo);
            transaction.Hide(fragOpen);
            transaction.Hide(fragCreate);
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

