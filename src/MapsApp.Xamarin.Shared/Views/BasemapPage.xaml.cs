using MapsApp.Shared.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasemapPage : ContentPage
    { 
        public BasemapPage(BasemapsViewModel basemapViewModel)
        {
            this.BindingContext = basemapViewModel;
            InitializeComponent();
        }
    }
}