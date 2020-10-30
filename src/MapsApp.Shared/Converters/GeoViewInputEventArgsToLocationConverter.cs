// /*******************************************************************************
//  * Copyright 2017 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  https://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

using System;
using System.Globalization;
#if __ANDROID__ || __IOS__ || NETFX_CORE
using IValueConverter = Xamarin.Forms.IValueConverter;
using Esri.ArcGISRuntime.Xamarin.Forms;
#else
using Esri.ArcGISRuntime.UI.Controls;
using System.Windows.Data;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Converters
{/// <summary>
 /// Converts event args from event to MapPoint
 /// </summary>
    class GeoViewInputEventArgsToLocationConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // get the Location from the event args
            return (value is GeoViewInputEventArgs) ? ((GeoViewInputEventArgs)value).Location : null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
