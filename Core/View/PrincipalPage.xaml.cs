using Newtonsoft.Json;
using RadarFamilyCore.Models.Utils;
using RadarFamilyCore.ViewModels.Dto;
using System;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RadarFamilyCore.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrincipalPage : ContentPage
    {
        public PrincipalPage()
        {
            InitializeComponent();
            //this.Padding = Device.OnPlatform(
            //    new Thickness(5, 10, 5, 5),
            //    new Thickness(5),
            //    new Thickness(5)
            //    );

            saveButton.Clicked += Save_Clicked;
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

            if (String.IsNullOrEmpty(txtSenha.Text))
            {
                await DisplayAlert("Atenção", "Digite uma senha válida!", "Entendi");
                txtSenha.Focus();
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

                    var loginRequest = new DtoLogin
                    {
                        Name = txtName.Text,
                        Login = txtLogin.Text,
                        Password = txtSenha.Text,
                        Avatar = avatar.Name
                    };


                    var jsonRequest = JsonConvert.SerializeObject(loginRequest);
                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    string uri = "http://207.180.246.227:8095/admin/Login/Insert";

                    var retorno = await client.PostAsync(uri, httpContent);

                    var resultString = await retorno.Content.ReadAsStringAsync();

                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await DisplayAlert("Erro", resultString, "Ok");
                        waitActivityIndicator.IsRunning = false;
                        return;
                    }

                    await DisplayAlert("Sucesso.", "Conta criada com sucesso", "Registrado");

                    if (Application.Current.Properties.ContainsKey("login") && Application.Current.Properties.ContainsKey("senha"))
                    {
                        Application.Current.Properties["login"] = loginRequest.Login;
                        Application.Current.Properties["senha"] = loginRequest.Password;
                    }
                    else
                    {
                        Application.Current.Properties.Add("login", loginRequest.Login);
                        Application.Current.Properties.Add("senha", loginRequest.Password);
                    }

                    waitActivityIndicator.IsRunning = false;

                    Application.Current.MainPage = new LoginPage();

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
    }
}