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

using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Helpers;
using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.iOS;
using Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin;
using Esri.ArcGISRuntime.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(StartPage), typeof(SignInPageRenderer))]
namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.iOS
{
    public class SignInPageRenderer : PageRenderer, IOAuthAuthorizeHandler
    {
        // ctor
        public SignInPageRenderer()
        {
            // Set the OAuth authorization handler to this class (Implements IOAuthAuthorize interface)
            AuthenticationManager.Current.OAuthAuthorizeHandler = this;
        }

        #region IOAuthAuthorizationHandler implementation
        public Task<IDictionary<string, string>> AuthorizeAsync(Uri serviceUri, Uri authorizeUri, Uri callbackUri)
        {
            // Create a task completion source
            var taskCompletionSource = new TaskCompletionSource<IDictionary<string, string>>();

            // Create a new Xamarin.Auth.OAuth2Authenticator using the information passed in
            var authenticator = new OAuth2Authenticator(
                clientId: Configuration.AppClientID,
                scope: "",
                authorizeUrl: authorizeUri,
                redirectUrl: callbackUri);

            // Allow the user to cancel the OAuth attempt
            authenticator.AllowCancel = true;

            // hide error messages (Xamarin bug)
            authenticator.ShowErrors = false;

            // Define a handler for the OAuth2Authenticator.Completed event
            authenticator.Completed += (sender, authArgs) =>
            {
                try
                {
                    // Dismiss the OAuth UI when complete
                    this.DismissViewController(true, null);

                    // Throw an exception if the user could not be authenticated
                    if (!authArgs.IsAuthenticated)
                    {
                        throw new Exception("Unable to authenticate user.");
                    }

                    // If authorization was successful, get the user's account
                    Account authenticatedAccount = authArgs.Account;

                    // Set the result (Credential) for the TaskCompletionSource
                    taskCompletionSource.SetResult(authenticatedAccount.Properties);
                }
                catch (Exception ex)
                {
                    // If authentication failed, set the exception on the TaskCompletionSource
                    taskCompletionSource.SetException(ex);
                }
            };

            // If an error was encountered when authenticating, set the exception on the TaskCompletionSource
            authenticator.Error += (sndr, errArgs) =>
            {
                // Dismiss the OAuth UI when complete
                this.DismissViewController(true, null);

                // If the user cancels, the Error event is raised but there is no exception ... best to check first
                if (errArgs.Exception != null)
                {
                    taskCompletionSource.TrySetException(errArgs.Exception);
                }
                else if (errArgs.Message == "OAuth Error = The user denied your request." && taskCompletionSource.Task.Status != TaskStatus.Faulted)
                {
                    authenticator.OnCancelled();
                }
            };

            // Present the OAuth UI (on the app's UI thread) so the user can enter user name and password
            InvokeOnMainThread(() =>
            {
                this.PresentViewController(authenticator.GetUI(), true, null);
            });

            // Return completion source task so the caller can await completion
            return taskCompletionSource.Task;
        }
        #endregion
    }
}