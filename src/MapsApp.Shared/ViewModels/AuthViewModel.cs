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
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels
{
    class AuthViewModel : BaseViewModel
    {
        private ICommand _logInOutCommand;
        private  PortalUser _authenticatedUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthViewModel"/> class.
        /// </summary>
        public AuthViewModel()
        {
            // Set up authentication manager to handle logins
            UpdateAuthenticationManager();
        }

        /// <summary>
        /// Gets or sets the authenticated user for the Portal instance provided
        /// </summary>
        public PortalUser AuthenticatedUser
        {
            get { return _authenticatedUser; }
            set
            {
                _authenticatedUser = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the command to login or log out the user
        /// </summary>
        public ICommand LogInOutCommand
        {
            get
            {
                return _logInOutCommand ?? (_logInOutCommand = new DelegateCommand(
                    async (x) =>
                    {
                        try
                        {
                            if (AuthenticatedUser == null)
                            {
                                await TriggerUserLogin();
                            }
                            else
                            {
                                foreach (var credential in AuthenticationManager.Current.Credentials)
                                {
                                    AuthenticationManager.Current.RemoveCredential(credential);
                                }
                                AuthenticatedUser = null;
                            }
                        }
                        catch
                        {
                            //TODO: do something if unable to login
                            AuthenticatedUser = null;
                        }

                    }));
            }
        }

        public async Task TriggerUserLogin()
        {
            var credential = await AuthenticationManager.Current.GenerateCredentialAsync(new Uri(Configuration.ArcGISOnlineUrl));
            var portal = await ArcGISPortal.CreateAsync(new Uri(Configuration.ArcGISOnlineUrl), credential);
            AuthenticatedUser = portal.User;
        }

        // ChallengeHandler function that will be called whenever access to a secured resource is attempted
        public async Task<Credential> CreateCredentialAsync(CredentialRequestInfo info)
        {
            try
            {
                // IOAuthAuthorizeHandler will challenge the user for OAuth credentials
                return await AuthenticationManager.Current.GenerateCredentialAsync(info.ServiceUri);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Set up singleton instance of Authentication manager
        /// </summary>
        private void UpdateAuthenticationManager()
        {
            // Define the server information for ArcGIS Online
            var portalServerInfo = new ServerInfo
            {
                ServerUri = new Uri(Configuration.ArcGISOnlineUrl),
                TokenAuthenticationType = TokenAuthenticationType.OAuthImplicit,
                OAuthClientInfo = new OAuthClientInfo
                {
                    ClientId = Configuration.AppClientID,
                    RedirectUri = new Uri(Configuration.RedirectURL)
                },
            };

            try
            {
                // Register the ArcGIS Online server information with the AuthenticationManager
                AuthenticationManager.Current.RegisterServer(portalServerInfo);

#if WPF
                // In WPF, use the OAuthAuthorize class to create a new web view to show the login UI
                AuthenticationManager.Current.OAuthAuthorizeHandler = new WPF.Views.OAuthAuthorize();
#endif

                // Create a new ChallengeHandler that uses a method in this class to challenge for credentials
                AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CreateCredentialAsync);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
