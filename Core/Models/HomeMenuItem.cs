using System;
using System.Collections.Generic;
using System.Text;

namespace RadarFamilyCore.Models
{
    public enum MenuItemType
    {
        Pessoa,
        UltimaPosicao,
        HistoricoPosicao,
        ConfigRegras,
        ViolacaoRegra
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; }
    }
}
