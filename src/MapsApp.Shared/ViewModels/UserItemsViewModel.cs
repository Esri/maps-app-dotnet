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

using Esri.ArcGISRuntime.ExampleApps.MapsApp.Commands;
using Esri.ArcGISRuntime.Portal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels
{
    class UserItemsViewModel : BaseViewModel
    {
        private ObservableCollection<PortalItem> _userItems;
        private PortalItem _selectedUserItem;
        private ICommand _loadUserItemsCommand;
        private ICommand _discardUserItemsCommand;
        // TODO: Figure out what are all the item types that should be supported
        // Portal item types that should be displayed
        private static readonly ICollection<PortalItemType> _validUserItemTypes =
            new PortalItemType[] { PortalItemType.WebMap};

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
        /// Gets the command to load maps for the authenticated user
        /// </summary>
        public ICommand LoadUserItemsCommand
        {
            get
            {
                return _loadUserItemsCommand ?? (_loadUserItemsCommand = new DelegateCommand(
                    async (x) =>
                    {
                        await LoadUserItems();
                    }));
            }
        }

        /// <summary>
        /// Gets the command to clear maps for the authenticated user
        /// </summary>
        public ICommand DiscardUserItemsCommand
        {
            get
            {
                return _discardUserItemsCommand ?? (_discardUserItemsCommand = new DelegateCommand(
                    (x) =>
                    {
                        UserItems = null;
                    }));
            }
        }

        /// <summary>
        /// Loads user maps from Portal
        /// </summary>
        public async Task LoadUserItems()
        {
            var portalUser = AuthViewModel.Instance.AuthenticatedUser?.Portal?.User as PortalUser;

            try
            {
                var userContent = await portalUser.GetContentAsync();

                UserItems = new ObservableCollection<PortalItem>();
                foreach (var item in userContent.Items)
                {
                    if (_validUserItemTypes.Contains(item.Type))
                        UserItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Unable to retrieve user maps";
                StackTrace = ex.ToString();
            }
        }
    }
}
