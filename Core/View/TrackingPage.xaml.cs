using Newtonsoft.Json;
using RadarFamilyCore.Models;
using RadarFamilyCore.Service;
using RadarFamilyCore.ViewModels;
using RadarFamilyCore.ViewModels.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Color = Xamarin.Forms.Color;
using Distance = Xamarin.Forms.GoogleMaps.Distance;
using MapSpan = Xamarin.Forms.GoogleMaps.MapSpan;
using Pin = Xamarin.Forms.GoogleMaps.Pin;
using PinType = Xamarin.Forms.GoogleMaps.PinType;
using Polyline = Xamarin.Forms.GoogleMaps.Polyline;
using Position = Xamarin.Forms.GoogleMaps.Position;

namespace RadarFamilyCore.View
{
    public partial class TrackingPage : ContentPage
    {
        public TrackingPage(MenuItemType ItemTracking)
        {
            InitializeComponent();

            MyMap.MyLocationEnabled = true;
            MyMap.UiSettings.MyLocationButtonEnabled = true;
            
            MyMap.UiSettings.ZoomControlsEnabled = true;

            if ((int)ItemTracking == 1)
            {
                this.MontaUltimaPosicao();
                this.Title = "Últimas Posições";
            }
            else
            {
                this.MontaHistoricoPosicao();
                this.Title = "Histórico de Posições";
            }

        }

        private async void MontaUltimaPosicao()
        {
            Int32 IdAdmin = (int)Application.Current.Properties["IdUser"];

            List<DtoPosition> itens = new List<DtoPosition>();
            try
            {
                using (var client = new HttpClient())
                {

                    string uri = "http://207.180.246.227:8095/admin/Position/GetLastPosition?paramIdAdmin=" + IdAdmin;

                    HttpResponseMessage retorno = await client.GetAsync(uri);

                    var resultString = await retorno.Content.ReadAsStringAsync();

                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await DisplayAlert("Erro", "Ocorreu um erro ao retonar as posições", "Ok");
                        return;
                    }
                    if (resultString != "[]")
                    {
                        PositionService pos = new PositionService();

                        List<DtoPosition> listPosition = new List<DtoPosition>();

                        foreach (var item in JsonConvert.DeserializeObject<List<DtoPosition>>(resultString))
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
                            listPosition.Add(position);
                        }

                        lvPosition.ItemsSource = listPosition;

                        var polyline = new Polyline();

                        foreach (DtoPosition x in lvPosition.ItemsSource)
                        {

                            polyline.Positions.Add(new Position(x.Latitude, x.Longitude));

                            polyline.StrokeColor = Color.Red;
                            polyline.StrokeWidth = 5f;
                            polyline.Tag = "POLYLINE"; // Can set any object

                            polyline.IsClickable = false;
                            polyline.Clicked += (s, e) =>
                            {
                                // handle click polyline
                            };


                            Pin pin = new Pin()
                            {
                                Icon = BitmapDescriptorFactory.FromBundle(x.Avatar),
                                Type = PinType.Place,
                                Label = x.Name,
                                Address = x.Address,
                                Position = new Position(x.Latitude, x.Longitude),
                                ZIndex = 5,
                            };

                            MyMap.Pins.Add(pin);
                        }

                        MyMap.Polylines.Add(polyline);
                        Dictionary<Coordenates, Velocity> geo = new Dictionary<Coordenates, Velocity>();

                        Coordenates geoLoc = new Coordenates();

                        geo = await pos.GetCurrentPositionAsync();

                        foreach (KeyValuePair<Coordenates, Velocity> dadosGeo in geo)
                        {

                            geoLoc.lat = dadosGeo.Key.lat;
                            geoLoc.lng = dadosGeo.Key.lng;
                        }

                        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(geoLoc.lat, geoLoc.lng), Distance.FromKilometers(1)));

                        
                        //waitActivityIndicator.IsVisible = false;
                        //waitActivityIndicator.IsRunning = false;
                    }
                    else
                    {
                        //waitActivityIndicator.IsVisible = false;
                        //waitActivityIndicator.IsRunning = false;
                    }

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "Erro ao listar Matérias...");
                return;
            }
        }

        private async void MontaHistoricoPosicao()
        {
            List<DtoPosition> itens = new List<DtoPosition>();
            try
            {
                using (var client = new HttpClient())
                {
                    //var dadosRequest = new DtoFiltro()
                    //{
                    //    Assunto = filtro.Assunto,
                    //    ChkImpresso = filtro.ChkImpresso,
                    //    ChkTv = filtro.ChkTv,
                    //    ChkRd = filtro.ChkRd,
                    //    ChkOnline = filtro.ChkOnline,
                    //    ChkInter = filtro.ChkInter,
                    //    ChkMSocial = filtro.ChkMSocial,
                    //    Palavra = filtro.Palavra,
                    //    DataIni = new DateTime(Convert.ToDateTime(filtro.DataIni).Year, Convert.ToDateTime(filtro.DataIni).Month, Convert.ToDateTime(filtro.DataIni).Day),
                    //    DataFim = new DateTime(Convert.ToDateTime(filtro.DataFim).Year, Convert.ToDateTime(filtro.DataFim).Month, Convert.ToDateTime(filtro.DataFim).Day),
                    //    NomeBanco = filtro.NomeBanco,
                    //    Cliente = filtro.Cliente
                    //};

                    //var jsonRequest = JsonConvert.SerializeObject(dadosRequest);
                    //var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    waitActivityIndicator.IsVisible = true;
                    waitActivityIndicator.IsRunning = true;

                    string uri = "http://207.180.246.227:8095/admin/Position/GetHistoricPositionByData?paramDataIni=2020-04-01T08:00:00&paramDataFim=2020-12-31T12:00:00";

                    HttpResponseMessage retorno = await client.GetAsync(uri);

                    var resultString = await retorno.Content.ReadAsStringAsync();

                    if (retorno.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await DisplayAlert("Erro", "Ocorreu um erro ao retonar as posições", "Ok");
                        return;
                    }
                    if (resultString != "[]")
                    {
                        //noNews.IsVisible = false;
                        PositionService pos = new PositionService();
                        lvPosition.ItemsSource = JsonConvert.DeserializeObject<List<DtoPosition>>(resultString);
                        int countPos = JsonConvert.DeserializeObject<List<DtoPosition>>(resultString).Count();

                        Dictionary<Coordenates, Velocity> geo = new Dictionary<Coordenates, Velocity>();

                        Coordenates geoLoc = new Coordenates();

                        geo = await pos.GetCurrentPositionAsync();

                        foreach (KeyValuePair<Coordenates, Velocity> dadosGeo in geo)
                        {

                            geoLoc.lat = dadosGeo.Key.lat;
                            geoLoc.lng = dadosGeo.Key.lng;
                        }

                        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(geoLoc.lat, geoLoc.lng), Distance.FromMeters(5000)));
                    }
                    else
                    {
                        waitActivityIndicator.IsVisible = false;
                        waitActivityIndicator.IsRunning = false;
                        //noNews.IsVisible = true;
                    }

                    waitActivityIndicator.IsVisible = false;
                    waitActivityIndicator.IsRunning = false;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "Erro ao listar Matérias...");
                return;
            }
        }

        private void OnTappedTitulo(object sender, EventArgs e)
        {
            Label lblClicked = (Label)sender;
            var item = (TapGestureRecognizer)lblClicked.GestureRecognizers[0];
            var latlong = item.CommandParameter;

            string[] coordenadas = latlong.ToString().Split('|');
            double lat = Convert.ToDouble(coordenadas[0]);
            double lng = Convert.ToDouble(coordenadas[1]);

                Pin pin = new Pin
                {
                    Position = new Position(lat, lng),
                    Label = coordenadas[2],
                    Type = PinType.Place//,
                    //Icon = BitmapDescriptorFactory.FromBundle(coordenadas[3])
                };
                MyMap.Pins.Add(pin);

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(lat, lng), Distance.FromMeters(5000)));

            //await Browser.OpenAsync(url.ToString(), BrowserLaunchMode.SystemPreferred);
        }

    }
}