using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
using Java.Lang;

namespace WR.CustomViews
{
    public class FoldersListAdapter : BaseAdapter<Section>
    {
        List<Section> sections;

        public FoldersListAdapter(List<Section> sections)
        {
            this.sections = sections;
        }

        public override Section this[int position] => sections[position];

        public override int Count => sections.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Section item = sections[position];
            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListViewFoldersRow, parent, false);
            }
            view.FindViewById<ImageView>(Resource.Id.folderIcon);
            view.FindViewById<TextView>(Resource.Id.nameOfProjectTextView).Text = item.Name;
            view.FindViewById<TextView>(Resource.Id.DateCreation).Text = item.Created.ToString();
            return view;
        }
    }
}
