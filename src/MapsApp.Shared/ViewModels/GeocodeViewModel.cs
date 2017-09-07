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
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.ExampleApps.MapsApp.Commands;
using Esri.ArcGISRuntime.Geometry;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels
{
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
                Locator = await LocatorTask.CreateAsync(new Uri(GeocodeServiceUrl));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
            
        }

        /// <summary>
        /// Gets the geocoder for the map
        /// </summary>yea
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        // Call method to get location suggestions
                        // Disable unawaited async warning
                        GetLocationSuggestionsAsync(_searchText);
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
                    GetSearchedLocationAsync(_selectedSuggestion);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
                if (value != null)
                {
                    _suggestionsList = value;
                    OnPropertyChanged();
                }
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
                        await GetSearchedLocationAsync((string)x);
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
            var locatorInfo = Locator?.LocatorInfo;
            if (locatorInfo == null)
                return;

            if (!string.IsNullOrEmpty(userInput) && locatorInfo.SupportsSuggestions)
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
        private async Task GetSearchedLocationAsync(string geocodeAddress)
        {
            SuggestionsList.Clear();
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
                    Place = matches.FirstOrDefault();
                }
                else
                {
                    ErrorMessage = "Unable to load geocoder";
                }

                // Select located feature on map
                if (Place != null)
                {
                    // Set viewpoint to the feature's extent
                    AreaOfInterest = Place.Extent != null ? new Viewpoint(Place.Extent) :
                        new Viewpoint(Place.DisplayLocation, 4000);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
        }
    }
}
