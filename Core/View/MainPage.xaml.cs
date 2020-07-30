using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RadarFamilyCore.Models;
using RadarFamilyCore.ViewModels.Dto;

namespace RadarFamilyCore.View
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    //[DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage(DtoResultLogin usuarioLogado)
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            //MenuPages.Add((int)MenuItemType.Pessoa, (NavigationPage)Detail);
            detailPage.BarBackgroundColor = Color.MediumPurple;
            detailPage.BarTextColor = Color.White;
            detailPage.Title = "Radar Family";

            IsPresented = true;

            //MenuPages.Add((int)MenuItemType.Pessoa, new NavigationPage(new TrackerUnitPage(MenuItemType.Pessoa)) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White, Title = "Unidades Rastreadas" });

       
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Pessoa:
                        MenuPages.Add(id, new NavigationPage(new TrackerUnitPage(MenuItemType.Pessoa)) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White, Title = "Unidades Rastreadas" });
                        break;
                    case (int)MenuItemType.UltimaPosicao:
                        MenuPages.Add(id, new NavigationPage(new TrackingPage(MenuItemType.UltimaPosicao)) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White, Title = "Últimas Posições" });
                        break;
                    case (int)MenuItemType.HistoricoPosicao:
                        MenuPages.Add(id, new NavigationPage(new TrackingPage(MenuItemType.HistoricoPosicao)) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White, Title = "Histórico de Posições" });
                        break;
                    case (int)MenuItemType.ConfigRegras:
                        MenuPages.Add(id, new NavigationPage(new TrackerUnitPage(MenuItemType.Pessoa)) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White, Title = "Unidades Rastreadas" });
                        break;
                    case (int)MenuItemType.ViolacaoRegra:
                        MenuPages.Add(id, new NavigationPage(new TrackerUnitPage(MenuItemType.Pessoa)) { BarBackgroundColor = Color.MediumPurple, BarTextColor = Color.White, Title = "Unidades Rastreadas" });
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}