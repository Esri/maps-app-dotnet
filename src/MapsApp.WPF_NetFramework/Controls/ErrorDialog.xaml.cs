using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Esri.ArcGISRuntime.OpenSourceApps.MapsApp.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog : UserControl
    {
        public ErrorDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the error message to be shown to the user
        /// </summary>
        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register("ErrorMessage", typeof(string), typeof(ErrorDialog), new PropertyMetadata(OnErrorMessageChanged));

        /// <summary>
        /// Gets or sets the stack trace to be shown to the user
        /// </summary>
        public static readonly DependencyProperty StackTraceProperty = DependencyProperty.Register("StackTrace", typeof(string), typeof(ErrorDialog), new PropertyMetadata(OnStackTraceChanged));

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
        static void OnErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ErrorDialog;

            // if ErrorMessage is null, set to empty string
            control.ErrorMessageLabel.Text = e.NewValue?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Changes the stack trace of the error window when the ErrorMessage property changes
        /// </summary>
        static void OnStackTraceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ErrorDialog;

            // if StackTrace is null, set to empty string
            control.StackTraceLabel.Text = e.NewValue?.ToString() ?? string.Empty;
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
