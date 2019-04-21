using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Content;
using System.Collections.Generic;
using ProjectStructure;
using System;
using Jp.Wasabeef;
using System.IO;


namespace WR.Fragments
{

    public class EditorFragment : Android.Support.V4.App.Fragment
    {
        RichEditor editor;

        FileStream fs;
        string path, text;


        //ImageButton undo, bold, italic, underline, heading1, heading2, heading3,
        //    alignLeft, alignRight, alignCenter;

        //Android.Graphics.Color bcolorOfBtnChosen = new Android.Graphics.Color(255, 67, 0);
        //Android.Graphics.Color fcolorOfBtnChosen = new Android.Graphics.Color(255, 255, 255);

        //Android.Graphics.Color bcolorOfBtn = new Android.Graphics.Color(250, 250, 250);
        //Android.Graphics.Color fcolorOfBtn = new Android.Graphics.Color(0, 0, 0);

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.EditorFragment, container, false);
            editor = view.FindViewById<RichEditor>(Resource.Id.richTextEditor);
            editor.SetEditorFontSize(14);
            editor.SetPadding(10, 10, 10, 10);

            text = this.Activity.Intent.GetStringExtra("htmlText");
            if (text != null)
            {
                editor.SetHtml(text);
            }


            //fs = new FileStream(path, FileMode.OpenOrCreate);


            editor.SetOnTextChangeListener(new RichEditor.OnTextChangeListener((obj) =>
            {
                text = editor.GetHtml();
            }));

            view.FindViewById<ImageButton>(Resource.Id.action_undo).Click += (sender, e) =>
            {
                editor.Undo();
            };

            view.FindViewById<ImageButton>(Resource.Id.action_bold).Click += (sender, e) =>
            {
                editor.SetBold();
            };

            view.FindViewById<ImageButton>(Resource.Id.action_italic).Click += (sender, e) =>
            {
                editor.SetItalic();
            };

            view.FindViewById<ImageButton>(Resource.Id.action_heading1).Click += (sender, e) =>
            {
                editor.SetHeading(1);
            };

            view.FindViewById<ImageButton>(Resource.Id.action_heading2).Click += (sender, e) =>
            {
                editor.SetHeading(2);
            };

            view.FindViewById<ImageButton>(Resource.Id.action_heading3).Click += (sender, e) =>
            {
                editor.SetHeading(3);
            };

            view.FindViewById<ImageButton>(Resource.Id.action_underline).Click += (sender, e) =>
            {
                editor.SetUnderline();
            };

            view.FindViewById<ImageButton>(Resource.Id.action_align_left).Click += (sender, e) =>
            {
                editor.SetAlignLeft();
            };

            view.FindViewById<ImageButton>(Resource.Id.action_align_right).Click += (sender, e) =>
            {
                editor.SetAlignRight();
            };

            view.FindViewById<ImageButton>(Resource.Id.action_align_center).Click += (sender, e) =>
            {
                editor.SetAlignCenter();
            };

            return view;
        }

    }

}
