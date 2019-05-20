using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace WR.CustomViews
{
    public class FormFieldsRemovingListAdapter : BaseAdapter<string[]>
    {
        public List<string[]> fields = new List<string[]>();
        private Activity activity;

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
                    FieldNameTV = fieldNameTV,
                    FieldInfoTV = fieldInfoET,
                    Icon = icon,
                };

                view.Tag = holder;
            }
            else
            {
                holder = (ViewHolderRemoving)view.Tag;
            }

            holder.FieldNameTV.Text = item[0];
            holder.FieldInfoTV.Text = item[1];

            return view;
        }

        public class ViewHolderRemoving : Java.Lang.Object
        {
            public TextView FieldNameTV { get; set; }
            public TextView FieldInfoTV { get; set; }
            public ImageView Icon { get; set; }
        }
    }
}