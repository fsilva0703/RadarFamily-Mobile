using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RadarFamilyCore.Models;
using RadarFamilyCore.View;
using RadarFamilyCore.ViewModels;
using RadarFamilyCore.ViewModels.Dto;
using System.Net.Http;
using Newtonsoft.Json;

namespace RadarFamilyCore.View
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    //[DesignTimeVisible(false)]
    public partial class TrackerUnitPage : ContentPage
    {
        ItemsViewModel viewModel;

        public TrackerUnitPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
        }

        public TrackerUnitPage(MenuItemType ItemTracking)
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, EventArgs args)
        {
            var layout = (BindableObject)sender;
            var item = (DtoUnitTracker)layout.BindingContext;
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()){ BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.IsBusy = true;
        }
    }
}