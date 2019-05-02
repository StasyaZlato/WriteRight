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
    public class FormFieldsListAdapter : BaseAdapter<string[]>
    {
        public List<string[]> fields = new List<string[]>();
        Activity activity;
        public bool changed;

        //public event EventHandler<CustomEventArgs.FormListViewEventArgs> CollectionChanged;

        public FormFieldsListAdapter(Activity act, List<string[]> fields)
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
            ViewHolder holder;

            if (view == null)
            {
                view = this.activity.LayoutInflater.Inflate(Resource.Layout.FormFieldsListViewRow, parent, false);
                ImageView icon = view.FindViewById<ImageView>(Resource.Id.formFieldIcon);
                EditText fieldNameTV = view.FindViewById<EditText>(Resource.Id.nameOfFieldTV);
                EditText fieldInfoET = view.FindViewById<EditText>(Resource.Id.FieldInfoET);

                holder = new ViewHolder()
                {
                    fieldNameTV = fieldNameTV,
                    fieldInfoET = fieldInfoET,
                    icon = icon,
                };

                view.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)view.Tag;
            }

            holder.Position = position;

            holder.fieldNameTV.Text = item[0];
            holder.fieldInfoET.Text = item[1];


            holder.fieldInfoET.FocusChange += (sender, e) =>
            {
                if (!holder.fieldInfoET.HasFocus)
                {
                    fields[holder.Position][1] = ((EditText)sender).Text;
                    changed = true;
                }
            };

            holder.fieldNameTV.FocusChange += (sender, e) =>
            {
                if (!holder.fieldNameTV.HasFocus)
                {
                    fields[holder.Position][0] = ((EditText)sender).Text;
                    changed = true;
                }
            };

            return view;
        }
    }



    public class ViewHolder : Java.Lang.Object
    {
        public EditText fieldNameTV { get; set; }
        public EditText fieldInfoET { get; set; }
        public ImageView icon { get; set; }
        public int Position { get; set; }
    }

}


