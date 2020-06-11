﻿using Foundation;
using UIKit;
using Xamarin.Forms;
using System;
using RadarFamilyCore.Messages;
using Xamarin.Forms.Platform.iOS;

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
			Xamarin.FormsGoogleMaps.Init("AIzaSyDPxQFP4WSIpMLk_610YoYwxa09i9Xx89c");

			LoadApplication (new App ());

			UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView; 
			if (statusBar != null && statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
			{
				statusBar.BackgroundColor = Color.FromHex("#9370DB").ToUIColor(); // change to your desired color 
			}

			WireUpLongRunningTask ();
			WireUpDownloadTask ();

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