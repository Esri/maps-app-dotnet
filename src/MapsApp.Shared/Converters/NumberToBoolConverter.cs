using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MapsApp.Shared.Converters
{
    class NumberToBoolConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (targetType == typeof(bool))
            {
                var number = System.Convert.ToDouble(value, culture);

                // Do not show the chevron buttons if only one value is present, but do show the items control
                return (number == 0) ? false : true;
            }

            throw new NotSupportedException("Converter can only convert to value of type Visibility.");
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
