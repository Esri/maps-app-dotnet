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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.ViewModels
{
    /// <summary>
    /// Base View Model that all View Models inherit
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private string _errorMessage;
        private string _stackTrace;

        public BaseViewModel()
        {

        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the error message to be shown to the user
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the stack trace to be shown to the user
        /// </summary>
        public string StackTrace
        {
            get { return _stackTrace; }
            set
            {
                if (_stackTrace != value)
                {
                    _stackTrace = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
