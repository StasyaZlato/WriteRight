using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace WR
{
    public class MyActionBarDrawerToggle : SupportActionBarDrawerToggle
    {
        private AppCompatActivity hostActivity;
        private int openedResource, closedResource;

        public MyActionBarDrawerToggle(AppCompatActivity host, DrawerLayout
         drawer, int openedResource, int closedResource) : base(host, drawer, openedResource, closedResource)
        {
            hostActivity = host;
            this.openedResource = openedResource;
            this.closedResource = closedResource;
        }

        public override void OnDrawerOpened(View drawerView)
        {
            base.OnDrawerOpened(drawerView);
            hostActivity.SupportActionBar.SetTitle(openedResource);
        }

        public override void OnDrawerClosed(View drawerView)
        {
            base.OnDrawerClosed(drawerView);
        }

        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            base.OnDrawerSlide(drawerView, slideOffset);
        }
    }
}