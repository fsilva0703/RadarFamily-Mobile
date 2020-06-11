using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RadarFamilyCore.Models;
using RadarFamilyCore.ViewModels;
using RadarFamilyCore.ViewModels.Dto;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace RadarFamilyCore.View
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    //[DesignTimeVisible(false)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
            saveButton.Clicked += Save_ButtonClicked;
        }

        [Obsolete]
        private async void Save_ButtonClicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text))
            {
                await DisplayAlert("Erro", "Digite um nome válido!", "Aceitar");
                txtName.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtLogin.Text))
            {
                await DisplayAlert("Erro", "Digite um login válido!", "Aceitar");
                txtLogin.Focus();
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    waitActivityIndicator.IsRunning = true;

                    var unitTrackerRequest = new DtoUnitTracker
                    {
                        IdUser = this.viewModel.Item.IdUser,
                        Name  = txtName.Text,
                        Login = txtLogin.Text,
                        CalculoDistancia = Convert.ToInt32(lblCalculoDistancia.Text),
                        IntervaloPosicao = Convert.ToInt32(lblIntervaloPosicao.Text),
                        IntervaloPosicaoParado = Convert.ToInt32(lblIntervaloPosicaoParado.Text)
                    };

                    var jsonRequest = JsonConvert.SerializeObject(unitTrackerRequest);
                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    string uri = "http://radarfamily.somee.com/RadarFamily/admin/UnitTracker/Update";

                    var retorno = await client.PostAsync(uri, httpContent);

                    var resultString = await retorno.Content.ReadAsStringAsync();

                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await DisplayAlert("Erro", resultString, "Ok");
                        waitActivityIndicator.IsRunning = false;
                        return;
                    }

                    await DisplayAlert("Sucesso.", "Unidade rastreada atualizada com sucesso!", "Atualizado");

                    
                    waitActivityIndicator.IsRunning = false;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "Erro ao atualizar...");
                waitActivityIndicator.IsRunning = false;
                return;
            }

        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new DtoUnitTracker
            {
                Name = "Item 1",
                Login = "This is an item description."
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            int value = (int)args.NewValue;
            lblCalculoDistancia.Text = value.ToString();
        }

        private void OnSlider1ValueChanged(object sender, ValueChangedEventArgs args)
        {
            int value = (int)args.NewValue;
            lblIntervaloPosicao.Text = value.ToString();
        }

        private void OnSlider2ValueChanged(object sender, ValueChangedEventArgs args)
        {
            int value = (int)args.NewValue;
            lblIntervaloPosicaoParado.Text = value.ToString();
        }
    }
}