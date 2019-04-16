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
using System.Xml.Serialization;
using System.IO;

namespace WR.Activities
{
    [Activity(Label = "WriteRight", MainLauncher = true, Icon = "@mipmap/icon",
    Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        public SupportToolbar toolbar;
        public ListView leftMenu;
        MyActionBarDrawerToggle drawerToggle;
        public DrawerLayout drawerLayout;
        int currentTitleOfActionBar = Resource.String.closeDrawer;
        public SupportFragment currentFragment;
        Fragments.CreateProjectFragment fragCreate;
        Fragments.OpenExistingProjectFragment fragOpen;
        Fragments.InfoFragment fragInfo;
        Fragments.HelloFragment fragHello;
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

            fragInfo = new Fragments.InfoFragment();
            fragOpen = new Fragments.OpenExistingProjectFragment();
            fragHello = new Fragments.HelloFragment();
            fragCreate = new Fragments.CreateProjectFragment();

            SetSupportActionBar(toolbar);

            //drawerLayout.OpenDrawer(leftMenu);

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

            string intent = this.Intent.GetStringExtra("frag");
            if (intent != null)
            {
                switch (intent)
                {
                    case "open":
                        ShowFragment(fragOpen);
                        break;
                    case "create":
                        ShowFragment(fragCreate);
                        break;
                    case "info":
                        ShowFragment(fragInfo);
                        break;
                    default:
                        break;
                }
                drawerLayout.CloseDrawer(leftMenu);
            }

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
            string dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), project.Name);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var pathToXML = Path.Combine(dir,  $"{project.Name}.xml");


            XmlSerializer xml = new XmlSerializer(typeof(Project), new Type[] { typeof(FileOfProject) });

            using (FileStream fs = new FileStream(pathToXML, FileMode.Create))
            {
                xml.Serialize(fs, project);
            }

            Intent intent = new Intent(this, typeof(OpenProjectActivity));
            intent.PutExtra("xml", pathToXML);
            StartActivity(intent);
        }

        public void ShowFragment(SupportFragment fragment)
        {
            var transaction = SupportFragmentManager.BeginTransaction();

            transaction.Hide(currentFragment);
            transaction.Show(fragment);
            transaction.Commit();

            currentFragment = fragment;
        }

        void DrawerLayout_DrawerClosed(object sender, DrawerLayout.DrawerClosedEventArgs e)
        {
            SupportActionBar.SetTitle(currentTitleOfActionBar);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            drawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }


        public void LeftMenu_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
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
                case 3:
                    Intent intent = new Intent(this, typeof(EditorActivity));
                    StartActivity(intent);
                    break;
            }
            drawerLayout.CloseDrawer(leftMenu);
        }
    }
}

