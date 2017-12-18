using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorDialog : ContentView
	{
        private string _errorMessage;
		public ErrorDialog ()
		{
			InitializeComponent ();
		}

        /// <summary>
        /// Gets or sets the error message to be shown to the user
        /// </summary>
        //BindableProperty ErrorMessageProperty = new BindableProperty("ErrorMessage", typeof(string), typeof(ErrorDialog),)

        //ErrorMessage_ValueChanged()
        //{
        //    ErrorMessage.Text = newValue;
        //}
    }
}