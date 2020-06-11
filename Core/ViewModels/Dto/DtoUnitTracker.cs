using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarFamilyCore.ViewModels.Dto
{
    public class DtoUnitTracker
    {
        public Int32 IdUser { get; set; }
        public String Name { get; set; }
        public String Login { get; set; }
        public Int32 CalculoDistancia { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 IntervaloPosicao { get; set; }
        public String Avatar { get; set; }
        public Int32 IntervaloPosicaoParado { get; set; }
        public Int32 IdAdmin { get; set; }
    }
}
