using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RadarFamilyCore.Models.Utils
{
    public class ClearData
    {
        public void ClearDataUser()
        {
            Application.Current.Properties.Remove("IdUnidadeRastreada");
            Application.Current.Properties.Remove("Name");
            Application.Current.Properties.Remove("IdUser");
            Application.Current.Properties.Remove("IdAdmin");
            Application.Current.Properties.Remove("IsAdmin");
        }
    }
}
