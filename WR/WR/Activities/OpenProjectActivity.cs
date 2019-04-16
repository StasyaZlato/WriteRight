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

        public event EventHandler<ProjectEventArgs> OnProjectCreated;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            fragOpened = new Fragments.OpenedProjectFragment();

            string xmlProject = Intent.GetStringExtra("xml");
            GetData(xmlProject);

            //drawerLayout.CloseDrawer(leftMenu);

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mainScreenFragmentsContainer, fragOpened);
            transaction.Commit();
            currentFragment = fragOpened;
            ShowFragment(fragOpened);

            OnProjectCreated += fragOpened.Handle_OnOpenCreatedProject;

            OnProjectCreated?.Invoke(this, new ProjectEventArgs(project));
            leftMenu.ItemClick -= LeftMenu_ItemClick;
            leftMenu.ItemClick += LeftMenu_ItemClick1;

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
                case 3:
                    Intent intentEditor = new Intent(this, typeof(EditorActivity));
                    StartActivity(intentEditor);
                    return;
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
