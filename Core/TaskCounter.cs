using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;
using RadarFamilyCore.Messages;
using System.Diagnostics;
using System;
using RadarFamilyCore.Service;
using RadarFamilyCore.Models.Utils;

namespace RadarFamilyCore
{
	public class TaskCounter
	{
		Stopwatch stopwatch;
		PositionService servicePosition;
		CheckInternetConnection IConnection;

		public async Task RunCounter(CancellationToken token)
		{
			int i = 0;

			stopwatch = new Stopwatch();
			servicePosition = new PositionService();
			IConnection = new CheckInternetConnection();

			int? minPosAnterior = null;
			string msgPos = string.Empty;
			String msgWithOutInternet = string.Empty;

			await Task.Run (async () => {

			while(true)
			{
					token.ThrowIfCancellationRequested();

					if (!stopwatch.IsRunning)
					{
						stopwatch.Start();
						Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
						{

							TimeSpan ts = stopwatch.Elapsed;
							String timer = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);

							var tmessage = "TickedMessage";
							if (token.IsCancellationRequested == true)
							{
								stopwatch.Stop();
								timer = "00:00:00";
								stopwatch.Reset();
								tmessage = "CancelledMessage";
							}

							var message = new TickedMessage();
							message.Message = "|" + timer + "|";
							if(minPosAnterior != null && minPosAnterior != ts.Minutes)
							{
								Boolean HasInternet = IConnection.HasInternetConnection();

								if (HasInternet)
								{
									i++;
									//Save position
									Task taskPosition = new Task(() => servicePosition.SetPosition());
									taskPosition.Start();

									msgWithOutInternet = string.Empty;
								}
								else
								{
									msgWithOutInternet = "Rastreamento não está sendo realizado por falta de internet.";
								}
								
								if (i == 1)
									msgPos = " posição rastreada.";
								else
									msgPos = " posições rastreadas.";
								
							}

							message.Message = i.ToString() + msgPos +"|"+ timer + "|" + msgWithOutInternet;

							minPosAnterior = ts.Minutes;

							Device.BeginInvokeOnMainThread(() =>
							{
								MessagingCenter.Send<TickedMessage>(message, tmessage);
							});

							if (!stopwatch.IsRunning)
							{
								return false;
							}
							{ return true; }

						});
					}

					//return false;

				}
			}, token);
		}
	}
}