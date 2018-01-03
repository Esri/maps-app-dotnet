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
using Esri.ArcGISRuntime.ExampleApps.MapsApp.Commands;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels
{
    /// <summary>
    /// View Model handling logic for the Map
    /// </summary>
    public class MapViewModel : BaseViewModel
    {
        private const int DefaultZoomScale = 4000;
        private Map _map = new Map(Basemap.CreateTopographicVector());
        private Viewpoint _areaOfInterest;
        private LocationDataSource _locationDataSource;
        private Location.Location _lastLocation;
        private ICommand _moveToCurrentLocationCommand;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel"/> class.
        /// </summary>
        public MapViewModel()
        {
            LocationDataSource = new SystemLocationDataSource();
            LocationDataSource.LocationChanged += (s, l) =>
            {
                _lastLocation = l;
            };

            // reset map if user is logged out
            AuthViewModel.Instance.PropertyChanged += (s, l) =>
            {
                if (l.PropertyName == nameof(AuthViewModel.AuthenticatedUser) && AuthViewModel.Instance.AuthenticatedUser == null)
                {
                    // capture map's current extent
                    Map = new Map(Basemap.CreateTopographicVector())
                    {
                        InitialViewpoint = AreaOfInterest
                    };
                }
            };
        }

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get
            {
                return _map;
            }

            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current location data source
        /// </summary>
        public LocationDataSource LocationDataSource
        {
            get { return _locationDataSource; }
            set
            {
                if (_locationDataSource != value)
                {
                    _locationDataSource = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current area of interest
        /// </summary>
        public Viewpoint AreaOfInterest
        {
            get { return _areaOfInterest; }
            set
            {
                if (_areaOfInterest != value)
                {
                    _areaOfInterest = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command to search using the locator
        /// </summary>
        public ICommand MoveToCurrentLocationCommand
        {
            get
            {
                return _moveToCurrentLocationCommand ?? (_moveToCurrentLocationCommand = new DelegateCommand(
                    (x) =>
                    {
                        // Set viewpoint to the user's current location
                        AreaOfInterest = new Viewpoint(_lastLocation?.Position, DefaultZoomScale);
                    }));
            }
        }
    }
}
