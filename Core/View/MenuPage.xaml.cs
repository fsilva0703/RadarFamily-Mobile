using RadarFamilyCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RadarFamilyCore.View
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    //[DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            lblUser.Text = Application.Current.Properties["Name"].ToString();
            imgAvatar.Source = Application.Current.Properties["Avatar"].ToString();
            lblLogin.Text = "("+Application.Current.Properties["login"].ToString()+")";

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Pessoa, Title="Unidade Rastreada", Icon="icon_pessoa.png" },
                new HomeMenuItem {Id = MenuItemType.UltimaPosicao, Title="Última Posição", Icon="icon_gps.png" },
                new HomeMenuItem {Id = MenuItemType.HistoricoPosicao, Title="Histórico de Posição", Icon="icon_gpsHistoric.png" },
                new HomeMenuItem {Id = MenuItemType.ViolacaoRegra, Title="Violação de Regra", Icon="icon_rules.png" },
                new HomeMenuItem {Id = MenuItemType.ConfigRegras, Title="Config. de Regra", Icon="icon_config.png" },
                
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}