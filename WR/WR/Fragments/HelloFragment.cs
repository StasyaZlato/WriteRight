﻿using Android.OS;
using Android.Views;
using Android.Support.V4.App;

namespace WR
{
    public class HelloFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HelloFragment, container, false);
            return view;
        }
    }
}