using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RadarFamilyCore.ViewModels.Dto
{
    public class AvatarViewModel : BaseViewModel
    {
        public ObservableCollection<DtoAvatar> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public AvatarViewModel()
        {
            Title = "Avatar";
            Items = new ObservableCollection<DtoAvatar>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await ListaAvatarByAdmin();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task<List<DtoAvatar>> ListaAvatarByAdmin()
        {
            List<DtoAvatar> itens = new List<DtoAvatar>();

            try
            {
                using (var client = new HttpClient())
                {

                    string uri = "http://207.180.246.227:8095/admin/UnitTracker/GetAvatar";

                    HttpResponseMessage retorno = await client.GetAsync(uri);

                    var resultString = await retorno.Content.ReadAsStringAsync();

                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        //await DisplayAlert("Erro", "Ocorreu um erro ao retonar as unidades rastreadas", "Ok");
                        return itens;
                    }
                    if (resultString != "[]")
                    {
                        //noNews.IsVisible = false;

                        itens = JsonConvert.DeserializeObject<List<DtoAvatar>>(resultString);
                        int countPos = JsonConvert.DeserializeObject<List<DtoAvatar>>(resultString).Count();

                    }
                    else
                    {
                        //waitActivityIndicator.IsVisible = false;
                        //waitActivityIndicator.IsRunning = false;
                        //noNews.IsVisible = true;
                    }

                }

                return itens;

            }
            catch (Exception ex)
            {
                //await DisplayAlert("Erro", ex.Message, "Erro ao listar Matérias...");
                return itens;
            }
        }

    }
}
