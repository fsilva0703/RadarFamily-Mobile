using Newtonsoft.Json;
using RadarFamilyCore.View;
using RadarFamilyCore.ViewModels.Dto;
using System;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RadarFamilyCore
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            if (Application.Current.Properties.ContainsKey("login") && Application.Current.Properties.ContainsKey("senha"))
            {
                txtLogin.Text = Application.Current.Properties["login"].ToString();
                txtSenha.Text = Application.Current.Properties["senha"].ToString();
                lembrarmeSwitch.IsToggled = true;
                lembrarmeSwitch.OnColor = Color.Gray;
                lembrarmeSwitch.ThumbColor = Color.Green;
            }
            else
            {
                lembrarmeSwitch.IsToggled = false;
                lembrarmeSwitch.ThumbColor = Color.Gray;
            }

            loginButton.Clicked += Login_ButtonClicked;
        }

        private async void Login_ButtonClicked(object sender, EventArgs e)
        {
            waitActivityIndicator.IsRunning = true;

            if (String.IsNullOrEmpty(txtLogin.Text))
            {
                await DisplayAlert("Erro", "Digite um login válido!", "Aceitar");
                txtLogin.Focus();
                waitActivityIndicator.IsRunning = false;
                return;
            }

            if (String.IsNullOrEmpty(txtSenha.Text))
            {
                await DisplayAlert("Erro", "Digite uma senha válida!", "Aceitar");
                txtSenha.Focus();
                waitActivityIndicator.IsRunning = false;
                return;
            }

            this.LogarAsync(txtLogin.Text, txtSenha.Text);
        }

        public async void LogarAsync(string login, string senha)
        {

            try
            {
                using (var client = new HttpClient())
                {

                    DtoLogin loginRequest = new DtoLogin()
                    {
                        Login = login,
                        Password = senha,
                    };

                    var jsonRequest = JsonConvert.SerializeObject(loginRequest);
                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    string uri = "http://207.180.246.227:8095/admin/Login/GetLogin";

                    var retorno = client.PostAsync(uri, httpContent).Result;

                    var resultString = retorno.Content.ReadAsStringAsync().Result;

                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await DisplayAlert("Erro", "Usuário ou Senha Incorretos", "Ok");
                        waitActivityIndicator.IsRunning = false;
                        return;
                    }

                    DtoResultLogin user = new DtoResultLogin();
                    user = JsonConvert.DeserializeObject<DtoResultLogin>(resultString);

                    await DisplayAlert("Olá " + user.Nome.ToUpper() + "!", "Seja bem-vindo ao Radar Family!", "Acessar");

                    if (Application.Current.Properties.ContainsKey("login") && Application.Current.Properties.ContainsKey("senha") && lembrarmeSwitch.IsToggled)
                    {
                        Application.Current.Properties["login"] = user.Login;
                        Application.Current.Properties["senha"] = user.Password;
                    }
                    else if (lembrarmeSwitch.IsToggled)
                    {
                        Application.Current.Properties.Add("login", user.Login);
                        Application.Current.Properties.Add("senha", user.Password);
                    }

                    if (Application.Current.Properties.ContainsKey("IdUnidadeRastreada"))
                    {
                        Application.Current.Properties["IdUnidadeRastreada"] = user.IdUnidadeRastreada;
                        Application.Current.Properties["Name"] = user.Nome;
                        Application.Current.Properties["IdUser"] = user.IdUser;
                        Application.Current.Properties["IdAdmin"] = user.IdAdmin;
                        Application.Current.Properties["IsAdmin"] = user.IsAdmin;
                        Application.Current.Properties["CalculoDistancia"] = user.CalculoDistancia;
                        Application.Current.Properties["IntervaloPosicao"] = user.IntervaloPosicao;
                        Application.Current.Properties["Avatar"] = user.Avatar;
                    }
                    else
                    {
                        Application.Current.Properties.Add("IdUnidadeRastreada", user.IdUnidadeRastreada);
                        Application.Current.Properties.Add("Name", user.Nome);
                        Application.Current.Properties.Add("IdUser", user.IdUser);
                        Application.Current.Properties.Add("IdAdmin", user.IdAdmin);
                        Application.Current.Properties.Add("IsAdmin", user.IsAdmin);
                        Application.Current.Properties.Add("CalculoDistancia", user.CalculoDistancia);
                        Application.Current.Properties.Add("IntervaloPosicao", user.IntervaloPosicao);
                        Application.Current.Properties.Add("Avatar", user.Avatar);
                    }

                    await Application.Current.SavePropertiesAsync();

                    waitActivityIndicator.IsRunning = false;

                    if (user.IsAdmin)
                        Application.Current.MainPage = new MainPage(user);
                    else
                        Application.Current.MainPage = new NavigationPage(new LongRunningPage(user)) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White };

                }
            }
            catch (Exception)
            {
                await DisplayAlert("Erro", "Ocorreu um erro. Tente de novo, por favor.", "Conexão falhou");
                waitActivityIndicator.IsRunning = false;
                return;
            }


        }

        private void OnToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value == false)
            {
                Application.Current.Properties.Remove("login");
                Application.Current.Properties.Remove("senha");
            }
            else
            {
                lembrarmeSwitch.OnColor = Color.Gray;
                lembrarmeSwitch.ThumbColor = Color.Green;
            }
        }

        private async void Handle_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrincipalPage());
        }
    }
}