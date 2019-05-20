using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProjectStructure;

namespace WR.Activities
{
    [Activity(Label = "OpenProjectActivity", Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class OpenProjectActivity : MainActivity
    {
        private Fragments.OpenedProjectFragment fragOpened;
        private Project project;
        public ImageButton exportBtn, importBtn;
        private string xmlProjectPath;

        public event EventHandler<CustomEventArgs.ProjectEventArgs> OnProjectCreated;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            fragOpened = new Fragments.OpenedProjectFragment();

            xmlProjectPath = Intent.GetStringExtra("xml");
            project = Project.GetData(xmlProjectPath);

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mainScreenFragmentsContainer, fragOpened);
            transaction.Commit();
            currentFragment = fragOpened;
            ShowFragment(fragOpened);

            exportBtn = FindViewById<ImageButton>(Resource.Id.ExportBtn);
            exportBtn.Visibility = ViewStates.Visible;

            importBtn = FindViewById<ImageButton>(Resource.Id.ImportBtn);
            importBtn.Visibility = ViewStates.Visible;

            exportBtn.Click += ExportBtn_Click;

            OnProjectCreated += fragOpened.Handle_OnOpenCreatedProject;

            OnProjectCreated?.Invoke(this, new CustomEventArgs.ProjectEventArgs(project));

            leftMenu.ItemClick -= LeftMenu_ItemClick;
            leftMenu.ItemClick += LeftMenu_ItemClick1;

            changeUser.Click += ChangeUser_Click;
        }

        private void ChangeUser_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("frag", "userCh");
            StartActivity(intent);
        }

        private void ExportBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ExportActivity));
            intent.PutExtra("path", xmlProjectPath);
            StartActivity(intent);
        }

        private void LeftMenu_ItemClick1(object sender, AdapterView.ItemClickEventArgs e)
        {
            int itemSelected = e.Position;
            Intent intent = new Intent(this, typeof(MainActivity));
            switch (itemSelected)
            {
                case 0:
                    intent.PutExtra("frag", "hello");
                    break;
                case 1:
                    intent.PutExtra("frag", "create");
                    break;
                case 2:
                    intent.PutExtra("frag", "open");
                    break;
                case 3:
                    intent.PutExtra("frag", "info");
                    break;
            }
            StartActivity(intent);
            drawerLayout.CloseDrawer(leftDrawer);
        }

        protected override void OnRestart()
        {
            project = Project.GetData(xmlProjectPath);
            OnProjectCreated?.Invoke(this, new CustomEventArgs.ProjectEventArgs(project));
            base.OnRestart();
        }
    }
}