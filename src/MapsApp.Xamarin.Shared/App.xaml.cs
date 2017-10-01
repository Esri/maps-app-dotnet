
using Xamarin.Forms;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin
{

    public partial class App : Application
	{
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Xamarin.StartPage());
            NavigationPage.SetHasNavigationBar(MainPage, false);
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
