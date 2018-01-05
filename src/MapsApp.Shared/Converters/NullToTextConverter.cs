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
using System.Linq;
#if __ANDROID__ || __IOS__ || NETFX_CORE
using IValueConverter = Xamarin.Forms.IValueConverter;
#else
using System.Windows.Data;
using System.Windows;
#endif

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Converters
{
    /// <summary>
    /// Converts null to a text value. The value is being passed in the parameter of the function
    /// Parameter type is text1|text2 where text1 is applied if value is null
    /// </summary>
    class NullToTextConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Handle null to text
            if (parameter != null)
            {
                //if value is null, the first text value is displayed
                return (value == null) ? ((string)parameter).Split('|').ElementAtOrDefault(0) ?? "Invalid Parameter": ((string)parameter).Split('|').ElementAtOrDefault(1) ?? "Invalid Parameter";
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
