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
using Esri.ArcGISRuntime.ExampleApps.MapsApp.Helpers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels
{
    /// <summary>
    /// View Model handling logic for the Geocoder
    /// </summary>
    class GeocodeViewModel : BaseViewModel
    {
        private const int DefaultZoomScale = 4000;
        private string _searchText;
        private string _fromSearchText;
        private string _toSearchText;
        private string _selectedSuggestion;
        private string _selectedFromSuggestion;
        private string _selectedToSuggestion;
        private string _errorMessage;
        private Viewpoint _areaOfInterest;
        private GeocodeResult _place;
        private GeocodeResult _fromPlace;
        private GeocodeResult _toPlace;
        private ICommand _searchCommand;
        private ICommand _cancelLocationSearchCommand;
        private ICommand _reverseGeocodeCommand;
        private ICommand _setSelectedStartLocationCommand;
        private ObservableCollection<string> _suggestionsList;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodeViewModel"/> class.
        /// </summary>
        public GeocodeViewModel()
        {
            Initialize();
        }

        /// <summary>
        /// Intialize view model
        /// </summary>
        private async void Initialize()
        {
            try
            {
                // Load locator 
                Locator = await LocatorTask.CreateAsync(new Uri(Configuration.GeocodeUrl));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
            
        }

        /// <summary>
        /// Gets the geocoder for the map
        /// </summary>
        internal LocatorTask Locator { get; set; }

        /// <summary>
        /// Gets or sets the search text the user has entered
        /// </summary>
        public string SearchText
        {
            get
            {
                return _searchText;
            }

            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(_searchText))
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        // Call method to get location suggestions
                        // Disable unawaited async warning
                        GetLocationSuggestionsAsync(_searchText);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                    else
                    {
                        SuggestionsList = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the search text the user has entered
        /// </summary>
        public string FromSearchText
        {
            get
            {
                return _fromSearchText;
            }

            set
            {
                if (_fromSearchText != value)
                {
                    _fromSearchText = value;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(_fromSearchText))
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        // Call method to get location suggestions
                        // Disable unawaited async warning
                        GetLocationSuggestionsAsync(_fromSearchText);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                    else
                    {
                        SuggestionsList = null;
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the search text the user has entered
        /// </summary>
        public string ToSearchText
        {
            get
            {
                return _toSearchText;
            }

            set
            {
                if (_toSearchText != value)
                {
                    _toSearchText = value;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(_toSearchText))
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        // Call method to get location suggestions
                        // Disable unawaited async warning
                        GetLocationSuggestionsAsync(_toSearchText);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                    else
                    {
                        SuggestionsList = null;
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
               return _selectedSuggestion;
            }
            
            set
            {
                if (_selectedSuggestion != value && value != null)
                {
                    _selectedSuggestion = value;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    // Call method to search location
                    // Disable unawaited async warning
                    GetSearchedLocationAsync(_selectedSuggestion, nameof(Place));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    OnPropertyChanged();

                }
            }
        }

        /// <summary>
        /// Gets or sets the suggested from location that the user has selected
        /// </summary>
        public string SelectedFromSuggestion
        {
            get
            {
                return _selectedFromSuggestion;
            }

            set
            {
                if (_selectedFromSuggestion != value && value != null)
                {
                    _selectedFromSuggestion = value;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    // Call method to search location
                    // Disable unawaited async warning
                    GetSearchedLocationAsync(_selectedFromSuggestion, nameof(FromPlace));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    OnPropertyChanged();

                }
            }
        }

        /// <summary>
        /// Gets or sets the suggested to location that the user has selected
        /// </summary>
        public string SelectedToSuggestion
        {
            get
            {
                return _selectedToSuggestion;
            }

            set
            {
                if (_selectedToSuggestion != value && value != null)
                {
                    _selectedToSuggestion = value;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    // Call method to search location
                    // Disable unawaited async warning
                    GetSearchedLocationAsync(_selectedToSuggestion, nameof(ToPlace));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    OnPropertyChanged();

                }
            }
        }

        /// <summary>
        /// Gets or sets the result of the place selected by the user
        /// </summary>
        public GeocodeResult Place
        {
            get { return _place; }
            set
            {
                if (_place != value)
                {
                    _place = value;
                    ZoomToPlace();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the result of the place where the routing will start
        /// </summary>
        public GeocodeResult FromPlace
        {
            get { return _fromPlace; }
            set
            {
                if (_fromPlace != value)
                {
                    _fromPlace = value;
                    FromSearchText = _fromPlace.Label;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Gets or sets the result of the place where the routing will end
        /// </summary>
        public GeocodeResult ToPlace
        {
            get { return _toPlace; }
            set
            {
                if (_toPlace != value)
                {
                    _toPlace = value;
                    ToSearchText = _toPlace.Label;
                    OnPropertyChanged();
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
                return _suggestionsList;
            }

            set
            {
                _suggestionsList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current area of interest
        /// </summary>
        public Viewpoint AreaOfInterest
        {
            get
            {
                return _areaOfInterest;
            }

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
        /// Gets or sets the error message to be shown to the user
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value && value != null)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
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
                return _searchCommand ?? (_searchCommand = new DelegateCommand(
                    async (x) =>
                    {
                        await GetSearchedLocationAsync((string)x, nameof(Place));
                    }));
            }
        }

        // Gets the command to perform reverse geocoding using the locator
        public ICommand ReverseGeocodeCommand
        {
            get
            {
                return _reverseGeocodeCommand ?? (_reverseGeocodeCommand = new DelegateCommand(
                    async (x) =>
                    {
                        if (x != null)
                        {
                            await GetReverseGeocoderLocationAsync((MapPoint)x);
                        }
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
                return _cancelLocationSearchCommand ?? (_cancelLocationSearchCommand = new DelegateCommand(
                    (x) =>
                    {
                        SearchText = string.Empty;
                        Place = null;
                    }));
            }
        }

        /// <summary>
        /// Gets list of suggested locations from the locator based on user input
        /// </summary>
        /// <param name="userInput">User input</param>
        /// <returns>List of suggestions</returns>
        private async Task GetLocationSuggestionsAsync(string userInput)
        {
            if (Locator?.LocatorInfo?.SupportsSuggestions ?? false && !string.IsNullOrEmpty(userInput))
            {
                try
                {
                    // restrict the search to return no more than 10 suggestions
                    var suggestParams = new SuggestParameters { MaxResults = 10, PreferredSearchLocation = AreaOfInterest?.TargetGeometry as MapPoint, };

                    // get suggestions for the text provided by the user
                    var suggestions = await Locator.SuggestAsync(userInput, suggestParams);
                    var s = new ObservableCollection<string>();
                    foreach (var suggestion in suggestions)
                    {
                        s.Add(suggestion.Label);
                    }
                    SuggestionsList = s;
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
        public async Task GetSearchedLocationAsync(string geocodeAddress, string sender)
        {
            //SuggestionsList.Clear();
            SearchText = string.Empty;

            try
            {
                // Locate the searched feature
                if (Locator != null)
                {
                    var geocodeParameters = new GeocodeParameters
                    {
                        MaxResults = 1,
                        
                        PreferredSearchLocation = AreaOfInterest?.TargetGeometry as MapPoint,
                    };
                    var matches = await Locator.GeocodeAsync(geocodeAddress, geocodeParameters);

                    // set it into the appropriate property
                    this.GetType().GetProperty(sender).SetValue(this, matches.FirstOrDefault());
                }
                else
                {
                    ErrorMessage = "Unable to load geocoder";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
        }

        /// <summary>
        /// Use the locator to perform a reverse geocode operation, returning the place that the user tapped on inside the map
        /// </summary>
        private async Task GetReverseGeocoderLocationAsync(MapPoint location)
        {
            var matches = await Locator.ReverseGeocodeAsync(location);
            Place = matches.First();
        }

        /// <summary>
        /// Zoom to the location inside the Place property
        /// </summary>
        private void ZoomToPlace()
        {
            // Select located feature on map
            if (Place != null)
            {
                // Set viewpoint to the feature's extent
                AreaOfInterest = Place.Extent != null ? new Viewpoint(Place.Extent) :
                    new Viewpoint(Place.DisplayLocation, DefaultZoomScale);
            }
        }
    }
}
