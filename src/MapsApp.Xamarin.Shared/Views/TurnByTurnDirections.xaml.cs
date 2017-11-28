using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TurnByTurnDirections : ContentPage
	{
		public TurnByTurnDirections ()
		{
			InitializeComponent ();
		}
	}
}