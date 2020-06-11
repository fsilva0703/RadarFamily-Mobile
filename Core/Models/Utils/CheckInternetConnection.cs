using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RadarFamilyCore.Models.Utils
{
    public class CheckInternetConnection
    {
        public Boolean HasInternetConnection()
        {
            var current = Connectivity.NetworkAccess;

            Boolean HasInternet = false;

            if (current == NetworkAccess.Internet ? HasInternet = true : false);

            return HasInternet;
        }
    }
}
