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

using System;
using Xamarin.Forms;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin.Helpers
{
    /// <summary>
    /// Trigger Action changes the visibility of the TargetName element
    /// </summary>
    class ChangeVisibilityAction: TriggerAction<Element>
    {
        /// <summary>
        /// Gets or sets the TargetName for the control to be modified
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// Trigger action 
        /// </summary>
        protected override void Invoke(Element sender)
        {
            var topView = GetParent(sender);

            var control = topView.FindByName<ListView>(TargetName);

            if (control != null)
            {
                try
                {
                    // assume the senser is SearchBar or Entry control
                    // return if the control is not infocus
                    if (sender is SearchBar)
                    {
                        var searchBar = sender as SearchBar;
                        control.IsVisible = (searchBar.IsFocused == true && !string.IsNullOrEmpty(searchBar.Text)) ? true : false;
                    }
                    else if (sender is Entry)
                    {
                        control.IsVisible = !control.IsVisible;
                    }
                }
                catch (Exception ex)
                {
                    // Fail silently
                }
            }
        }

        /// <summary>
        /// Navigates recursively up the visual tree to the top element
        /// </summary>
        private Element GetParent(Element visualElement)
        {
            var view = visualElement.Parent;
            if (view != null )
            {
                GetParent(view);
            }
            return visualElement;
        }
    }
}
