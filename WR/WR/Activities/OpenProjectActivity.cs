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
        public event EventHandler BackBtnPressed;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            fragOpened = new Fragments.OpenedProjectFragment();

            string xmlProject = Intent.GetStringExtra("xml");
            GetData(xmlProject);

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mainScreenFragmentsContainer, fragOpened);
            transaction.Commit();
            currentFragment = fragOpened;
            ShowFragment(fragOpened);

            OnProjectCreated += fragOpened.Handle_OnOpenCreatedProject;

            OnProjectCreated?.Invoke(this, new ProjectEventArgs(project));
            
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
