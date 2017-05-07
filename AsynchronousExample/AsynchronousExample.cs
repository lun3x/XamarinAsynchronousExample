using Xamarin.Forms;

namespace AsynchronousExample
{
	public class App : Application
	{
		public App()
		{
			MainPage = new NavigationPage(new MainPage());
		}
	}
}
