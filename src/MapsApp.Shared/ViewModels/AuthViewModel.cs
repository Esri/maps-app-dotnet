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
        private ICommand _loginCommand;
        private ICommand _logoutCommand;
        private PortalUser _authenticatedUser;

        public AuthViewModel()
        {
            // Set up authentication manager to handle logins
            UpdateAuthenticationManager();
        }

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
        /// Gets the command to login the user to Portal
        /// </summary>
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new DelegateCommand(
                    async (x) =>
                    {
                        try
                        {
                            var credential = await AuthenticationManager.Current.GenerateCredentialAsync(new Uri(Configuration.ArcGISOnlineUrl));
                            var portal = await ArcGISPortal.CreateAsync(new Uri(Configuration.ArcGISOnlineUrl), credential);
                            AuthenticatedUser = portal.User;
                        }
                        catch
                        {
                            //TODO: do something if unable to login
                            AuthenticatedUser = null;
                        }

                    }));
            }
        }

        /// <summary>
        /// Gets the command to log user out of Portal
        /// </summary>
        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new DelegateCommand(
                    (x) =>
                    {
                        foreach (var credential in AuthenticationManager.Current.Credentials)
                        {
                            AuthenticationManager.Current.RemoveCredential(credential);
                        }
                        AuthenticatedUser = null;
                    }));
            }
        }

        

        // ChallengeHandler function that will be called whenever access to a secured resource is attempted
        public async Task<Credential> CreateCredentialAsync(CredentialRequestInfo info)
        {
            Credential credential = null;

            try
            {
                // IOAuthAuthorizeHandler will challenge the user for OAuth credentials
                credential = await AuthenticationManager.Current.GenerateCredentialAsync(info.ServiceUri);
            }
            catch (Exception ex)
            {
                // Exception will be reported in calling function
                throw (ex);
            }

            return credential;
        }

        private void UpdateAuthenticationManager()
        {
            // Define the server information for ArcGIS Online
            ServerInfo portalServerInfo = new ServerInfo();

            // ArcGIS Online URI
            portalServerInfo.ServerUri = new Uri(Configuration.ArcGISOnlineUrl);

            // Type of token authentication to use
            portalServerInfo.TokenAuthenticationType = TokenAuthenticationType.OAuthImplicit;

            // Define the OAuth information
            OAuthClientInfo oAuthInfo = new OAuthClientInfo
            {
                ClientId = Configuration.AppClientID,
                RedirectUri = new Uri(Configuration.RedirectURL)
            };
            portalServerInfo.OAuthClientInfo = oAuthInfo;

            try
            {
                // Register the ArcGIS Online server information with the AuthenticationManager
                AuthenticationManager.Current.RegisterServer(portalServerInfo);

                //#if __ANDROID__ || __IOS__
                //            thisAuthenticationManager.OAuthAuthorizeHandler = this;
                //            // Use the OAuthAuthorize class in this project to create a new web view to show the login UI
                AuthenticationManager.Current.OAuthAuthorizeHandler = new WPF.Views.OAuthAuthorize();

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
