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
using System.IO;
using System.Xml.Serialization;

namespace WR.Activities
{
    [Activity(Label = "OpenProjectActivity", Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class OpenProjectActivity : MainActivity
    {
        Fragments.OpenedProjectFragment fragOpened;
        //SupportFragment previousFragment;
        Project project;
        //Project project;
        public ImageButton exportBtn;
        string xmlProjectPath;


        public event EventHandler<CustomEventArgs.ProjectEventArgs> OnProjectCreated;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            fragOpened = new Fragments.OpenedProjectFragment();

            xmlProjectPath = Intent.GetStringExtra("xml");
            GetData(xmlProjectPath);

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mainScreenFragmentsContainer, fragOpened);
            transaction.Commit();
            currentFragment = fragOpened;
            ShowFragment(fragOpened);

            exportBtn = FindViewById<ImageButton>(Resource.Id.ExportBtn);
            exportBtn.Visibility = ViewStates.Visible;

            exportBtn.Click += ExportBtn_Click;

            OnProjectCreated += fragOpened.Handle_OnOpenCreatedProject;

            OnProjectCreated?.Invoke(this, new CustomEventArgs.ProjectEventArgs(project));
            leftMenu.ItemClick -= LeftMenu_ItemClick;
            leftMenu.ItemClick += LeftMenu_ItemClick1;

        }

        void ExportBtn_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ExportActivity));
            intent.PutExtra("path", xmlProjectPath);
            StartActivity(intent);
        }

        void LeftMenu_ItemClick1(object sender, AdapterView.ItemClickEventArgs e)
        {
            int itemSelected = e.Position;
            Intent intent = new Intent(this, typeof(MainActivity));
            switch (itemSelected)
            {
                case 0:
                    intent.PutExtra("frag", "create");
                    break;
                case 1:
                    intent.PutExtra("frag", "open");
                    break;
                case 2:
                    intent.PutExtra("frag", "info");
                    break;
            }
            StartActivity(intent);
            drawerLayout.CloseDrawer(leftMenu);
        }


        public void GetData(string xml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Project), new Type[] { typeof(FileOfProject) });

            using (FileStream fs = new FileStream(xml, FileMode.Open))
            {
                project = (Project)xmlSerializer.Deserialize(fs);
            }
        }
    }
}
