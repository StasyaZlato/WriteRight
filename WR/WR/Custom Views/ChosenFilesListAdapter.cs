﻿using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using ProjectStructure;

namespace WR.CustomViews
{
    public class ChosenFilesListAdapter : BaseAdapter<TextFile>
    {
        private List<TextFile> files;

        public ChosenFilesListAdapter(List<TextFile> files)
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
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListViewChosenFilesRow, parent, false);
            }

            view.FindViewById<ImageView>(Resource.Id.fileIconChosen);
            TextView expF = view.FindViewById<TextView>(Resource.Id.nameOfFileForExportChosen);
            expF.Text = item.PathInProject;
            TextView num = view.FindViewById<TextView>(Resource.Id.numOfFileForExport);
            num.Text = (position + 1).ToString();
            return view;
        }
    }
}