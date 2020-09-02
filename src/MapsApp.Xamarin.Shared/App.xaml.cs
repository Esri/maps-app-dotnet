
using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin.Themes;
using Xamarin.Forms;

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin
{
    public partial class App : Application
	{
        public static ResourceDictionary SelectedTheme = new ResourceDictionary();
        public App()
        {
            InitializeComponent();

            Resources.MergedDictionaries.Add(SelectedTheme);

            SetAppTheme();

            var navPage = new NavigationPage(new Xamarin.StartPage());
            navPage.SetDynamicResource(NavigationPage.BarBackgroundColorProperty, "AccentColor");
            NavigationPage.SetHasNavigationBar(navPage, false);

            MainPage = navPage;

            RequestedThemeChanged += (s, a) =>
            {
                SetAppTheme();
            };
        }

        private void SetAppTheme()
        {
            SelectedTheme.MergedDictionaries.Clear();
            switch (RequestedTheme)
            {
                case OSAppTheme.Dark:
                    SelectedTheme.MergedDictionaries.Add(new DarkTheme());
                    break;
                case OSAppTheme.Light:
                case OSAppTheme.Unspecified:
                default:
                    SelectedTheme.MergedDictionaries.Add(new LightTheme());
                    break;
            }
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
