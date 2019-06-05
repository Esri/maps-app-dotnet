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

using Android.App;
using Esri.ArcGISRuntime.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Auth;
using Esri.ArcGISRuntime.ExampleApps.MapsApp.Helpers;
using Esri.ArcGISRuntime.ExampleApps.MapsApp.Xamarin;

[assembly: ExportRenderer(typeof(StartPage), typeof(Esri.ArcGISRuntime.ExampleApps.MapsApp.Android.SignInPageRenderer))]
namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.Android
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
        // IOAuthAuthorizeHandler.AuthorizeAsync implementation
        public Task<IDictionary<string, string>> AuthorizeAsync(Uri serviceUri, Uri authorizeUri, Uri callbackUri)
        {
            // Create a task completion source
            var taskCompletionSource = new TaskCompletionSource<IDictionary<string, string>>();

            // Get the current Android Activity
            var activity = this.Context as Activity;

            // Create a new Xamarin.Auth.OAuth2Authenticator using the information passed in
            var authenticator = new OAuth2Authenticator(
                clientId: Configuration.AppClientID,
                scope: "",
                authorizeUrl: authorizeUri,
                redirectUrl: callbackUri);

            // Allow the user to cancel the OAuth attempt
            authenticator.AllowCancel = true;

            // Hide errors
            authenticator.ShowErrors = false;

            // Define a handler for the OAuth2Authenticator.Completed event
            authenticator.Completed += (sender, authArgs) =>
            {
                try
                {
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
                    taskCompletionSource.TrySetException(ex);
                }
                finally
                {
                    // Dismiss the OAuth login
                    activity.FinishActivity(99);
                }
            };

            // If an error was encountered when authenticating, set the exception on the TaskCompletionSource
            authenticator.Error += (sndr, errArgs) =>
            {
                // If the user cancels, the Error event is raised but there is no exception
                if (errArgs.Exception != null)
                {
                    taskCompletionSource.TrySetException(errArgs.Exception);
                }
                else if (errArgs.Message == "OAuth Error = The user denied your request." && taskCompletionSource.Task.Status != TaskStatus.Faulted)
                {
                    authenticator.OnCancelled();
                }

                activity.FinishActivity(99);
            };

            // Present the OAuth UI so the user can enter user name and password
            var intent = authenticator.GetUI(activity);
            activity.StartActivityForResult(intent, 99);

            // Return completion source task so the caller can await completion
            return taskCompletionSource.Task;
        }
        #endregion
    }
}