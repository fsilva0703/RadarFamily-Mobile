using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RadarFamilyCore.Models;
using RadarFamilyCore.ViewModels.Dto;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using RadarFamilyCore.Models.Utils;

namespace RadarFamilyCore.View
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    //[DesignTimeVisible(false)]
    public partial class NewItemPage : ContentPage
    {
        public DtoUnitTracker Item { get; set; }
        AvatarViewModel viewModel;

        public NewItemPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new AvatarViewModel();

            lblCalculoDistancia.Text = "30";
            lblIntervaloPosicao.Text = "1";
            lblIntervaloPosicaoParado.Text = "30";

            saveButton.Clicked += Save_Clicked;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.IsBusy = true;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            //MessagingCenter.Send(this, "AddItem", Item);

            if (String.IsNullOrEmpty(txtName.Text))
            {
                await DisplayAlert("Atenção", "Digite um nome válido!", "Entendi");
                txtName.Focus();
                return;
            }

            if (String.IsNullOrEmpty(txtLogin.Text))
            {
                await DisplayAlert("Atenção", "Digite um login válido!", "Entendi");
                txtLogin.Focus();
                return;
            }

            ValidaEmail validEmail = new ValidaEmail();

            if (!validEmail.IsValidEmail(txtLogin.Text))
            {
                await DisplayAlert("Atenção", "Esse não é um e-mail válido!", "Entendi");
                txtLogin.Focus();
                return;
            }

            DtoAvatar avatar = new DtoAvatar();
            avatar = (DtoAvatar)ItemsCollectionView.SelectedItem;

            if (avatar == null)
            {
                await DisplayAlert("Atenção", "É necessário escolher um avatar", "Entendi");
                ItemsCollectionView.Focus();
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    waitActivityIndicator.IsRunning = true;

                    var unitTrackerRequest = new DtoUnitTracker
                    {
                        Name = txtName.Text,
                        Login = txtLogin.Text,
                        CalculoDistancia = Convert.ToInt32(lblCalculoDistancia.Text),
                        IntervaloPosicao = Convert.ToInt32(lblIntervaloPosicao.Text),
                        IntervaloPosicaoParado = Convert.ToInt32(lblIntervaloPosicaoParado.Text),
                        IdAdmin = (int)Application.Current.Properties["IdUser"],
                        Avatar = avatar.Name
                    };


                    var jsonRequest = JsonConvert.SerializeObject(unitTrackerRequest);
                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    string uri = "http://radarfamily.somee.com/RadarFamily/admin/UnitTracker/Insert";

                    var retorno = await client.PostAsync(uri, httpContent);

                    var resultString = await retorno.Content.ReadAsStringAsync();

                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await DisplayAlert("Erro", resultString, "Ok");
                        waitActivityIndicator.IsRunning = false;
                        return;
                    }

                    await DisplayAlert("Sucesso.", "Unidade rastreada criada com sucesso!", "Cadastrado");


                    waitActivityIndicator.IsRunning = false;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "Erro ao cadastrar...");
                waitActivityIndicator.IsRunning = false;
                return;
            }


            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
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