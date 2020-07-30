using Newtonsoft.Json;
using RadarFamilyCore.Service;
using RadarFamilyCore.ViewModels.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RadarFamilyCore.ViewModels
{
    public class TrackingPageViewModel : BaseViewModel
    {
        public ObservableCollection<DtoPosition> Items { get; set; }

        public TrackingPageViewModel(Int32 IdAdmin)
        {
            getListLastPosition(IdAdmin);
        }

        private async void getListLastPosition(Int32 IdAdmin)
        {
            Items = new ObservableCollection<DtoPosition>();
            Items.Clear();

            try
            {
                using (var client = new HttpClient())
                {

                    string uri = "http://207.180.246.227:8095/admin/Position/GetLastPosition?paramIdAdmin=" + IdAdmin;

                    HttpResponseMessage retorno = await client.GetAsync(uri);

                    var resultString = await retorno.Content.ReadAsStringAsync();

                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await Application.Current.MainPage.DisplayAlert("Erro", "Ocorreu um erro ao retonar as posições", "Ok");
                        return;
                    }
                    if (resultString != "[]")
                    {
                        foreach (DtoPosition item in JsonConvert.DeserializeObject<List<DtoPosition>>(resultString))
                        {
                            DtoPosition position = new DtoPosition()
                            {

                                Name = item.Name,
                                DateEvent = item.DateEvent,
                                DateAtualizacao = item.DateAtualizacao,
                                Avatar = item.Avatar,
                                Address = item.Address,
                                LatLong = item.LatLong,
                                Latitude = item.Latitude,
                                Longitude = item.Longitude
                            };
                            Items.Add(position);
                        }

                    }
                }
            
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
