// /*******************************************************************************
//  * Copyright 2018 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  https://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#if __ANDROID__
using AndroidOS = Android.OS;
using AndroidViews = Android.Views;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TurnByTurnDirections : ContentPage
	{
		public TurnByTurnDirections ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            #if __ANDROID__
            if (AndroidOS.Build.VERSION.SdkInt >= AndroidOS.BuildVersionCodes.P)
            {
                // Xamarin.Forms navigation page doesn't handle safe areas, so revert to default Window behavior
                Android.MainActivity.Instance.Window.ClearFlags(AndroidViews.WindowManagerFlags.LayoutNoLimits);
            }
            #endif
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            #if __ANDROID__
            if (AndroidOS.Build.VERSION.SdkInt >= AndroidOS.BuildVersionCodes.P)
            {
                // Restore window behavior
                Android.MainActivity.Instance.Window.SetFlags(AndroidViews.WindowManagerFlags.LayoutNoLimits, AndroidViews.WindowManagerFlags.LayoutNoLimits);
            }
            #endif

        }
    }
}