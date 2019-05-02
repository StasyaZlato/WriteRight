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
            FindViewById<RelativeLayout>(Resource.Id.SaveBtnRL).Visibility = ViewStates.Visible;
            formEditor = new Fragments.FormEditorFragment();
            var transaction = SupportFragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.mainScreenFragmentsContainer, formEditor);
            transaction.Commit();
            currentFragment = formEditor;
            ShowFragment(formEditor);
        }
    }
}

