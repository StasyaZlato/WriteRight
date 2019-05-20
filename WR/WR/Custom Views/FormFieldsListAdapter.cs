using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace WR.CustomViews
{
    public class FormFieldsListAdapter : BaseAdapter<string[]>
    {
        public List<string[]> fields = new List<string[]>();
        private Activity activity;
        public bool changed;

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
                    FieldNameET = fieldNameTV,
                    FieldInfoET = fieldInfoET,
                    Icon = icon,
                };

                view.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)view.Tag;
            }

            holder.Position = position;

            holder.FieldNameET.Text = item[0];
            holder.FieldInfoET.Text = item[1];

            holder.FieldInfoET.FocusChange += (sender, e) =>
            {
                if (!holder.FieldInfoET.HasFocus)
                {
                    fields[holder.Position][1] = ((EditText)sender).Text;
                    changed = true;
                }
            };

            holder.FieldNameET.FocusChange += (sender, e) =>
            {
                if (!holder.FieldNameET.HasFocus)
                {
                    fields[holder.Position][0] = ((EditText)sender).Text;
                    changed = true;
                }
            };

            return view;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public EditText FieldNameET { get; set; }
            public EditText FieldInfoET { get; set; }
            public ImageView Icon { get; set; }
            public int Position { get; set; }
        }
    }
}