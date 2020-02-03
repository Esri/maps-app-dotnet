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

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Helpers
{
    static class Configuration
    {
        // URL of the server to authenticate with (ArcGIS Online)
        public const string ArcGISOnlineUrl = @"https://www.arcgis.com/sharing/rest";

        // Client ID for the app registered with the server (Portal Maps)
        public const string AppClientID = "YourClientID";

        // Redirect URL after a successful authorization (configured for the Maps App application)
        public const string RedirectURL = @"https://developers.arcgis.com";

        // Url used for the geocode service
        public const string GeocodeUrl = @"https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";

        // Url used for the routing service
        public const string RouteUrl = @"https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World";
    }
}
