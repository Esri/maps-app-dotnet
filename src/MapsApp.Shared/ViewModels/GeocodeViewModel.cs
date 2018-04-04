// /*******************************************************************************
//  * Copyright 2018 Esri
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
        private string _selectedSuggestion;
        private MapPoint _reverseGeocodeInputLocation;
        private Viewpoint _areaOfInterest;
        private GeocodeResult _place;
        private GeocodeResult _fromPlace;
        private GeocodeResult _toPlace;
        private ICommand _searchCommand;
        private ICommand _cancelLocationSearchCommand;
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
                ErrorMessage = "Unable to load Geocoder. Searching may be affected.";
                StackTrace = ex.ToString();
            }
        }

        /// <summary>
        /// Gets the geocoder for the map
        /// </summary>
        private LocatorTask Locator { get; set; }

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
                        // Call method to get location suggestions
                        GetLocationSuggestionsAsync(_searchText).ContinueWith((t) =>
                        {
                            if (t.Status == TaskStatus.RanToCompletion)
                                SuggestionsList = t.Result;
                        });
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
                if ( value != null)
                {
                    _selectedSuggestion = value;

                    // Call method to search location
                    GetSearchedLocationAsync(_selectedSuggestion).ContinueWith((t) =>
                    {
                        if (t.Status == TaskStatus.RanToCompletion)
                            Place = t.Result;
                    });
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

                    // Set viewpoint to the feature's extent
                    AreaOfInterest = Place != null ? (Place.Extent != null ? new Viewpoint(Place.Extent) :
                        new Viewpoint(Place.DisplayLocation, DefaultZoomScale)) : AreaOfInterest;
                    SearchText = Place?.Label ?? string.Empty;
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
        /// Gets or sets the location to be reverse geocoded
        /// </summary>
        public MapPoint ReverseGeocodeInputLocation
        {
            get { return _reverseGeocodeInputLocation; }
            set
            {
                if (_reverseGeocodeInputLocation != value)
                {
                    _reverseGeocodeInputLocation = value;

                    GetReverseGeocodedLocationAsync(_reverseGeocodeInputLocation).ContinueWith((t) =>
                    {
                        if (t.Status == TaskStatus.RanToCompletion)
                            Place = t.Result;
                    });

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
                    (x) =>
                    {
                        GetSearchedLocationAsync((string)x).ContinueWith((t) =>
                        {
                            if (t.Status == TaskStatus.RanToCompletion)
                                Place = t.Result;
                        }) ;
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
        private async Task<ObservableCollection<string>> GetLocationSuggestionsAsync(string userInput)
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
                    return s;
                }
                catch
                {
                    // If error happens, do not show suggestions
                    return new ObservableCollection<string>();
                }
            }
            else
                return new ObservableCollection<string>();
        }

        /// <summary>
        /// Get location searched by user from the locator
        /// </summary>
        /// <param name="_searchString">User input</param>
        /// <returns>Location that best matches the search string</returns>
        private async Task<GeocodeResult> GetSearchedLocationAsync(string geocodeAddress)
        {
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

                    // return the first match
                    var matches = await Locator.GeocodeAsync(geocodeAddress, geocodeParameters);
                    return matches.FirstOrDefault();
                }
                else
                {
                    ErrorMessage = "Geocoder is not available. Please reload the app. If you continue to receive this message, contact your GIS administrator.";
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Your search request could not be completed";
                StackTrace = ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// Use the locator to perform a reverse geocode operation, returning the place that the user tapped on inside the map
        /// If the user's current location is passed, then set that as the FromPlace used for routing instead
        /// </summary>
        internal async Task<GeocodeResult> GetReverseGeocodedLocationAsync(MapPoint location)
        {
            try
            {
                var matches = await Locator.ReverseGeocodeAsync(location); 
                return matches.First();
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format("Unable to perform reverse geocode request");
                StackTrace = ex.ToString();
                return null;
            }
        }
    }
}
