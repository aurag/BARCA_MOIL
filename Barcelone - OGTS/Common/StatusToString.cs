using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Barcelone___OGTS.Common
{
    [ValueConversion(typeof(String), typeof(String))]
    public class StatusToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString().Equals("1"))
                return "Demande créée";
            if (value.ToString().Equals("2"))
                return "Demande en attente de validation";
            if (value.ToString().Equals("5"))
                return "Demande acceptée";
            if (value.ToString().Equals("6"))
                return "Demande refusée";

            return "Status inconnu";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
