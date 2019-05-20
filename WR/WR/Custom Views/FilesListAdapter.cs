using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using ProjectStructure;

namespace WR.CustomViews
{
    public class FilesListAdapter : BaseAdapter<FileOfProject>
    {
        private List<FileOfProject> files;

        public FilesListAdapter(List<FileOfProject> files)
        {
            this.files = files;
        }

        public override FileOfProject this[int position] => files[position];

        public override int Count => files.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            FileOfProject item = files[position];
            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListViewTextFilesRow, parent, false);
            }
            ImageView img = view.FindViewById<ImageView>(Resource.Id.textFileIcon);
            if (item is FormFile)
            {
                img.SetImageResource(Resource.Drawable.file_table);
            }
            else
            {
                img.SetImageResource(Resource.Drawable.file_document);
            }
            view.FindViewById<TextView>(Resource.Id.textFileName).Text = item.Name;
            return view;
        }
    }
}