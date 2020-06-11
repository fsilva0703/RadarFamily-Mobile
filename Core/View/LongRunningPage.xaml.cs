using Xamarin.Forms;
using RadarFamilyCore.Messages;
using System.Diagnostics;
using System;
using RadarFamilyCore.ViewModels.Dto;

namespace RadarFamilyCore
{
	public partial class LongRunningPage : ContentPage
	{
		
		public LongRunningPage (DtoResultLogin user)
		{
			InitializeComponent ();

			lblCronometro.Text = "00:00:00";


			//Wire up XAML buttons
			longRunningTask.Clicked += (s, e) => {
				var message = new StartLongRunningTaskMessage();
				MessagingCenter.Send(message, "StartLongRunningTaskMessage");
			};

			stopLongRunningTask.Clicked += (s, e) => {
				var message = new StopLongRunningTaskMessage ();
				MessagingCenter.Send (message, "StopLongRunningTaskMessage");
				//stopwatch.Stop();
				//lblCronometro.Text = "00:00:00";
				//stopwatch.Reset();
			};
				
			HandleReceivedMessages ();
		}

		void HandleReceivedMessages ()
		{
			MessagingCenter.Subscribe<TickedMessage> (this, "TickedMessage", message => {
				Device.BeginInvokeOnMainThread(() => {
					if(message.Message.Split('|')[0] != ticker.Text && message.Message.Split('|')[0] != "" && message.Message.Split('|')[0] != "0")
						ticker.Text = message.Message.Split('|')[0];
					
					lblCronometro.Text = message.Message.Split('|')[1];

					if (!String.IsNullOrEmpty(message.Message.Split('|')[2]) && message.Message.Split('|')[2] != "")
					{
						lblStatusInternet.Text = "Status internet: sem conexão no momento.";
						lblSemInternet.Text = message.Message.Split('|')[2];
					}
					else
					{
						lblStatusInternet.Text = string.Empty;
						lblSemInternet.Text = string.Empty;
					}


				});
			});

			MessagingCenter.Subscribe<CancelledMessage> (this, "CancelledMessage", message => {
				Device.BeginInvokeOnMainThread(() => {
					ticker.Text = "Rastreamento finalizado";
				});
			});
		}
	}
}