using MapsApp.Shared.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasemapPage : ContentPage
    { 
        public BasemapPage(BasemapsViewModel basemapViewModel, MapViewModel mapViewModel)
        {
            this.BindingContext = basemapViewModel;
            basemapViewModel.MapViewModel = mapViewModel;
            InitializeComponent();
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}