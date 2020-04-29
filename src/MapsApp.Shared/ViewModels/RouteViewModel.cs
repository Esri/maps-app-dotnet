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

using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Commands;
using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Helpers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Http;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.ViewModels
{
    internal class RouteViewModel : BaseViewModel
    {
        private bool _isBusy;
        private GeocodeResult _fromPlace;
        private GeocodeResult _toPlace;
        private RouteResult _route;
        private Viewpoint _areaOfInterest;
        private IReadOnlyList<DirectionManeuver> _directionManeuvers;
        private ICommand _clearRouteCommand;

        /// <summary>
        /// Gets or sets the property indicating whether the app is busy processing
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the start location for the route
        /// </summary>
        public GeocodeResult FromPlace
        {
            get { return _fromPlace; }
            set
            {
                if (_fromPlace != value)
                {
                    _fromPlace = value;
                    _ = GetRouteAsync();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the end location for the route
        /// </summary>
        public GeocodeResult ToPlace
        {
            get { return _toPlace; }
            set
            {
                if (_toPlace != value)
                {
                    _toPlace = value;
                    _ = GetRouteAsync();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the route 
        /// </summary>
        public RouteResult Route
        {
            get { return _route; }
            set
            {
                if (_route != value)
                {
                    _route = value;
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
        /// Gets or sets the turn by turn directions for the returned route
        /// </summary>
        public IReadOnlyList<DirectionManeuver> DirectionManeuvers
        {
            get
            {
                return _directionManeuvers;
            }
            set
            {
                if (_directionManeuvers != value)
                {
                    _directionManeuvers = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command to cancel the route and clear the pins and route off the map
        /// </summary>
        public ICommand ClearRouteCommand
        {
            get
            {
                return _clearRouteCommand ?? (_clearRouteCommand = new DelegateCommand(
                    (_) =>
                    {
                        Route = null;
                        FromPlace = null;
                        ToPlace = null;
                    }));
            }
        }

        /// <summary>
        /// Gets the router for the map
        /// </summary>
        internal RouteTask Router { get; set; }

        /// <summary>
        /// Generates route from the geocoded locations
        /// </summary>
        private async Task GetRouteAsync()
        {
            if (FromPlace == null || ToPlace == null)
            {
                return;
            }

            IsBusy = true;

            if (Router == null)
            {
                try
                {
                    await CreateRouteTask();
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Unable to load routing service. The routing functionality may not work.";
                    StackTrace = ex.ToString();

                    IsBusy = false;
                    return;
                }
            }

            // set the route parameters
            var routeParams = await Router.CreateDefaultParametersAsync();
            routeParams.ReturnDirections = true;
            routeParams.ReturnRoutes = true;

            // add route stops as parameters
            try
            {
                routeParams.SetStops(new List<Stop>() { new Stop(FromPlace.RouteLocation),
                                                            new Stop(ToPlace.RouteLocation) });
                Route = await Router.SolveRouteAsync(routeParams);

                // Set the AOI to an area slightly larger than the route's extent
                var aoiBuilder = new EnvelopeBuilder(Route.Routes.FirstOrDefault()?.RouteGeometry.Extent);
                aoiBuilder.Expand(1.2);
                AreaOfInterest = new Viewpoint(aoiBuilder.ToGeometry());

                // Set turn by turn directions
                DirectionManeuvers = Route.Routes.FirstOrDefault()?.DirectionManeuvers;
            }
            catch (ArcGISWebException e)
            {
                // This is returned when user hits the Cancel button in iOS or the back arrow in Android
                // It does not get caught in the SignInRenderer and needs to be handled here
                if (e.Message.Contains("Token Required"))
                {
                    FromPlace = null;
                    ToPlace = null;
                    IsBusy = false;
                    return;
                }

                ErrorMessage = "A web exception occured. Are you connected to the internet?";
                StackTrace = e.ToString();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong and the routing operation failed.";
                StackTrace = ex.ToString();
            }

            IsBusy = false;
        }

        private async Task CreateRouteTask()
        {
            try
            {
                Router = await RouteTask.CreateAsync(new Uri(Configuration.RouteUrl));
            }
            catch (Exception)
            {
                // Try one more time, to work around a bug. dotnet-api/6024
                try
                {
                    Router = await RouteTask.CreateAsync(new Uri(Configuration.RouteUrl));
                }
                catch (Exception exx)
                {
                    // This is returned when user hits the Cancel button in iOS or the back arrow in Android
                    // It does not get caught in the SignInRenderer and needs to be handled here
                    if (exx.Message.Contains("Token Required"))
                    {
                        FromPlace = null;
                        ToPlace = null;
                        IsBusy = false;
                        return;
                    }

                    throw;
                }
            }
        }
    }
}
