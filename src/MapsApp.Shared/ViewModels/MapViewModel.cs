// <copyright file="MapViewModel.cs" company="Esri">
//      Copyright (c) 2017 Esri. All rights reserved.
//
//      Licensed under the Apache License, Version 2.0 (the "License");
//      you may not use this file except in compliance with the License.
//      You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//      Unless required by applicable law or agreed to in writing, software
//      distributed under the License is distributed on an "AS IS" BASIS,
//      WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//      See the License for the specific language governing permissions and
//      limitations under the License.
// </copyright>

namespace MapsApp.Shared.ViewModels
{
    using Esri.ArcGISRuntime.Location;
    using Esri.ArcGISRuntime.Mapping;
    using MapsApp.Shared.Commands;
    using System.Windows.Input;

    /// <summary>
    /// View Model handling logic for the Map
    /// </summary>
    class MapViewModel : BaseViewModel
    {
        private Map _map = new Map(Basemap.CreateTopographicVector());
        private Viewpoint _areaOfInterest;
        private LocationDataSource _locationDataSource;
        private Location _lastLocation;
        private ICommand _moveToCurrentLocationCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel"/> class.
        /// </summary>
        public MapViewModel()
        {
            this.LocationDataSource = new SystemLocationDataSource();
            LocationDataSource.LocationChanged += (s, l) =>
            {
                this._lastLocation = l;
            };
        }

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get
            {
                return this._map;
            }

            set
            {
                this._map = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current location data source
        /// </summary>
        public LocationDataSource LocationDataSource
        {
            get { return this._locationDataSource; }
            set
            {
                if (this._locationDataSource != value)
                {
                    this._locationDataSource = value;
                    this.OnPropertyChanged();
                }
            }
        }



        /// <summary>
        /// Gets or sets the current area of interest
        /// </summary>
        public Viewpoint AreaOfInterest
        {
            get { return this._areaOfInterest; }
            set
            {
                if (this._areaOfInterest != value)
                {
                    this._areaOfInterest = value;
                    this.OnPropertyChanged();
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
                return this._moveToCurrentLocationCommand ?? (this._moveToCurrentLocationCommand = new DelegateCommand(
                    (x) =>
                    {
                        // Set viewpoint to the user's current location
                        if (this._lastLocation != null)
                        {
                            this.AreaOfInterest = this._lastLocation.Position.Extent != null ? new Viewpoint(this._lastLocation.Position.Extent) :
                                new Viewpoint(this._lastLocation.Position, 4000);
                        }
                    }));
            }
        }
    }
}
