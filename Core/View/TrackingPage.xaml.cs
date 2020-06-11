using Newtonsoft.Json;
using RadarFamilyCore.Models;
using RadarFamilyCore.Service;
using RadarFamilyCore.ViewModels.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;
using Pin = Xamarin.Forms.GoogleMaps.Pin;
using PinType = Xamarin.Forms.GoogleMaps.PinType;
using Position = Xamarin.Forms.GoogleMaps.Position;
using MapSpan = Xamarin.Forms.GoogleMaps.MapSpan;
using Distance = Xamarin.Forms.GoogleMaps.Distance;
using Android.Graphics;
using Android.Widget;
using System.IO;
using System.Reflection;
using FFImageLoading;
using Polyline = Xamarin.Forms.GoogleMaps.Polyline;
using Color = Xamarin.Forms.Color;

namespace RadarFamilyCore.View
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    //[DesignTimeVisible(false)]
    public partial class TrackingPage : ContentPage
    {
        public TrackingPage(MenuItemType ItemTracking)
        {
            InitializeComponent();

            MyMap.MyLocationEnabled = true;
            MyMap.UiSettings.MyLocationButtonEnabled = true;

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
            List<DtoPosition> itens = new List<DtoPosition>();
            try
            {
                using (var client = new HttpClient())
                {

                    string uri = "http://radarfamily.somee.com/RadarFamily/admin/Position/GetLastPosition?paramIdUnidadeRastreada=";

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
                        lvPosition.ItemsSource = JsonConvert.DeserializeObject<List<DtoPosition>>(resultString);
                        int countPos = JsonConvert.DeserializeObject<List<DtoPosition>>(resultString).Count();

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
                                ZIndex = 5
                            };

                            MyMap.Pins.Add(pin);
                        }

                        MyMap.Polylines.Add(polyline);

                        Coordenates geo = await pos.GetCurrentPositionAsync();

                        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(geo.lat, geo.lng), Distance.FromMeters(5000)));

                        
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

                    string uri = "http://www.radarfamily.somee.com/RadarFamily/admin/Position/GetHistoricPositionByData?paramDataIni=2020-04-01T08:00:00&paramDataFim=2020-12-31T12:00:00";

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

                        Coordenates geo = await pos.GetCurrentPositionAsync();

                        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(geo.lat, geo.lng), Distance.FromMeters(5000)));
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

        private async void OnTappedTitulo(object sender, EventArgs e)
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