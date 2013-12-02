using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Barcelone___OGTS.Common
{
    [ValueConversion(typeof(string), typeof(Brush))]
    public class LeaveTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString().StartsWith("RE"))
                return Brushes.DimGray;
            if (value.ToString().StartsWith("RF") || value.ToString().StartsWith("CP"))
                return Brushes.Tomato;
            if (value.ToString().StartsWith("FE"))
                return Brushes.LightGray;
            if (value.ToString().StartsWith("PV"))
                return Brushes.LightPink;
            if (value.ToString().StartsWith("X"))
                return Brushes.White;
            if (value.ToString().StartsWith("Pas encore"))
                return Brushes.Red;
            if (value.ToString().StartsWith("En exploit"))
                return Brushes.Green;

            return Brushes.PaleGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
