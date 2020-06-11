using System;
using System.Collections.Generic;
using System.Text;

namespace RadarFamilyCore.ViewModels.Dto
{
    public class DtoResultLogin
    {
        public String Login { get; set; }
        public String Password { get; set; }
        public String Nome { get; set; }
        public Int32 IdUser { get; set; }
        public Int32 IdAdmin { get; set; }
        public Int32 IdUnidadeRastreada { get; set; }
        public Boolean IsAdmin { get; set; }
        public Int32 CalculoDistancia { get; set; }
        public Int32 IntervaloPosicao { get; set; }
        public String Avatar { get; set; }
    }
}
