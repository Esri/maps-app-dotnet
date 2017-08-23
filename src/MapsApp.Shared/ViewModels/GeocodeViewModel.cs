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
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Esri.ArcGISRuntime.Mapping;
    using Esri.ArcGISRuntime.Tasks.Geocoding;
    using MapsApp.Shared.Commands;
    using System.Diagnostics;

    /// <summary>
    /// View Model handling logic for the Geocoder
    /// </summary>
    class GeocodeViewModel : BaseViewModel
    {
        private const string GeocodeServiceUrl = @"https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";
        private string _searchText;
        private string _selectedSuggestion;
        private string _errorMessage;
        private Viewpoint _areaOfInterest;
        private GeocodeResult _place;
        private bool _isTopBannerVisible;
        private ICommand _searchCommand;
        private ICommand _cancelLocationSearchCommand;
        private ICommand _reverseGeocodeCommand;
        private ObservableCollection<string> _suggestionsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodeViewModel"/> class.
        /// </summary>
        public GeocodeViewModel()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.GetInfoFromGeocoderAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Gets the geocoder for the map
        /// </summary>yea
        public LocatorTask Locator { get; private set; }

        /// <summary>
        /// Gets or sets the search text the user has entered
        /// </summary>
        public string SearchText
        {
            get
            {
                return this._searchText;
            }

            set
            {
                if (this._searchText != value)
                {
                    this._searchText = value;
                    this.OnPropertyChanged();

                    if (!string.IsNullOrEmpty(this._searchText))
                    {
                        // Call method to get location suggestions
                        // Disable unawaited async warning
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        this.GetLocationSuggestionsAsync(this._searchText);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the suggestion the user has selected
        /// </summary>
        public string SelectedSearchItem
        {
            get
            {
                return this._selectedSuggestion;
            }

            set
            {
                if (this._selectedSuggestion != value && value != null)
                {
                    this._selectedSuggestion = value;

                    // Call method to search location
                    // Disable unawaited async warning
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    GetSearchedLocationAsync(this._selectedSuggestion);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                }
            }
        }

        /// <summary>
        /// Gets or sets the list of suggestions to be shown to the user
        /// </summary>
        public ObservableCollection<string> SuggestionsList
        {
            get
            {
                return this._suggestionsList;
            }

            set
            {
                if (value != null)
                {
                    this._suggestionsList = value;
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
        /// Gets or sets the result of the place selected by the user
        /// </summary>
        public GeocodeResult Place
        {
            get { return this._place; }
            set
            {
                if (this._place != value)
                {
                    this._place = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the top banner
        /// </summary>
        public bool IsTopBannerVisible
        {
            get
            {
                return this._isTopBannerVisible;
            }
            set
            {
                this._isTopBannerVisible = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the error message to be shown to the user
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (this._errorMessage != value && value != null)
                {
                    this._errorMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command to search using the locator
        /// </summary>
        public ICommand SearchCommand
        {
            get
            {
                return this._searchCommand ?? (this._searchCommand = new DelegateCommand(
                    (x) =>
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        this.GetSearchedLocationAsync((string)x);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }));
            }
        }

        /// <summary>
        /// Gets the command to cancel the location search and clear the pin off the map
        /// </summary>
        public ICommand CancelLocationSearchCommand
        {
            get
            {
                return this._cancelLocationSearchCommand ?? (this._cancelLocationSearchCommand = new DelegateCommand(
                    (x) =>
                    {
                        this.SearchText = string.Empty;
                        this.IsTopBannerVisible = false;
                    }));
            }
        }

        public ICommand ReverseGeocodeCommand
        {
            get
            {
                return this._reverseGeocodeCommand ?? (this._reverseGeocodeCommand = new DelegateCommand(
                    (x) =>
                    {
                        Debug.WriteLine(x?.ToString());
                    }));
            }
        }

        /// <summary>
        /// Gets the locator info
        /// </summary>
        internal LocatorInfo LocatorInfo { get; private set; }

        /// <summary>
        /// Loads the geocoder and gets locator info
        /// </summary>
        /// <returns>Async task</returns>
        private async Task GetInfoFromGeocoderAsync()
        {
            try
            {
                // Load locator and get locator info
                this.Locator = await LocatorTask.CreateAsync(new Uri(GeocodeServiceUrl));
                this.LocatorInfo = this.Locator?.LocatorInfo;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.ToString();
            }
        }



        /// <summary>
        /// Gets list of suggested locations from the locator based on user input
        /// </summary>
        /// <param name="userInput">User input</param>
        /// <returns>List of suggestions</returns>
        private async Task GetLocationSuggestionsAsync(string userInput)
        {
            if (userInput.Length > 0 && this.LocatorInfo != null)
            {
                try
                {
                    if (this.LocatorInfo.SupportsSuggestions)
                    {
                        // restrict the search to return no more than 10 suggestions
                        var suggestParams = new SuggestParameters { MaxResults = 10, PreferredSearchLocation = AreaOfInterest?.TargetGeometry as Esri.ArcGISRuntime.Geometry.MapPoint, };

                        // get suggestions for the text provided by the user
                        var suggestions = await this.Locator.SuggestAsync(userInput, suggestParams);
                        var s = new ObservableCollection<string>();
                        foreach (var suggestion in suggestions)
                        {
                            s.Add(suggestion.Label);
                        }
                        this.SuggestionsList = s;
                    }
                }
                catch
                {
                    // If error happens, do not show suggestions
                }
            }
        }

        /// <summary>
        /// Get location searched by user from the locator
        /// </summary>
        /// <param name="_searchString">User input</param>
        /// <returns>Location that best matches the search string</returns>
        private async Task GetSearchedLocationAsync(string geocodeAddress)
        {
            this.SuggestionsList.Clear();
            try
            {
                var geocodeParameters = new GeocodeParameters
                {
                    MaxResults = 1,
                    PreferredSearchLocation = AreaOfInterest?.TargetGeometry as Esri.ArcGISRuntime.Geometry.MapPoint,
                };
                var matches = await this.Locator?.GeocodeAsync(geocodeAddress, geocodeParameters);
                this.Place = matches.First();

                // Select located feature on map
                if (this.Place != null)
                {
                    this.IsTopBannerVisible = true;

                    // Set viewpoint to the feature's extent
                    this.AreaOfInterest = this.Place.Extent != null ? new Viewpoint(this.Place.Extent) :
                        new Viewpoint(this.Place.DisplayLocation, 4000);
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.ToString();
            }
        }
    }
}
