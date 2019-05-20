using System;
using System.IO;
using Android.OS;
using Android.Views;
using Android.Widget;
using ProjectStructure;

namespace WR.Fragments
{
    public class CreateProjectFragment : Android.Support.V4.App.Fragment
    {
        public event EventHandler<CustomEventArgs.ProjectEventArgs> ProjectIsCreated;

        private EditText nameOfProjectInput;
        private Spinner genreChoice, themeChoice;
        private TextView themeChoiceTextView, textView4;
        private ArrayAdapter adapterGenre;
        private ArrayAdapter adapterTheme;
        private LinearLayout mainLinearLayout, checkBoxLayout;
        private CheckBox checkBoxText, checkBoxDraft, checkBoxInfo;
        private Button acceptBtn;
        private Project project;

        private string nameOfProject, genre, theme;
        private bool textSection, draftSection, infoSection;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.CreateProjectFragment, container, false);

            nameOfProjectInput = view.FindViewById<EditText>(Resource.Id.NameOfProjectInput);
            genreChoice = view.FindViewById<Spinner>(Resource.Id.GenreOfProjectChoice);
            themeChoice = view.FindViewById<Spinner>(Resource.Id.ThemeOfProjectChoice);
            themeChoiceTextView = view.FindViewById<TextView>(Resource.Id.textView3);
            mainLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.FormLayout);
            textView4 = view.FindViewById<TextView>(Resource.Id.textView4);
            checkBoxLayout = view.FindViewById<LinearLayout>(Resource.Id.CheckBoxesLayout);
            checkBoxText = view.FindViewById<CheckBox>(Resource.Id.checkBoxText);
            checkBoxDraft = view.FindViewById<CheckBox>(Resource.Id.checkBoxDraft);
            checkBoxInfo = view.FindViewById<CheckBox>(Resource.Id.checkBoxInfo);

            acceptBtn = view.FindViewById<Button>(Resource.Id.AcceptProjectInfoBtn);

            mainLinearLayout.RemoveView(themeChoiceTextView);
            mainLinearLayout.RemoveView(themeChoice);

            nameOfProjectInput.TextChanged += NameOfProjectInput_TextChanged;
            genreChoice.ItemSelected += GenreChoice_ItemSelected;
            themeChoice.ItemSelected += ThemeChoice_ItemSelected;
            checkBoxText.CheckedChange += CheckBoxText_CheckedChange;
            checkBoxDraft.CheckedChange += CheckBoxDraft_CheckedChange;
            checkBoxInfo.CheckedChange += CheckBoxInfo_CheckedChange;

            acceptBtn.Click += AcceptBtn_Click;

            adapterGenre = ArrayAdapter.CreateFromResource(this.Context, Resource.Array.Genres, Android.Resource.Layout.SimpleSpinnerItem);
            adapterTheme = ArrayAdapter.CreateFromResource(this.Context, Resource.Array.Theme, Android.Resource.Layout.SimpleSpinnerItem);
            return view;
        }

        private void GenreChoice_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Android.Graphics.Color color = new Android.Graphics.Color(38, 50, 56);
            ((TextView)e.Parent.GetChildAt(0)).SetTextColor(color);
            int res = e.Position;
            switch (res)
            {
                // мне нужно, чтобы порядок элементов строго сохранялся, но при этом 
                // чтобы при выборе художественной литературы появлялась возможность
                // выбрать жанр. С visibility реализовать это не вышло, так что использовала removeview
                case 0:
                    mainLinearLayout.RemoveView(themeChoiceTextView);
                    mainLinearLayout.RemoveView(themeChoice);

                    theme = null;
                    break;
                case 1:
                case 2:
                case 3:
                    mainLinearLayout.RemoveView(textView4);
                    mainLinearLayout.RemoveView(checkBoxLayout);
                    mainLinearLayout.RemoveView(acceptBtn);
                    // здесь может возникнуть только один эксепшен, джавовский, "вид уже существует". Тогда просто игнорим
                    try
                    {
                        mainLinearLayout.AddView(themeChoiceTextView);
                        mainLinearLayout.AddView(themeChoice);
                    }
                    catch (Java.Lang.IllegalStateException) { }

                    mainLinearLayout.AddView(textView4);
                    mainLinearLayout.AddView(checkBoxLayout);
                    mainLinearLayout.AddView(acceptBtn);
                    break;
            }
            genre = adapterGenre.GetItem(res).ToString();
        }

        private void ThemeChoice_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Android.Graphics.Color color = new Android.Graphics.Color(38, 50, 56);
            ((TextView)e.Parent.GetChildAt(0)).SetTextColor(color);
            theme = adapterTheme.GetItem(e.Position).ToString();
        }

        private void NameOfProjectInput_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            nameOfProject = e.Text.ToString();
        }

        private void CheckBoxText_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            textSection = e.IsChecked;
        }

        private void CheckBoxDraft_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            draftSection = e.IsChecked;
        }

        private void CheckBoxInfo_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            infoSection = e.IsChecked;
        }

        private void AcceptBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameOfProject))
            {
                Toast toast = Toast.MakeText(this.Activity, Resource.String.alertCreatingProjectMsgNoName, ToastLength.Short);
                toast.Show();
                return;
            }

            if (!(infoSection || textSection || draftSection))
            {
                Toast toast = Toast.MakeText(this.Activity, Resource.String.alertCreatingProjectMsgNoSections, ToastLength.Short);
                toast.Show();
                return;
            }
            else
            {
                if (Section.CheckInvalidFileName(nameOfProject))
                {
                    Toast.MakeText(this.Activity, "Название содержит недопустимые символы", ToastLength.Short).Show();
                    return;
                }

                string dir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), nameOfProject);

                if (Directory.Exists(dir))
                {
                    Toast.MakeText(this.Activity, "Проект с таким названием уже существует", ToastLength.Short).Show();
                    return;
                }

                Directory.CreateDirectory(dir);
                project = new Project(nameOfProject, genre, theme, textSection, draftSection, infoSection);

                Toast toast = Toast.MakeText(this.Activity, "Проект создан!", ToastLength.Short);
                toast.Show();

                ProjectIsCreated(this, new CustomEventArgs.ProjectEventArgs(project));
            }
        }
    }
}