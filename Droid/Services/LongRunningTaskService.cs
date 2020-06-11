using Android.App;
using Android.Content;
using System.Threading.Tasks;
using Android.OS;
using System.Threading;
using Xamarin.Forms;
using RadarFamilyCore.Messages;
using RadarFamilyCore;
using RadarFamilyCore.Droid;

namespace RadarFamily.Droid
{
	[Service]
	public class LongRunningTaskService : Service
	{
		CancellationTokenSource _cts;
		public const int FORSERVICE_NOTIFICATION_ID = 10000;
		public const string CHANNEL_ID = "1";
		public const string MAIN_ACTIVITY_ACTION = "Main_activity";
		public const string PUT_EXTRA = "has_service_been_started";

		public override IBinder OnBind (Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			_cts = new CancellationTokenSource ();

			registerForService();

			Task.Run (() => {
				try {
					//INVOKE THE SHARED CODE
					var counter = new TaskCounter();
					counter.RunCounter(_cts.Token).Wait();
				}
				catch (OperationCanceledException) {
				}
				finally {
					if (_cts.IsCancellationRequested) {
						var message = new CancelledMessage();
						Device.BeginInvokeOnMainThread (
							() => MessagingCenter.Send(message, "CancelledMessage")
						);
					}
				}

			}, _cts.Token);

			return StartCommandResult.Sticky;
		}

		private void registerForService()
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				var channel = new NotificationChannel(CHANNEL_ID, "Channel", NotificationImportance.Default)
				{
					Description = "Rastreamento iniciado"
				};

				var notificationManager = (NotificationManager)GetSystemService(NotificationService);
				notificationManager.CreateNotificationChannel(channel);

				var notification = new Notification.Builder(this, CHANNEL_ID)
				.SetContentTitle("etc")
				.SetContentText("etc")
				.SetSmallIcon(Resource.Drawable.icone)
				.SetContentIntent(BuildIntentToShowMainActivity())
				.SetOngoing(true)
				//.AddAction(BuildRestartTimerAction())
				//.AddAction(BuildStopServiceAction())
				.Build();

				// Enlist this instance of the service as a foreground service
				StartForeground(FORSERVICE_NOTIFICATION_ID, notification);
			}
			else
			{
				var notification = new Notification.Builder(this)
					.SetContentTitle("RADAR FAMILY")
					.SetContentText("Rastreamento inicializado.")
					.SetSmallIcon(Resource.Drawable.icone)
					.SetContentIntent(BuildIntentToShowMainActivity())
					.SetOngoing(true)
					//.AddAction(BuildRestartTimerAction())
					//.AddAction(BuildStopServiceAction())
					.Build();

				// Enlist this instance of the service as a foreground service
				StartForeground(FORSERVICE_NOTIFICATION_ID, notification);
			}
		}

		PendingIntent BuildIntentToShowMainActivity()
		{
			var notificationIntent = new Intent(this, typeof(MainActivity));
			notificationIntent.SetAction(MAIN_ACTIVITY_ACTION);
			notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
			notificationIntent.PutExtra(PUT_EXTRA, true);

			var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
			return pendingIntent;
		}


		public override void OnDestroy ()
		{
			if (_cts != null) {
				_cts.Token.ThrowIfCancellationRequested ();

				_cts.Cancel ();

				// Remove the notification from the status bar.
				var notificationManager = (NotificationManager)GetSystemService(NotificationService);
				notificationManager.Cancel(FORSERVICE_NOTIFICATION_ID);
				StopSelf();
				StopForeground(true);
			}
			base.OnDestroy ();
		}
	}
}