using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
    public class OpenExistingProjectFragment : Android.Support.V4.App.Fragment
    {
        ListView listOfProjects;
        string path;

        List<string> projects = new List<string>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.OpenExistingProjectFragment, container, false);

            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            GetProjects();

            listOfProjects = view.FindViewById<ListView>(Resource.Id.listViewProjects);
            listOfProjects.Adapter = new CustomViews.ProjectsListAdapter(projects);

            listOfProjects.ItemClick += ListOfProjects_ItemClick;

            return view;
        }

        void ListOfProjects_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string pathToXml = Path.Combine(path, Path.Combine(projects[e.Position], $"{projects[e.Position]}.xml"));
            Intent intent = new Intent(this.Activity, typeof(Activities.OpenProjectActivity));
            intent.PutExtra("xml", pathToXml);
            StartActivity(intent);
        }


        public void GetProjects()
        {
            string[] tempProjects = Directory.GetDirectories(path);
            Array.ForEach(tempProjects, x =>
            {
                DirectoryInfo dir = new DirectoryInfo(x);
                string name = dir.Name;
                if (!name.StartsWith("."))
                {
                    projects.Add(name);
                }
            });
        }
    }
}
