using System;
using System.Dynamic;

namespace RadarFamilyCore.ViewModels.Dto
{
    public class DtoPosition
    {
        private String _latLong;

        public Int64? _id { get; set; }
        public Int32 IdUnidadeRastreada { get; set; }
        public String Name { get; set; }
        public DateTime DateEvent { get; set; }
        public DateTime? DateAtualizacao { get; set; }
        public String Address { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Int32? IdRegra { get; set; }
        public String Avatar { get; set; }
        public String LatLong 
        { 
            get { return Latitude.ToString() + "|" + Longitude.ToString() + "|" + Name.ToString(); }
            set
            {
                _latLong = value;
            }
        }
        public double LevelBattery { get; set; }
        public double Velocity { get; set; }
        
    }
}
