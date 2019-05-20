using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProjectStructure;

namespace WR.Fragments
{
    public class OpenExistingProjectFragment : Android.Support.V4.App.Fragment
    {
        private ListView listOfProjects;
        private string path;

        // context dialog for removing / renaming
        private Dialog dialogRename;

        private ImageButton closeBtnRename;
        private EditText renameProject;
        private Button acceptNewName;

        private List<string> projects = new List<string>();

        private int listPosition;

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

            RegisterForContextMenu(listOfProjects);

            listOfProjects.ItemClick += ListOfProjects_ItemClick;

            return view;
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            menu.Add(Resource.String.ContextMenuRenameProject);
            menu.Add(Resource.String.ContextMenuDeleteProject);

        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            listPosition = info.Position;
            string pathToProject = Path.Combine(path, projects[listPosition]);
            switch (item.ToString())
            {
                case "Переименовать проект":
                    ShowPopUpRename();
                    break;
                case "Удалить проект":
                    Directory.Delete(pathToProject, true);
                    Refresh();
                    break;
            }
            return base.OnContextItemSelected(item);
        }

        private void ShowPopUpRename()
        {
            dialogRename = new Dialog(this.Activity);
            // the exception appears if the dialog has been created earlier
            try
            {
                dialogRename.RequestWindowFeature((int)WindowFeatures.NoTitle);
            }
            catch (Android.Util.AndroidRuntimeException) { }

            dialogRename.SetContentView(Resource.Layout.CustomPopUpRenameFolder);
            closeBtnRename = dialogRename.FindViewById<ImageButton>(Resource.Id.TextViewClosePopUpRename);
            renameProject = dialogRename.FindViewById<EditText>(Resource.Id.RenameSectionTE);
            acceptNewName = dialogRename.FindViewById<Button>(Resource.Id.AcceptNewName);

            closeBtnRename.Click += (sender, e) =>
            {
                dialogRename.Dismiss();
            };

            acceptNewName.Click += AcceptNewName_Click;

            dialogRename.Show();
        }

        private void AcceptNewName_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(renameProject.Text))
            {
                Toast.MakeText(this.Activity, "Новое имя не указано!", ToastLength.Short).Show();
            }
            else if (Directory.Exists(Path.Combine(path, renameProject.Text)))
            {
                Toast.MakeText(this.Activity, "Проект с таким именем уже существует!", ToastLength.Short).Show();
            }
            else if (Section.CheckInvalidFileName(renameProject.Text))
            {
                Toast.MakeText(this.Activity, "Новое имя содержит недопустимые символы", ToastLength.Short).Show();
            }
            else
            {
                string newPath = Path.Combine(path, renameProject.Text);
                string PathToXml = Path.Combine(Path.Combine(path, projects[listPosition]), $"{projects[listPosition]}.xml");
                string newPathToXml = Path.Combine(Path.Combine(path, projects[listPosition]), $"{renameProject.Text}.xml");
                string pathToProject = Path.Combine(path, projects[listPosition]);

                File.Move(PathToXml, newPathToXml);
                Directory.Move(pathToProject, newPath);

                newPathToXml = Path.Combine(Path.Combine(path, renameProject.Text), $"{renameProject.Text}.xml");

                Project project = Project.GetData(newPathToXml);
                project.Name = renameProject.Text;
                project.Path = renameProject.Text + "\\";
                project.UpdatePaths();
                project.CommitChanges();
                dialogRename.Dismiss();

                Refresh();
            }
        }

        private void ListOfProjects_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
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

        public void Refresh()
        {
            projects.Clear();
            GetProjects();
            listOfProjects.Adapter = new CustomViews.ProjectsListAdapter(projects);
        }
    }
}