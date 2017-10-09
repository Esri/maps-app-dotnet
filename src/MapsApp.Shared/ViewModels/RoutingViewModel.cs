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
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels
{
    class RoutingViewModel : BaseViewModel
    {
        private ICommand _triggerRouteCommand;
        private string _startLocation;
        private string _endLocation;

		/// <summary>
        /// Initializes a new instance of the <see cref="RoutingViewModel"/> class.
        /// </summary>
        public RoutingViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                // Load locator 
                Locator = await LocatorTask.CreateAsync(new Uri(Configuration.GeocodeUrl));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public ICommand TriggerRouteCommand
        {
            get
            {
                return _triggerRouteCommand ?? (_triggerRouteCommand = new DelegateCommand(
                    async (X) =>
                    {
                        await LoadRoutingService();
                    }));
            }
        }

        /// <summary>
        /// Gets or sets the end location for the route
        /// </summary>
        public string EndLocation
        {
            get { return _endLocation; }
            set
            {
                if (_endLocation != value)
                {
                    _endLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the end location for the route
        /// </summary>
        public string StartLocation
        {
            get { return _startLocation; }
            set
            {
                if (_startLocation != value)
                {
                    _startLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the geocoder for the map
        /// </summary>
        internal LocatorTask Locator { get; set; }

        private async Task<MapPoint> GetMapPointForAddress(string address)
        {
            try
            {
                // Locate the searched feature
                if (Locator != null)
                {
                    var geocodeParameters = new GeocodeParameters
                    {
                        MaxResults = 1
                    };
                    var matches = await Locator.GeocodeAsync(address, geocodeParameters);
                    return matches?.FirstOrDefault()?.DisplayLocation;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private async Task LoadRoutingService()
        { 
            var routeTask = await RouteTask.CreateAsync(new Uri(Configuration.RouteUrl));

            // set the route parameters
            var routeParams = await routeTask.CreateDefaultParametersAsync();
            routeParams.ReturnDirections = true;
            routeParams.ReturnRoutes = true;

            // set the route stops
            var start = new Stop (await GetMapPointForAddress(StartLocation));
            var end = new Stop (await GetMapPointForAddress(EndLocation));

            // add route stops as parameters
            routeParams.SetStops(new List<Stop>() { start, end });

            var route = await routeTask.SolveRouteAsync(routeParams);
        }

        ///// <summary>
        ///// Loads the user specified portal instance
        ///// </summary>
        //private async Task LoadPortal()
        //{
        //    try
        //    {
        //        if (AuthViewModel.Instance.AuthenticatedUser == null)
        //        {
        //            await AuthViewModel.Instance.TriggerUserLogin();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Unable to connect to Portal. " + ex.ToString());
        //    }
        //}
    }
}
