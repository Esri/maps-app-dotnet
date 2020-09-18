using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin.Helpers
{
    public class DirectionManeuverToFontIconImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            // Set up basic image source properties
            FontImageSource source = new FontImageSource();
            object fontFam = "";
            App.Current.Resources.TryGetValue("IconFont", out fontFam);
            source.FontFamily = (OnPlatform<string>)fontFam;

            source.Size = 18;
            source.SetDynamicResource(FontImageSource.ColorProperty, "AccentColor");

            //rewrite values to match icon font for turns
            string valueString = value.ToString();
            if (valueString.StartsWith("Turn"))
            {
                valueString = valueString.Substring(4);
            }

            // The icon font helpfully uses the same names as the direction manuever titles for navigation icons
            if(typeof(IconFont).GetFields().FirstOrDefault(field => field.Name == valueString) is FieldInfo fi)
            {
                source.Glyph = fi.GetValue(null).ToString();
            }

            if (source.Glyph != null)
            {
                return source;
            }
            // fall back to image path
            return $"{valueString}.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
