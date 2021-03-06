﻿using System;
using System.IO;
using Android.OS;
using Android.Views;
using Android.Widget;
using Jp.Wasabeef;

namespace WR.Fragments
{
    public class EditorFragment : Android.Support.V4.App.Fragment
    {
        private RichEditor editor;

        private string path, text;

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

            ((Activities.EditorActivity)this.Activity).saveBtn.Click += SaveBtn_Click;

            path = this.Activity.Intent.GetStringExtra("path");

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                text = sr.ReadToEnd();
            }

            if (text != null)
            {
                editor.SetHtml(text);
            }

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

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SaveText();

            Toast toast = Toast.MakeText(this.Activity, "Сохранено!", ToastLength.Short);
            toast.Show();
        }

        public void SaveText()
        {
            text = editor.GetHtml();

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(text);
            }
        }
    }
}