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
// <author>Mara Stoica</author>

namespace MapsApp.ViewModels 
{
    using Esri.ArcGISRuntime.Mapping;

    class MapViewModel : BaseViewModel
    {
        private Map map;

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get
            {
                return this.map;
            }

            set
            {
                if (this.map != value)
                {
                    this.map = value;
                    this.OnPropertyChanged(nameof(this.Map));
                }
            }
        }

        public MapViewModel()
        {
            this.Map = new Map(Basemap.CreateTopographicVector());
        }
    }
}
