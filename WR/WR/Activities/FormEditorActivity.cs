using Android.App;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using ProjectStructure;
using System;
using SupportFragment = Android.Support.V4.App.Fragment;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using Jp.Wasabeef;

namespace WR.Activities
{
    [Activity(Label = "Editor", MainLauncher = false, Icon = "@mipmap/icon",
        Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FormEditorActivity : MainActivity
    {
        Fragments.FormEditorFragment formEditor;
        public ImageButton saveBtn;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            saveBtn = FindViewById<ImageButton>(Resource.Id.SaveBtn);
            saveBtn.Visibility = ViewStates.Visible;

            formEditor = new Fragments.FormEditorFragment();

            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mainScreenFragmentsContainer, formEditor);
            transaction.Commit();

            currentFragment = formEditor;

            leftMenu.ItemClick -= LeftMenu_ItemClick;
            leftMenu.ItemClick += LeftMenu_ItemClick1;

            changeUser.Click += ChangeUser_Click;

            SupportActionBar.SetTitle(Resource.String.formEditorTitle);
        }

        void ChangeUser_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("frag", "userCh");
            StartActivity(intent);
        }

        void LeftMenu_ItemClick1(object sender, AdapterView.ItemClickEventArgs e)
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

        public override void OnBackPressed()
        {
            formEditor.form.SaveToFile();
            base.OnBackPressed();
        }

    }

}

