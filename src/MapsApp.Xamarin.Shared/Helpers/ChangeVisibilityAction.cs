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

using Xamarin.Forms;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin.Helpers
{
    /// <summary>
    /// Trigger Action changes the visibility of the TargetName element
    /// </summary>
    class ChangeVisibilityAction: TriggerAction<Element>
    {
        public string TargetName { get; set; }

        protected override void Invoke(Element sender)
        {
            var topView = GetParent(sender);

            var control = topView.FindByName<ListView>(TargetName);

            if (control != null)
            {
                try
                {
                    control.IsVisible = !control.IsVisible;
                }
                catch
                {
                    // this shouls fail silently if control does not have a visibility property
                }
            }
        }

        /// <summary>
        /// Navigates recursively up the vidual tree to the top element
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
