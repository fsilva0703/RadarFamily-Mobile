using Foundation;
using UIKit;
using Xamarin.Forms;
using System;
using RadarFamilyCore.Messages;
using Xamarin.Forms.Platform.iOS;
using FFImageLoading.Forms.Platform;

namespace RadarFamilyCore.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		void WireUpDownloadTask ()
		{
			MessagingCenter.Subscribe<DownloadMessage> (this, "Download", async message => {
				var downloader = new Downloader (message.Url);
				await downloader.DownloadFile ();
			});
		}

		public static Action BackgroundSessionCompletionHandler;

		public override void HandleEventsForBackgroundUrl (UIApplication application, string sessionIdentifier, Action completionHandler)
		{
			Console.WriteLine ("HandleEventsForBackgroundUrl(): " + sessionIdentifier);
			// We get a completion handler which we are supposed to call if our transfer is done.
			BackgroundSessionCompletionHandler = completionHandler;
		}

		#region Methods
		iOSLongRunningTaskExample longRunningTaskExample;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Forms.Init ();
			Xamarin.FormsGoogleMaps.Init("AIzaSyCtM8Szk1kzKUMvi2bPlBmmkzKkuxF2YHs");

			CachedImageRenderer.Init();

			LoadApplication (new App ());

			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
			UIApplication.SharedApplication.SetStatusBarHidden(false, false);

			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });

			WireUpLongRunningTask ();
			WireUpDownloadTask ();

			//OneSignal.Current.StartInit("f8ccd775-3a9c-47a4-aed0-58581056c335").EndInit();

			return base.FinishedLaunching (app, options);
		}

		void WireUpLongRunningTask ()
		{
			MessagingCenter.Subscribe<StartLongRunningTaskMessage> (this, "StartLongRunningTaskMessage", async message => {
				longRunningTaskExample = new iOSLongRunningTaskExample ();
				await longRunningTaskExample.Start ();
			});

			MessagingCenter.Subscribe<StopLongRunningTaskMessage> (this, "StopLongRunningTaskMessage", message => {
				longRunningTaskExample.Stop ();
			});
		}
		#endregion
	}
}