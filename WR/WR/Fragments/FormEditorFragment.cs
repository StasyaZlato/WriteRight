using System;
using System.IO;
using System.Xml.Linq;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using ProjectStructure;

namespace WR.Fragments
{
    public class FormEditorFragment : Android.Support.V4.App.Fragment
    {
        private ListView listOfFields, listForRemoving;
        private FloatingActionButton fabAddField, fabRemoveFile;
        private CustomViews.FormFieldsListAdapter adapter;
        private CustomViews.FormFieldsRemovingListAdapter adapterRemoving;
        private TextView templateTV;
        private Dialog dialog;

        private ListView templates;

        private Project project;

        public FormFile form;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.FormEditorFragment, container, false);

            form = JsonConvert.DeserializeObject<FormFile>(this.Activity.Intent.GetStringExtra("form"));

            form.ReadFromFile();

            string projectName = Path.GetFileName(Path.GetDirectoryName(form.PathToFile));
            string projectXml = Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), projectName), $"{projectName}.xml");

            project = Project.GetData(projectXml);

            listOfFields = view.FindViewById<ListView>(Resource.Id.listOfFields);
            listForRemoving = view.FindViewById<ListView>(Resource.Id.listOfFieldsRemove);
            fabAddField = view.FindViewById<FloatingActionButton>(Resource.Id.mainActionBtnAddField);
            fabRemoveFile = view.FindViewById<FloatingActionButton>(Resource.Id.mainActionBtnRemoveField);
            templateTV = view.FindViewById<TextView>(Resource.Id.OpenTemplateTV);

            RegisterForContextMenu(listOfFields);

            if (form.fields.Count > 0)
            {
                templateTV.Visibility = ViewStates.Gone;
            }

            adapter = new CustomViews.FormFieldsListAdapter(this.Activity, form.fields);
            adapterRemoving = new CustomViews.FormFieldsRemovingListAdapter(this.Activity, form.fields);

            listOfFields.Adapter = adapter;
            listForRemoving.Adapter = adapterRemoving;

            fabAddField.Click += FabAddField_Click;
            fabRemoveFile.Click += FabRemoveFile_Click;

            listForRemoving.ItemClick += ListForRemoving_ItemClick;

            ((Activities.FormEditorActivity)this.Activity).saveBtn.Click += SaveBtn_Click;

            templateTV.Click += TemplateTV_Click;

            return view;
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            menu.Add(Resource.String.ContextMenuAddToGlossary);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            int listPosition = info.Position;
            switch (item.ToString())
            {
                case "Добавить в глоссарий":
                    AddToGlossary(listPosition);
                    Toast.MakeText(this.Activity, "Добавлено в глоссарий", ToastLength.Short).Show();
                    break;

                default:
                    break;
            }
            return base.OnContextItemSelected(item);
        }

        private void AddToGlossary(int listPosition)
        {
            FormFile gloss;
            if (project.GlossaryExists)
            {
                Toast.MakeText(this.Activity, "Будет добавлено в существующий файл глоссария", ToastLength.Short).Show();
                gloss = (FormFile)project.files[project.files.FindIndex((obj) =>
                {
                    if (obj.Name == "Глоссарий" && obj is FormFile)
                    {
                        return true;
                    }
                    return false;
                })];
                gloss.ReadFromFile();
            }
            else
            {
                string pathToFile = System.IO.Path.Combine(
                    System.IO.Path.Combine(
                        System.Environment.GetFolderPath(
                            System.Environment.SpecialFolder.MyDocuments),
                        project.Name),
                    $"{project.CurrentFile}.xml");
                gloss = new FormFile("Глоссарий", pathToFile, project.CurrentFile++);
                project.AddFile(gloss);
                project.CommitChanges();
            }
            gloss.fields.Add(form.fields[listPosition]);
            gloss.SaveToFile();
        }

        private void TemplateTV_Click(object sender, EventArgs e)
        {
            ShowPopUpTemplates();
        }

        private void ShowPopUpTemplates()
        {
            dialog = new Dialog(this.Activity);
            // the exception appears if the dialog has been created earlier
            try
            {
                dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            }
            catch (Android.Util.AndroidRuntimeException) { }

            dialog.SetContentView(Resource.Layout.CustomPopUpTemplates);
            ImageButton closeBtn = dialog.FindViewById<ImageButton>(Resource.Id.closePopUp);
            templates = dialog.FindViewById<ListView>(Resource.Id.listViewTemplates);

            closeBtn.Click += (sender, e) =>
            {
                dialog.Dismiss();
            };

            templates.ItemClick += Templates_ItemClick;

            dialog.Show();
        }

        private void Templates_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    var reader = Resources.GetXml(Resource.Xml.hero);
                    XDocument doc = XDocument.Load(reader);
                    foreach (var field in doc.Element("form").Elements("field"))
                    {
                        string name = field.Attribute("name").Value;
                        form.fields.Add(new string[2] { name, string.Empty });
                    }
                    listOfFields.Adapter = new CustomViews.FormFieldsListAdapter(this.Activity, form.fields);
                    form.SaveToFile();
                    dialog.Dismiss();
                    templateTV.Visibility = ViewStates.Gone;
                    break;
            }
        }

        private void ListForRemoving_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int position = e.Position;
            form.fields.RemoveAt(position);

            adapter = new CustomViews.FormFieldsListAdapter(this.Activity, form.fields);
            adapterRemoving = new CustomViews.FormFieldsRemovingListAdapter(this.Activity, form.fields);

            listOfFields.Adapter = adapter;
            listForRemoving.Adapter = adapterRemoving;

            if (form.fields.Count == 0)
            {
                templateTV.Visibility = ViewStates.Visible;
            }
        }

        private void FabRemoveFile_Click(object sender, EventArgs e)
        {
            if (adapter.changed)
            {
                form.fields = adapter.fields;
                adapter = new CustomViews.FormFieldsListAdapter(this.Activity, form.fields);
                adapterRemoving = new CustomViews.FormFieldsRemovingListAdapter(this.Activity, form.fields);

                listOfFields.Adapter = adapter;
                listForRemoving.Adapter = adapterRemoving;
                adapter.changed = false;
            }

            listOfFields.Visibility = ViewStates.Gone;
            listForRemoving.Visibility = ViewStates.Visible;
            fabRemoveFile.SetImageResource(Resource.Drawable.check);
            fabAddField.Visibility = ViewStates.Gone;

            fabRemoveFile.Click -= FabRemoveFile_Click;
            fabRemoveFile.Click += FabRemoveFile_Click1;
        }

        private void FabRemoveFile_Click1(object sender, EventArgs e)
        {
            listOfFields.Visibility = ViewStates.Visible;
            listForRemoving.Visibility = ViewStates.Gone;
            fabAddField.Visibility = ViewStates.Visible;
            fabRemoveFile.SetImageResource(Resource.Drawable.delete);

            fabRemoveFile.Click -= FabRemoveFile_Click1;
            fabRemoveFile.Click += FabRemoveFile_Click;
        }

        private void FabAddField_Click(object sender, EventArgs e)
        {
            form.fields.Add(new string[2]);
            adapter = new CustomViews.FormFieldsListAdapter(this.Activity, form.fields);
            listOfFields.Adapter = adapter;
            if (form.fields.Count > 0)
            {
                templateTV.Visibility = ViewStates.Gone;
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            form.SaveToFile();
            Toast.MakeText(this.Activity, "Файл сохранен", ToastLength.Short).Show();
        }
    }
}