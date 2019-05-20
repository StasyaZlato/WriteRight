using System;
using System.IO;
using System.Xml.Serialization;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using ProjectStructure;

namespace WR.Fragments
{
    public class HelloFragment : Fragment
    {
        private EditText firstName, lastName;
        private Button accept;
        public LinearLayout userInfoLL;
        public RelativeLayout helloRL;
        private TextView helloTV;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HelloFragment, container, false);
            firstName = view.FindViewById<EditText>(Resource.Id.firstNameET);
            lastName = view.FindViewById<EditText>(Resource.Id.lastNameET);
            accept = view.FindViewById<Button>(Resource.Id.acceptUserBtn);
            userInfoLL = view.FindViewById<LinearLayout>(Resource.Id.userInfoLL);
            helloRL = view.FindViewById<RelativeLayout>(Resource.Id.HelloFragmentRL);
            helloTV = view.FindViewById<TextView>(Resource.Id.HelloFragmentTV);

            string userPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "user.xml");
            if (File.Exists(userPath))
            {
                userInfoLL.Visibility = ViewStates.Gone;
                helloRL.Visibility = ViewStates.Visible;
                User user;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(User));
                using (FileStream fs = new FileStream(userPath, FileMode.Open))
                {
                    user = (User)xmlSerializer.Deserialize(fs);
                }
                helloTV.Text = $"Добро пожаловать в приложение WriteRight, {user.FirstName} {user.LastName}";
            }

            accept.Click += Accept_Click;

            return view;
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(firstName.Text) && string.IsNullOrEmpty(lastName.Text))
            {
                Toast.MakeText(this.Activity, "данные пользователя отсутствуют!", ToastLength.Short).Show();
            }
            else
            {
                User user = new User(firstName.Text, lastName.Text);
                userInfoLL.Visibility = ViewStates.Gone;
                helloRL.Visibility = ViewStates.Visible;
                helloTV.Text = $"Добро пожаловать в приложение WriteRight, {user.FirstName} {user.LastName}";
                string userPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "user.xml");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(User));
                using (FileStream fs = new FileStream(userPath, FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(fs, user);
                }
                ((Activities.MainActivity)this.Activity).firstName.Text = $"First name: {user.FirstName}";
                ((Activities.MainActivity)this.Activity).lastName.Text = $"Last name: {user.LastName}";
            }
        }
    }
}