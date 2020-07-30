using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RadarFamilyCore.View
{
    public class SplashPage : ContentPage
    {
        Image splashImage;

        public SplashPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            var sub = new AbsoluteLayout();
            splashImage = new Image
            {
                Source = "logo_new.jpg",
                WidthRequest = 100,
                HeightRequest = 100
            };

            AbsoluteLayout.SetLayoutFlags(splashImage, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(splashImage, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            sub.Children.Add(splashImage);

            this.BackgroundColor = Color.White;
            this.Content = sub;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await splashImage.ScaleTo(0.6, 1000);
            await splashImage.ScaleTo(0.13, 500, Easing.Linear);
            await splashImage.ScaleTo(0.9, 500, Easing.Linear);
            Application.Current.MainPage = new NavigationPage(new LoginPage()) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White };
        }
    }
}
