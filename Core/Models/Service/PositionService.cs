using Newtonsoft.Json;
using RadarFamilyCore.Models.Service;
using RadarFamilyCore.Models.Utils;
using RadarFamilyCore.Service.Interface;
using RadarFamilyCore.ViewModels.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace RadarFamilyCore.Service
{
    public class PositionService
    {       

        public PositionService()
        {
        }
        public async Task<Coordenates> GetCurrentPositionAsync()
        {
            Coordenates geo = new Coordenates();

            var loc = await Geolocation.GetLastKnownLocationAsync();
            if(loc == null)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                loc = await Geolocation.GetLocationAsync(request);
            }

            geo.lat = loc.Latitude;
            geo.lng = loc.Longitude;

            return geo;
            
        }

        public async Task<Pin> GetAddressToPinMapByReverseGeoCode(Double lat, Double lng)
        {
            Geocoder geoCoder = new Geocoder();

            Position position = new Position(lat, lng);
            IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
            string address = possibleAddresses.FirstOrDefault();

            Pin pin = new Pin
            {
                Position = new Position(lat, lng),
                Label = "Felipe Gabriel",
                Address = address,
                Type = PinType.Place
            };

            return pin;
        }

        public async Task<String> GetAddressByReverseGeoCode(Double lat, Double lng)
        {
            Geocoder geoCoder = new Geocoder();

            Position position = new Position(lat, lng);
            IEnumerable<string> possibleAddresses = await geoCoder.GetAddressesForPositionAsync(position);
            string address = possibleAddresses.FirstOrDefault();

            return address;
        }

        public async Task<List<DtoPosition>> GetLastPosition(Int32 paramIdeUnidadeRastreada)
        {
            List<DtoPosition> listPosition = new List<DtoPosition>(); 

            using (var client = new HttpClient())
            {

                string uri = "http://radarfamily.somee.com/RadarFamily/admin/Position/GetLastPosition?paramIdUnidadeRastreada=" + paramIdeUnidadeRastreada;

                var retorno = await client.GetAsync(uri);

                var resultString = await retorno.Content.ReadAsStringAsync();

                listPosition = JsonConvert.DeserializeObject<List<DtoPosition>>(resultString);

                if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    //throw new Exception("Ocorrer um erro ao enviar posição!");
                }
            }

            return listPosition;
        }

        public async Task<Double> GetCalculateDistance(Int32 paramIdUnidadeRastreada, Coordenates latLongLastPos, Coordenates latLongCurrentePos)
        {
            
            Location lastPosition = new Location(latLongLastPos.lat, latLongLastPos.lng);
            Location currentPosition = new Location(latLongCurrentePos.lat, latLongCurrentePos.lng);
            Double metros = Location.CalculateDistance(lastPosition, currentPosition, DistanceUnits.Kilometers);
            
            return metros * 1000;
        }

        public async void SetPosition()
        {
            if (Application.Current.Properties.ContainsKey("IdUnidadeRastreada"))
            {

                Int32 IdUnidadeRastreada = (Int32)Application.Current.Properties["IdUnidadeRastreada"];
                Int32 Tolerancia = (Int32)Application.Current.Properties["CalculoDistancia"];

                try
                
                {

                    if (Application.Current.Properties["IdUnidadeRastreada"] != null && (Boolean)Application.Current.Properties["IsAdmin"] == false)
                    {
                        //Posição atual
                        Coordenates loc = await GetCurrentPositionAsync();

                        if (loc != null)
                        {
                            Double lat = 0;
                            Double lng = 0;

                            lat = loc.lat;
                            lng = loc.lng;

                            //Reverser GeoCode para endereço
                            String address = await GetAddressByReverseGeoCode(lat, lng);

                            //Posição anterior
                            DtoPosition lastPos = new DtoPosition();
                            lastPos = GetLastPosition(IdUnidadeRastreada).Result.FirstOrDefault();

                            Boolean gravaPosicao = true;

                            //Cálculo de distância
                            
                            if (lastPos != null)
                            {
                                Coordenates lastLatLong = new Coordenates();
                                lastLatLong.lat = lastPos.Latitude;
                                lastLatLong.lng = lastPos.Longitude;

                                Double distance = await GetCalculateDistance(IdUnidadeRastreada, lastLatLong, loc);

                                if (distance > Tolerancia || lastPos.DateEvent.Day != DateTime.Today.Day)
                                    gravaPosicao = true;
                                else
                                    gravaPosicao = false;

                            }
                            if (gravaPosicao)
                            {

                                using (var client = new HttpClient())
                                {

                                    var positionRequest = new DtoPosition
                                    {
                                        _id = null,
                                        IdUnidadeRastreada = (Int32)Application.Current.Properties["IdUnidadeRastreada"],
                                        Name = Application.Current.Properties["Name"].ToString(),
                                        DateEvent = DateTime.Now,
                                        Address = address,
                                        Latitude = lat,
                                        Longitude = lng,
                                        IdRegra = null,
                                        Avatar = Application.Current.Properties["Avatar"].ToString()
                                    };

                                    var jsonRequest = JsonConvert.SerializeObject(positionRequest);
                                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                                    string uri = "http://radarfamily.somee.com/RadarFamily/admin/Position/SetLastPosition";

                                    var retorno = await client.PostAsync(uri, httpContent);

                                    var resultString = await retorno.Content.ReadAsStringAsync();

                                    DtoPosition pos = new DtoPosition();
                                    pos = JsonConvert.DeserializeObject<DtoPosition>(resultString);

                                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                                    {
                                        //throw new Exception("Ocorrer um erro ao enviar posição!");
                                    }

                                    #region ViolacaoRegra

                                    RuleService rule = new RuleService();
                                    Boolean HasRule = await rule.HasViolationRule(pos.IdUnidadeRastreada, pos.Latitude, pos.Longitude, pos._id);

                                    #endregion


                                    //DtoPosition position = new DtoPosition();
                                    //position = JsonConvert.DeserializeObject<DtoPosition>(resultString);

                                }
                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu um erro ao enviar posição. Erro: " + ex.Message);
                }

                await Application.Current.SavePropertiesAsync();
            }
        }

    }
}
