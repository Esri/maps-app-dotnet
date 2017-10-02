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

using Esri.ArcGISRuntime.Portal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels
{
    class UserItemsViewModel : BaseViewModel
    {
        private ObservableCollection<PortalItem> _userItems;
        private PortalItem _selectedUserItem;
        private AuthViewModel _authViewModel;
        // TODO: Figure out what are all the item types that should be supported
        // Portal item types that should be displayed
        private static readonly ICollection<PortalItemType> validUserItemTypes = 
            new PortalItemType[] { PortalItemType.WebMap};

        /// <summary>
        /// Initializes a new instance of the <see cref="UserItemsViewModel"/> class.
        /// </summary>
        public UserItemsViewModel()
        {
            
        }

        /// <summary>
        /// Gets or sets the user item the user selected
        /// </summary>
        public PortalItem SelectedUserItem
        {
            get { return _selectedUserItem; }
            set
            {
                if (_selectedUserItem != value && value != null)
                {
                    _selectedUserItem = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the AuthViewModel instance to keep track of authenticated users and log on/off events
        /// </summary>
        public AuthViewModel AuthViewModel
        {
            get
            {
                return _authViewModel;
            }
            set
            {
                    _authViewModel = value;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    LoadPortal();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    OnPropertyChanged();
            }
        }

        /// <summary>
        /// Property holding the list of user items to be added to the UI
        /// </summary>
        public ObservableCollection<PortalItem> UserItems
        {
            get { return _userItems; }
            set
            {
                if (_userItems != value )
                {
                    _userItems = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Loads the user specified portal instance
        /// </summary>
        private async Task LoadPortal()
        {
            try
            {
                if (AuthViewModel.AuthenticatedUser == null)
                {
                    UserItems = null;
                    await AuthViewModel.TriggerUserLogin();
                }

                await LoadUserItems(AuthViewModel.AuthenticatedUser.Portal);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to connect to Portal. " + ex.ToString());
            }
        }

        /// <summary>
        /// Loads user maps from Portal
        /// </summary>
        private async Task LoadUserItems(ArcGISPortal portal)
        {          
            var portalUser = portal.User as PortalUser;
            var userContent = await portalUser.GetContentAsync();

            UserItems = new ObservableCollection<PortalItem>();

            foreach (var item in userContent.Items)
            {
                if (validUserItemTypes.Contains(item.Type))
                    UserItems.Add(item);
            }
        }
    }
}
