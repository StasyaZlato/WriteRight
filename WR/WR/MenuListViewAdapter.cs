using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Content;
using System.Collections.Generic;

namespace WR
{
    public class MenuListViewAdapter : BaseAdapter<string>
    {
        List<string> listOfItems;
        Context context;

        public MenuListViewAdapter(Context context, List<string> items)
        {
            listOfItems = items;
            this.context = context;
        }

        public override string this[int position]
        {
            get => listOfItems[position];
        }

        public override int Count { get => listOfItems.Count; }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.MenuListView, null, false);
            }

            TextView txt = row.FindViewById<TextView>(Resource.Id.textViewForMenuList);
            txt.Text = listOfItems[position];

            return row;
        }
    }
}
