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
using System.IO;
using Jp.Wasabeef;

namespace WR.Activities
{
    [Activity(Label = "Editor", MainLauncher = true, Icon = "@mipmap/icon",
        Theme = "@style/MainTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class EditorActivity : AppCompatActivity
    {
        RichEditor editor;
        TextView preview;

        ImageButton undo, bold, italic, underline, heading1, heading2, heading3,
            alignLeft, alignRight, alignCenter;

        Android.Graphics.Color bcolorOfBtnChosen = new Android.Graphics.Color(255, 67, 0);
        Android.Graphics.Color fcolorOfBtnChosen = new Android.Graphics.Color(255, 255, 255);

        Android.Graphics.Color bcolorOfBtn = new Android.Graphics.Color(250, 250, 250);
        Android.Graphics.Color fcolorOfBtn = new Android.Graphics.Color(0, 0, 0);

        //bool clickedBold, clickedItalic, clickedAlignRight, clickedAlignLeft, clickedAlingCenter,
            //clickedUnderline, clickedH1, clickedH2, clickedH3 = false;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Editor);
            editor = FindViewById<RichEditor>(Resource.Id.richTextEditor);
            editor.SetEditorFontSize(14);
            editor.SetPadding(10, 10, 10, 10);

            preview = FindViewById<TextView>(Resource.Id.preview);

            editor.SetOnTextChangeListener(new RichEditor.OnTextChangeListener((obj) =>
            {
                preview.Text = obj;
            }));

            //redo = FindViewById<ImageButton>(Resource.Id.action_redo);
            //redo.Click += (sender, e) =>
            //{
            //    editor.Redo();
            //};

            undo = FindViewById<ImageButton>(Resource.Id.action_undo);
            undo.Click += (sender, e) =>
            {
                editor.Undo();
            };

            bold = FindViewById<ImageButton>(Resource.Id.action_bold);
            bold.Click += (sender, e) =>
            {
                editor.SetBold();
                //if (!clickedBold)
                //{
                //    bold.SetBackgroundColor(bcolorOfBtnChosen);
                //    clickedBold = true;
                //}
                //else
                //{
                //    bold.SetBackgroundColor(bcolorOfBtn);
                //    clickedBold = false;
                //}
            };

            italic = FindViewById<ImageButton>(Resource.Id.action_italic);
            italic.Click += (sender, e) =>
             {
                 editor.SetItalic();
                 //if (!clickedItalic)
                 //{
                 //    italic.SetBackgroundColor(bcolorOfBtnChosen);
                 //    clickedItalic = true;
                 //}
                 //else
                 //{
                 //    italic.SetBackgroundColor(bcolorOfBtn);
                 //    clickedItalic = false;
                 //}
             };

            heading1 = FindViewById<ImageButton>(Resource.Id.action_heading1);
            heading1.Click += (sender, e) =>
            {
                editor.SetHeading(1);
                //if (!clickedH1)
                //{
                //    heading1.SetBackgroundColor(bcolorOfBtnChosen);
                //    heading2.SetBackgroundColor(bcolorOfBtn);
                //    heading3.SetBackgroundColor(bcolorOfBtn);
                //    clickedH1 = true;
                //    clickedH2 = false;
                //    clickedH3 = false;
                //}
                //else
                //{
                //    heading1.SetBackgroundColor(bcolorOfBtn);
                //    clickedH1 = false;
                //}
            };

            heading2 = FindViewById<ImageButton>(Resource.Id.action_heading2);
            heading2.Click += (sender, e) =>
            {
                editor.SetHeading(2);
                //if (!clickedH1)
                //{
                //    heading2.SetBackgroundColor(bcolorOfBtnChosen);
                //    heading1.SetBackgroundColor(bcolorOfBtn);
                //    heading3.SetBackgroundColor(bcolorOfBtn);
                //    clickedH2 = true;
                //    clickedH1 = false;
                //    clickedH3 = false;
                //}
                //else
                //{
                //    heading2.SetBackgroundColor(bcolorOfBtn);
                //    clickedH2 = false;
                //}
            };

            heading3 = FindViewById<ImageButton>(Resource.Id.action_heading3);
            heading3.Click += (sender, e) =>
            {
                editor.SetHeading(3);
                //if (!clickedH3)
                //{
                //    heading3.SetBackgroundColor(bcolorOfBtnChosen);
                //    heading1.SetBackgroundColor(bcolorOfBtn);
                //    heading2.SetBackgroundColor(bcolorOfBtn);
                //    clickedH3 = true;
                //    clickedH1 = false;
                //    clickedH2 = false;
                //}
                //else
                //{
                //    heading3.SetBackgroundColor(bcolorOfBtn);
                //    clickedH3 = false;
                //}
            };

            FindViewById<ImageButton>(Resource.Id.action_underline).Click += (sender, e) =>
            {
                editor.SetUnderline();
            };

            FindViewById<ImageButton>(Resource.Id.action_align_left).Click += (sender, e) =>
            {
                editor.SetAlignLeft();
            };

            FindViewById<ImageButton>(Resource.Id.action_align_right).Click += (sender, e) =>
            {
                editor.SetAlignRight();
            };

            FindViewById<ImageButton>(Resource.Id.action_align_center).Click += (sender, e) =>
            {
                editor.SetAlignCenter();
            };
        }
    }
}

