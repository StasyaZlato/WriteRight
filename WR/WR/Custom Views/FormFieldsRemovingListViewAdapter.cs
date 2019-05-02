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
    public class FormFieldsRemovingListAdapter : BaseAdapter<string[]>
    {
        public List<string[]> fields = new List<string[]>();
        Activity activity;

        public FormFieldsRemovingListAdapter(Activity act, List<string[]> fields)
        {
            this.fields = fields;
            activity = act;
        }

        public override string[] this[int position] => fields[position];

        public override int Count => fields.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            string[] item = fields[position];
            View view = convertView;
            ViewHolderRemoving holder;

            if (view == null)
            {
                view = this.activity.LayoutInflater.Inflate(Resource.Layout.ListViewRemovingFieldRow, parent, false);
                ImageView icon = view.FindViewById<ImageView>(Resource.Id.formFieldIconRed);
                TextView fieldNameTV = view.FindViewById<TextView>(Resource.Id.nameOfFieldForRemove);
                TextView fieldInfoET = view.FindViewById<TextView>(Resource.Id.FieldInfoForRemove);

                holder = new ViewHolderRemoving()
                {
                    fieldNameTV = fieldNameTV,
                    fieldInfoET = fieldInfoET,
                    icon = icon,
                };

                view.Tag = holder;
            }
            else
            {
                holder = (ViewHolderRemoving)view.Tag;
            }

            holder.fieldNameTV.Text = item[0];
            holder.fieldInfoET.Text = item[1];

            return view;
        }

    }



    public class ViewHolderRemoving : Java.Lang.Object
    {
        public TextView fieldNameTV { get; set; }
        public TextView fieldInfoET { get; set; }
        public ImageView icon { get; set; }
    }

}


