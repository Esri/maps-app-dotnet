
namespace MapsApp.Extensions
{
    using System;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        /// <summary>
        /// Get or sets the source of the image
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Converts images using resource ID to be loaded in XAML
        /// </summary>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return Source != null ? ImageSource.FromResource(Source): null;
        }
    }
}
