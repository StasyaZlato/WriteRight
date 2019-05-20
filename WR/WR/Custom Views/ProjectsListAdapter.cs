using System.Collections.Generic;
using Android.Views;
using Android.Widget;

namespace WR.CustomViews
{
    public class ProjectsListAdapter : BaseAdapter<string>
    {
        private List<string> projects;

        public ProjectsListAdapter(List<string> projects)
        {
            this.projects = projects;
        }

        public override string this[int position] => projects[position];

        public override int Count => projects.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            string item = projects[position];

            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListViewForProjectRows, parent, false);
            }
            view.FindViewById<ImageView>(Resource.Id.folderIcon);
            view.FindViewById<TextView>(Resource.Id.nameOfProjectTV).Text = item;
            return view;
        }
    }
}