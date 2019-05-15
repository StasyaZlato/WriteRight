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
using Android.Views.InputMethods;

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
        public string currentTitleOfActionBar = "WriteRight";
        public SupportFragment currentFragment;
        Fragments.CreateProjectFragment fragCreate;
        Fragments.OpenExistingProjectFragment fragOpen;
        Fragments.InfoFragment fragInfo;
        Fragments.HelloFragment fragHello;

        string userPath;

        public FrameLayout leftDrawer;

        public TextView firstName, lastName;

        public TextView changeUser;

        protected override void OnCreate(Bundle savedInstanceState)
        { 
            base.OnCreate(savedInstanceState);

            //this.Window.SetSoftInputMode(SoftInput.StateHidden);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.MainDrawer);
            leftMenu = FindViewById<ListView>(Resource.Id.LeftMenyListItems);
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbarMain);
            FindViewById<ImageView>(Resource.Id.UserIcon);
            firstName = FindViewById<TextView>(Resource.Id.firstName);
            lastName = FindViewById<TextView>(Resource.Id.lastName);
            leftDrawer = FindViewById<FrameLayout>(Resource.Id.LeftDrawer);
            changeUser = FindViewById<TextView>(Resource.Id.changeUser);
            drawerToggle = new MyActionBarDrawerToggle(this, drawerLayout, Resource.String.openDrawer,
                Resource.String.closeDrawer);

            fragInfo = new Fragments.InfoFragment();
            fragOpen = new Fragments.OpenExistingProjectFragment();
            fragHello = new Fragments.HelloFragment();
            fragCreate = new Fragments.CreateProjectFragment();

            userPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "user.xml");
            if (File.Exists(userPath))
            {
                User user;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(User));
                using (FileStream fs = new FileStream(userPath, FileMode.Open))
                {
                    user = (User)xmlSerializer.Deserialize(fs);
                }
                firstName.Text = user.FirstName;
                lastName.Text = user.LastName;
            }

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
                    case "userCh":
                        if (File.Exists(userPath))
                        {
                            File.Delete(userPath);
                        }
                        ShowFragment(fragHello);
                        break;
                    default:
                        ShowFragment(fragHello);
                        break;
                }
                drawerLayout.CloseDrawer(leftDrawer);
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

            changeUser.Click += ChangeUser_Click;
        }

        void ChangeUser_Click(object sender, EventArgs e)
        {
            if (File.Exists(userPath))
            {
                File.Delete(userPath);
            }
            fragHello.userInfoLL.Visibility = ViewStates.Visible;
            fragHello.helloRL.Visibility = ViewStates.Gone;

            ShowFragment(fragHello);

            drawerLayout.CloseDrawer(leftDrawer);
            firstName.Text = "First name";
            lastName.Text = "Last name";
        }


        private void FragCreate_ProjectIsCreated(object sender, CustomEventArgs.ProjectEventArgs e)
        {
            Project project = e.project;
            string dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), project.Name);

            var pathToXML = Path.Combine(dir,  $"{project.Name}.xml");


            XmlSerializer xml = new XmlSerializer(typeof(Project), new Type[] { typeof(FileOfProject), typeof(User) });

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
            SupportActionBar.Title = currentTitleOfActionBar;
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
                    currentTitleOfActionBar = Resources.GetString(Resource.String.closeDrawer);
                    ShowFragment(fragHello);
                    break;
                case 1:
                    currentTitleOfActionBar = Resources.GetString(Resource.String.createProject);
                    ShowFragment(fragCreate);
                    break;
                case 2:
                    currentTitleOfActionBar = Resources.GetString(Resource.String.openProject);
                    fragOpen.Refresh();
                    ShowFragment(fragOpen);
                    break;
                case 3:
                    currentTitleOfActionBar = Resources.GetString(Resource.String.info);
                    ShowFragment(fragInfo);
                    break;
            }
            drawerLayout.CloseDrawer(leftDrawer);
        }
    }
}

