using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using RadarFamilyCore.Messages;
using Android.Locations;
using Android.Runtime;
using Android;
using RadarFamily.Droid;
using Android.Views;
using Xamarin.Forms.GoogleMaps.Android;
using Com.OneSignal;
using FFImageLoading.Forms.Platform;
using Com.OneSignal.Abstractions;

namespace RadarFamilyCore.Droid
{
	[Activity (Label = "RadarFamily", MainLauncher = true, Icon = "@drawable/icone", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{

			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate (savedInstanceState);

			Forms.Init (this, savedInstanceState);

			var platformConfig = new PlatformConfig
			{
				BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
			};

			Xamarin.FormsGoogleMaps.Init(this, savedInstanceState, platformConfig);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);

			if (locationManager.IsProviderEnabled(LocationManager.GpsProvider) == false)
			{
				Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
				AlertDialog alert = dialog.Create();
				alert.SetTitle("RadarFamilyCore: ATIVAR LOCALIZAÇÃO");
				alert.SetMessage("É necessário ativar a localização do celular para que o rastreamento funcione.");
				alert.SetButton("OK", (c, ev) =>
				{
					Intent gpsSettingIntent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
					StartActivity(gpsSettingIntent);
				});
				alert.Show();
			}

			CachedImageRenderer.Init(enableFastRenderer: true);

			OneSignal.Current.StartInit("a3cd15a8-3fde-435d-b210-cb771ba6007b").EndInit();

			LoadApplication(new App());

			Window.SetStatusBarColor(Android.Graphics.Color.MediumPurple);

			WireUpLongRunningTask ();
			WireUpLongDownloadTask ();
        }

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
			{
				RequestPermissions(new string[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 0);
			}

		}

		void WireUpLongRunningTask()
		{
			MessagingCenter.Subscribe<StartLongRunningTaskMessage> (this, "StartLongRunningTaskMessage", message =>  {
				var intent = new Intent(this, typeof(LongRunningTaskService));
				StartService(intent);
			});

			MessagingCenter.Subscribe<StopLongRunningTaskMessage> (this, "StopLongRunningTaskMessage", message =>  {
				var intent = new Intent(this, typeof(LongRunningTaskService));
				StopService(intent);
			});
		}

		void WireUpLongDownloadTask ()
		{
			MessagingCenter.Subscribe<DownloadMessage> (this, "Download", message =>  {
				var intent = new Intent (this, typeof(DownloaderService));
				intent.PutExtra ("url", message.Url);
				StartService (intent);
			});
		}
	}
}