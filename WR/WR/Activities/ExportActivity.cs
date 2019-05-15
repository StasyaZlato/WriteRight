
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WR.Activities
{
    [Activity(Label = "Export", MainLauncher = false, Icon = "@mipmap/icon",
        Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ExportActivity : MainActivity
    {
        Fragments.ExportFragment fragExport;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            fragExport = new Fragments.ExportFragment();

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mainScreenFragmentsContainer, fragExport);
            transaction.Commit();
            currentFragment = fragExport;
            ShowFragment(fragExport);


            leftMenu.ItemClick -= LeftMenu_ItemClick;
            leftMenu.ItemClick += LeftMenu_ItemClick1;

            changeUser.Click += ChangeUser_Click;

            SupportActionBar.SetTitle(Resource.String.export);
            currentTitleOfActionBar = Resources.GetString(Resource.String.export);
        }

        void ChangeUser_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("frag", "userCh");
            StartActivity(intent);
        }

        private void LeftMenu_ItemClick1(object sender, AdapterView.ItemClickEventArgs e)
        {
            int itemSelected = e.Position;
            Intent intent = new Intent(this, typeof(MainActivity));
            switch (itemSelected)
            {
                case 0:
                    intent.PutExtra("frag", "hello");
                    break;
                case 1:
                    intent.PutExtra("frag", "create");
                    break;
                case 2:
                    intent.PutExtra("frag", "open");
                    break;
                case 3:
                    intent.PutExtra("frag", "info");
                    break;
            }
            StartActivity(intent);
            drawerLayout.CloseDrawer(leftDrawer);
        }
    }
}
