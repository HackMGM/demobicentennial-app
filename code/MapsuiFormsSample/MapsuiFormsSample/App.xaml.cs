using Xamarin.Forms;

namespace MapsuiFormsSample
{
    public partial class App
	{
		public App ()
		{
			InitializeComponent();

            // Map page adds extra complexity
            // (and instability of having map jump around if GPS signal is wonky)
            // in trying to make
            // it the primary way user interacts with markers
            // usiing list instead
            //MainPage = new NavigationPage(new MapPage());

            MainPage = new NavigationPage(new MainPage());
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
