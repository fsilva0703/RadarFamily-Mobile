using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using RadarFamilyCore.Models;
using RadarFamilyCore.View;
using RadarFamilyCore.ViewModels.Dto;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace RadarFamilyCore.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<DtoUnitTracker> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Unidades Rastreadas";
            Items = new ObservableCollection<DtoUnitTracker>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, DtoUnitTracker>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as DtoUnitTracker;
                Items.Add(newItem);
                //await DataStore.AddItemAsync(newItem);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await ListaUnidadesRastreadasByAdmin();
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

        async Task<List<DtoUnitTracker>> ListaUnidadesRastreadasByAdmin()
        {
            List<DtoUnitTracker> itens = new List<DtoUnitTracker>();
            Int32 IdAdmin = (int)Application.Current.Properties["IdUser"];

            try
            {
                using (var client = new HttpClient())
                {

                    string uri = "http://radarfamily.somee.com/RadarFamily/admin/UnitTracker/GetUnitTrackerByAdmin?paramIdAdmin=" + IdAdmin;

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

                        itens = JsonConvert.DeserializeObject<List<DtoUnitTracker>>(resultString);
                        int countPos = JsonConvert.DeserializeObject<List<DtoUnitTracker>>(resultString).Count();

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