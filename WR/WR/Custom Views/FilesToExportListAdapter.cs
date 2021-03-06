﻿using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using ProjectStructure;

namespace WR.CustomViews
{
    public class FilesToExportListAdapter : BaseAdapter<TextFile>
    {
        private List<TextFile> files;

        public FilesToExportListAdapter(List<TextFile> files)
        {
            this.files = files;
        }

        public override TextFile this[int position] => files[position];

        public override int Count => files.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            TextFile item = files[position];

            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListViewFilesToExportRow, parent, false);
            }

            view.FindViewById<ImageView>(Resource.Id.fileIcon);
            TextView expF = view.FindViewById<TextView>(Resource.Id.nameOfFileForExport);
            expF.Text = item.PathInProject;
            ImageView img = view.FindViewById<ImageView>(Resource.Id.CheckBoxFileForExport);
            return view;
        }
    }
}