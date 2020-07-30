using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RadarFamilyCore.Helpers
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sRet = null;

            if (value == null || string.IsNullOrEmpty(System.Convert.ToString(value)))
                return sRet;
            else
                return String.Format("{0:dd/MM/yyyy HH:mm:ss}", value); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
