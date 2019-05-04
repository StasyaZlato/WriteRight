using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using ProjectStructure;
using System;
using Jp.Wasabeef;
using System.IO;
using Android.Support.Design.Widget;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace WR.Fragments
{

    public class FormEditorFragment : Android.Support.V4.App.Fragment
    {
        ListView listOfFields, listForRemoving;
        FloatingActionButton fabAddField, fabRemoveFile;
        //List<string[]> fieldsOfForm = new List<string[]>();
        CustomViews.FormFieldsListAdapter adapter;
        CustomViews.FormFieldsRemovingListAdapter adapterRemoving;
        TextView templateTV;
        Dialog dialog;

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

            listOfFields = view.FindViewById<ListView>(Resource.Id.listOfFields);
            listForRemoving = view.FindViewById<ListView>(Resource.Id.listOfFieldsRemove);
            fabAddField = view.FindViewById<FloatingActionButton>(Resource.Id.mainActionBtnAddField);
            fabRemoveFile = view.FindViewById<FloatingActionButton>(Resource.Id.mainActionBtnRemoveField);
            templateTV = view.FindViewById<TextView>(Resource.Id.OpenTemplateTV);

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

        void TemplateTV_Click(object sender, EventArgs e)
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
            ListView templates = dialog.FindViewById<ListView>(Resource.Id.listViewTemplates);

            closeBtn.Click += (sender, e) =>
            {
                dialog.Dismiss();
            };

            templates.ItemClick += Templates_ItemClick; ;

            dialog.Show();
        }

        void Templates_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
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


        void ListForRemoving_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
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


        void FabRemoveFile_Click(object sender, EventArgs e)
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

        void FabRemoveFile_Click1(object sender, EventArgs e)
        {
            listOfFields.Visibility = ViewStates.Visible;
            listForRemoving.Visibility = ViewStates.Gone;
            fabAddField.Visibility = ViewStates.Visible;
            fabRemoveFile.SetImageResource(Resource.Drawable.delete);

            fabRemoveFile.Click -= FabRemoveFile_Click1;
            fabRemoveFile.Click += FabRemoveFile_Click;
        }


        void FabAddField_Click(object sender, EventArgs e)
        {
            form.fields.Add(new string[2]);
            adapter = new CustomViews.FormFieldsListAdapter(this.Activity, form.fields);
            listOfFields.Adapter = adapter;
            if (form.fields.Count > 0)
            {
                templateTV.Visibility = ViewStates.Gone;
            }
        }


        void SaveBtn_Click(object sender, EventArgs e)
        {
            form.SaveToFile();
            Toast.MakeText(this.Activity, "Файл сохранен", ToastLength.Short).Show();
        }
    }
}
