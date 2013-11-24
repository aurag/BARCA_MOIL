﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Barcelone___OGTS.Common
{
    [ValueConversion(typeof(String), typeof(Brush))]
    public class LeaveTypeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("Value : " + value.ToString());
            if (value.ToString().StartsWith("RE"))
                return Brushes.DimGray;
            if (value.ToString().StartsWith("RF") || value.ToString().StartsWith("CP"))
                return Brushes.Tomato;
            if (value.ToString().StartsWith("FE"))
                return Brushes.LightGray;
            if (value.ToString().StartsWith("X"))
                return Brushes.White;

            return Brushes.PaleGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}