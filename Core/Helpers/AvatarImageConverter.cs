using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RadarFamilyCore.Helpers
{
    public class AvatarImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sRet = null;

            if (value == null || string.IsNullOrEmpty(System.Convert.ToString(value)))
                return sRet;
            else
                return string.Format("http://207.180.246.227:8095/img/{0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
