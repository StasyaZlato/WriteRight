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

namespace WR.Fragments
{
    
    public class CreateProjectFragment : Android.Support.V4.App.Fragment
    {
        public event EventHandler<ProjectEventArgs> ProjectIsCreated;

        EditText nameOfProjectInput;
        Spinner genreChoice, themeChoice;
        TextView themeChoiceTextView, textView4;
        ArrayAdapter adapterGenre;
        ArrayAdapter adapterTheme;
        LinearLayout mainLinearLayout, checkBoxLayout;
        CheckBox checkBoxText, checkBoxDraft, checkBoxInfo;
        Button acceptBtn;
        Project project;

        string nameOfProject, genre, theme;
        bool textSection, draftSection, infoSection;


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

        void GenreChoice_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Android.Graphics.Color color = new Android.Graphics.Color(38, 50, 56);
            ((TextView)e.Parent.GetChildAt(0)).SetTextColor(color);
            int res = e.Position;
            switch (res)
            {
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
                    // здесь может возникнуть только один эксепшен, джавовский, "вид уже существует". Тогда просто игнорим
                    try
                    {
                        mainLinearLayout.AddView(themeChoiceTextView);
                        mainLinearLayout.AddView(themeChoice);
                    }
                    catch (Java.Lang.IllegalStateException) { }

                    mainLinearLayout.AddView(textView4);
                    mainLinearLayout.AddView(checkBoxLayout);
                    break;
            }
            genre = adapterGenre.GetItem(res).ToString();
        }

        void ThemeChoice_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Android.Graphics.Color color = new Android.Graphics.Color(38, 50, 56);
            ((TextView)e.Parent.GetChildAt(0)).SetTextColor(color);
            theme = adapterTheme.GetItem(e.Position).ToString();
        }


        void NameOfProjectInput_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            nameOfProject = e.Text.ToString();
        }

        void CheckBoxText_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            textSection = e.IsChecked;
        }

        void CheckBoxDraft_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            draftSection = e.IsChecked;
        }

        void CheckBoxInfo_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            infoSection = e.IsChecked;
        }

        void AcceptBtn_Click(object sender, EventArgs e)
        {
            Dialog dialog;
            if (nameOfProject == null)
            {
                Android.Support.V7.App.AlertDialog.Builder noNameAlert = new Android.Support.V7.App.AlertDialog.Builder(
                    new ContextThemeWrapper(this.Activity, Resource.Style.Theme_AppCompat_Light));
                noNameAlert.SetTitle(Resource.String.alertCreatingProjectTitle);
                noNameAlert.SetMessage(Resource.String.alertCreatingProjectMsgNoName);
                noNameAlert.SetNeutralButton(Resource.String.alertNeutralBTN, (senderAlert, args) => { });
                dialog = noNameAlert.Create();
                dialog.Show();
                return;
            }

            if (!(infoSection || textSection || draftSection))
            {
                Android.Support.V7.App.AlertDialog.Builder noSectionAlert = new Android.Support.V7.App.AlertDialog.Builder(
                    new ContextThemeWrapper(this.Activity, Resource.Style.Theme_AppCompat_Light));
                noSectionAlert.SetTitle(Resource.String.alertCreatingProjectTitle);
                noSectionAlert.SetMessage(Resource.String.alertCreatingProjectMsgNoSections);

                noSectionAlert.SetPositiveButton(Resource.String.alertOkBtn, (senderAlert, args) =>
                {
                    textSection = true;
                    project = new Project(nameOfProject, genre, theme, textSection, draftSection, infoSection);

                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(
                            new ContextThemeWrapper(this.Activity, Resource.Style.Theme_AppCompat_Light));
                    alert.SetTitle("Проект создан!");
                    alert.SetMessage("Ура");
                    alert.SetNeutralButton(Resource.String.alertNeutralBTN, (senderAlert1, args1) => { });
                    dialog = alert.Create();
                    dialog.Show();
                });
                noSectionAlert.SetNegativeButton(Resource.String.alertCancelBtn, (senderAlert, args) => { });
                dialog = noSectionAlert.Create();
                dialog.Show();
            }
            else
            {
                project = new Project(nameOfProject, genre, theme, textSection, draftSection, infoSection);

                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(
                        new ContextThemeWrapper(this.Activity, Resource.Style.Theme_AppCompat_Light));
                alert.SetTitle("Проект создан!");
                alert.SetMessage("Ура");
                alert.SetNeutralButton(Resource.String.alertNeutralBTN, (senderAlert1, args1) => { });
                dialog = alert.Create();
                dialog.Show();

                ProjectIsCreated(this, new ProjectEventArgs(project));
            }
        }

    }
}
