// /*******************************************************************************
//  * Copyright 2017 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/
using System;
using System.Globalization;
using System.Collections.ObjectModel;
#if __ANDROID__ || __IOS__ || NETFX_CORE
using IValueConverter = Xamarin.Forms.IValueConverter;
#else
using System.Windows.Data;
using System.Windows;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Converters
{
    /// <summary>
    /// Converts number to control visibility
    /// </summary>
    class NumberToVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double number;
            if (value == null)
                value = 0;

            if (value is ObservableCollection<string>)
            {
                number = ((ObservableCollection<string>)value).Count;
            }
            else
            {
                number = System.Convert.ToDouble(value, culture);
            }

#if __ANDROID__ || __IOS__ || NETFX_CORE
                return (number == 0) ? false : true;
#else
                return (number == 0) ? Visibility.Collapsed : Visibility.Visible;
#endif
            
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
