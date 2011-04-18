using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using EHaskins.Utilities.NumericExtensions;

namespace EHaskins.Utilities.Wpf
{
    public class MacAddressConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var data = value as byte[];
                var output = data.ToMacString();
                return output;
            }
            catch (Exception ex)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
