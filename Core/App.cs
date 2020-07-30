using Xamarin.Forms;
using RadarFamilyCore.View;
using Xamarin.Essentials;
using RadarFamilyCore.Service;
using RadarFamilyCore.Resources;
using Plugin.Multilingual;
using Com.OneSignal;

namespace RadarFamilyCore
{
	public class App : Application
	{
		#region Constructor
		//private readonly BackgroundPage _backgroundPage;
		public static string AzureBackendUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
		public static bool UseMockDataStore = true;

		public App ()
		{
			//_backgroundPage = new BackgroundPage ();

			//var tabbedPage = new TabbedPage ();
			//tabbedPage.Children.Add (_backgroundPage);
			//tabbedPage.Children.Add (new PrincipalPage ());
			//tabbedPage.Children.Add (new PrincipalPage());
			if (UseMockDataStore)
				DependencyService.Register<MockDataStore>();
			else
				DependencyService.Register<AzureDataStore>();

			AppResources.Culture = CrossMultilingual.Current.DeviceCultureInfo;

			OneSignal.Current.StartInit("a3cd15a8-3fde-435d-b210-cb771ba6007b").EndInit();

			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjg1Mzc1QDMxMzgyZTMyMmUzMGFHUTdTdTE5ODNucDFDeFZMMFhNY0hXOVdMbURCUWhPNjlvVjRJSEJ2Vkk9");

			MainPage = new NavigationPage(new SplashPage()) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White };
		}
		#endregion

		protected override void OnStart ()
		{
			//LoadPersistedValues ();
			OneSignal.Current.RegisterForPushNotifications();
		}

		protected override void OnSleep ()
		{
			//Application.Current.Properties["SleepDate"] = DateTime.Now.ToString("O");
			//Application.Current.Properties["FirstName"] = _backgroundPage.FirstName;
		}

		protected override void OnResume ()
		{
			//LoadPersistedValues ();
		}

		private void LoadPersistedValues()
		{
			//if (Application.Current.Properties.ContainsKey("SleepDate"))
			//{
			//	var value = (string)Application.Current.Properties["SleepDate"];
			//	DateTime sleepDate;
			//	if (DateTime.TryParse(value, out sleepDate))
			//	{
			//		_backgroundPage.SleepDate = sleepDate;
			//	}
			//}

			//if (Application.Current.Properties.ContainsKey("FirstName"))
			//{
			//	var firstName = (string)Application.Current.Properties["FirstName"];
			//	_backgroundPage.FirstName = firstName;
			//}
		}
			
	}
}