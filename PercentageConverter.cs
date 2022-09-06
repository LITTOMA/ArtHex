using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtHex
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float v)
            {
                return $"{v*100:F2}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string v)
            {
                v = v.Replace("%", "").Trim();
                return (float.Parse(v)) / 100.0f;
            }
            return 0f;
        }
    }
}
