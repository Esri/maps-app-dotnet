using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MapsApp.Shared.ViewModels
{
    public class BasemapsViewModel : BaseViewModel
    {
        private IEnumerable<PortalItem> _basemaps;
        private PortalItem _selectedBasemap;

        public MapViewModel MapViewModel { get; set; }

        public BasemapsViewModel()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadPortal();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public PortalInfo PortalInfo { get; set; }

        /// <summary>
        /// Property holding the list of basemaps to be added to the UI
        /// </summary>
        public IEnumerable<PortalItem> Basemaps
        {
            get { return _basemaps; }
            set
            {
                if (_basemaps != value && value != null)
                {
                    _basemaps = value;
                    OnPropertyChanged();
                }
            }
        }

        public PortalItem SelectedBasemap
        {
            get { return this._selectedBasemap; }
            set
            {
                if (_selectedBasemap != value && value != null)
                {
                    _selectedBasemap = value;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    LoadNewMap();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }

        private async Task LoadPortal()
        {
            try
            {
                var portal = await ArcGISPortal.CreateAsync();
                PortalInfo = portal.PortalInfo;
                await LoadMaps(portal);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to connect to Portal. " + ex.ToString());
            }           
        }

        private async Task LoadMaps(ArcGISPortal portal)
        {
            var items = await portal.GetBasemapsAsync();
            Basemaps = items.Select(b => b.Item).OfType<PortalItem>();        
        }

        private async Task LoadNewMap()
        {
            var newMap = new Map(SelectedBasemap);
            await newMap.LoadAsync();
            this.MapViewModel.Map = newMap;
        }
    }
}
