using System;
using Xamarin.Forms;
using RadarFamilyCore.Messages;

namespace RadarFamilyCore.View
{
	public partial class BackgroundPage : ContentPage
	{
		public BackgroundPage ()
		{
			InitializeComponent ();
		}

		public DateTime SleepDate
		{
			set { 
				this.SleepDateLabel.Text = value.ToString ("t");
			}
		}

		public string FirstName
		{
			get {
				return this.FirstNameEntry.Text;
			}
			set {
				this.FirstNameEntry.Text = value;
			}
		}
	}
}