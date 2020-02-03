using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.Xamarin.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorDialog : ContentView
	{
		public ErrorDialog ()
		{
			InitializeComponent ();
		}

        /// <summary>
        /// Gets or sets the error message to be shown to the user
        /// </summary>
        public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create("ErrorMessage", typeof(string), typeof(ErrorDialog), null, BindingMode.TwoWay, null, OnErrorMessageChanged);

        /// <summary>
        /// Gets or sets the stack trace to be shown to the user
        /// </summary>
        public static readonly BindableProperty StackTraceProperty = BindableProperty.Create("StackTrace", typeof(string), typeof(ErrorDialog), null, BindingMode.TwoWay, null, OnStackTraceChanged);

        /// <summary>
        /// Gets or sets the ErrorMessage property
        /// </summary>
        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the StackTrace property
        /// </summary>
        public string StackTrace
        {
            get { return (string)GetValue(StackTraceProperty); }
            set { SetValue(StackTraceProperty, value); }
        }

        /// <summary>
        /// Changes the label of the error window when the ErrorMessage property changes
        /// </summary>
        static void OnErrorMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as ErrorDialog;

            // if ErrorMessage is null, set to empty string
            control.ErrorMessageLabel.Text = newValue?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Changes the stack trace of the error window when the ErrorMessage property changes
        /// </summary>
        static void OnStackTraceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as ErrorDialog;

            // if StackTrace is null, set to empty string
            control.StackTraceLabel.Text = newValue?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Clears the error message and in turn the error window disappears
        /// </summary>
        private void ClearErrorMessage(object sender, EventArgs e)
        {
            ErrorMessage = null;
            StackTrace = null; 
        }
    }
}