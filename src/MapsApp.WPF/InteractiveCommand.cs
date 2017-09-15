using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.WPF
{
    /// <summary>
    /// Command used to pass event args from event in xaml
    /// </summary>
    public class InteractiveCommand : TriggerAction<DependencyObject>
    {    

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InteractiveCommand), null);


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(InteractiveCommand), null);

        public static readonly DependencyProperty InputConverterProperty = DependencyProperty.Register(
            "Converter", typeof(IValueConverter), typeof(InteractiveCommand), null);

        /// <summary>
        /// Gets or sets the command to be executed
        /// </summary>
        public ICommand Command
        {
            get{ return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the parameter that will be passed to the command
        /// </summary>
        public object CommandParameter
        {
            get{ return GetValue(CommandParameterProperty); }
            set{ SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the converter that will change the format of the event args data
        /// </summary>
        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(InputConverterProperty); }
            set { SetValue(InputConverterProperty, value); }
        }

        /// <summary>
        /// When the event fires, the command is executed and event args parameter passed through
        /// </summary>
        protected override void Invoke(object parameter)
        {
            if (Command == null)
            {
                return;
            }

            object resolvedParameter;

            if (Converter != null)
            {
                resolvedParameter = Converter.Convert(parameter, typeof(object), null, null);
            }
            else
            {
                resolvedParameter = parameter;
            }

            if (AssociatedObject != null)
            {
                if (Command.CanExecute(resolvedParameter))
                {
                    Command.Execute(resolvedParameter);
                }
            }
        }
    }
}
