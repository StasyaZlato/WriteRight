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

namespace WR.Fragments
{

    public class FormEditorFragment : Android.Support.V4.App.Fragment
    {
        ListView listOfFields, listForRemoving;
        FloatingActionButton fabAddField, fabRemoveFile;
        List<string[]> fieldsOfForm = new List<string[]>();
        CustomViews.FormFieldsListAdapter adapter;
        CustomViews.FormFieldsRemovingListAdapter adapterRemoving;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.FormEditorFragment, container, false);

            var reader = Resources.GetXml(Resource.Xml.hero);
            XDocument doc = XDocument.Load(reader);
            foreach (var field in doc.Element("form").Elements("field"))
            {
                string name = field.Attribute("name").Value;
                fieldsOfForm.Add(new string[2] { name, string.Empty });
            }

            listOfFields = view.FindViewById<ListView>(Resource.Id.listOfFields);
            listForRemoving = view.FindViewById<ListView>(Resource.Id.listOfFieldsRemove);
            fabAddField = view.FindViewById<FloatingActionButton>(Resource.Id.mainActionBtnAddField);
            fabRemoveFile = view.FindViewById<FloatingActionButton>(Resource.Id.mainActionBtnRemoveField);

            adapter = new CustomViews.FormFieldsListAdapter(this.Activity, fieldsOfForm);
            adapterRemoving = new CustomViews.FormFieldsRemovingListAdapter(this.Activity, fieldsOfForm);

            listOfFields.Adapter = adapter;
            listForRemoving.Adapter = adapterRemoving;

            fabAddField.Click += FabAddField_Click;
            fabRemoveFile.Click += FabRemoveFile_Click;

            listForRemoving.ItemClick += ListForRemoving_ItemClick;

            //adapter.CollectionChanged += (sender, e) =>
            //{
            //    fieldsOfForm = e.collection;
            //    adapter = new CustomViews.FormFieldsListAdapter(this.Activity, fieldsOfForm);
            //    adapterRemoving = new CustomViews.FormFieldsRemovingListAdapter(this.Activity, fieldsOfForm);

            //    listOfFields.Adapter = adapter;
            //    listForRemoving.Adapter = adapterRemoving;
            //};

           

            return view;
        }

        void ListForRemoving_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int position = e.Position;
            fieldsOfForm.RemoveAt(position);

            adapter = new CustomViews.FormFieldsListAdapter(this.Activity, fieldsOfForm);
            adapterRemoving = new CustomViews.FormFieldsRemovingListAdapter(this.Activity, fieldsOfForm);

            listOfFields.Adapter = adapter;
            listForRemoving.Adapter = adapterRemoving;
        }


        void FabRemoveFile_Click(object sender, EventArgs e)
        {
            if (adapter.changed)
            {
                fieldsOfForm = adapter.fields;
                adapter = new CustomViews.FormFieldsListAdapter(this.Activity, fieldsOfForm);
                adapterRemoving = new CustomViews.FormFieldsRemovingListAdapter(this.Activity, fieldsOfForm);

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
            fieldsOfForm.Add(new string[2]);
            adapter = new CustomViews.FormFieldsListAdapter(this.Activity, fieldsOfForm);
            listOfFields.Adapter = adapter;
        }


        void SaveBtn_Click(object sender, EventArgs e)
        {

        }

    }

}
