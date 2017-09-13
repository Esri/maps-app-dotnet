using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.WPF
{
    public class InteractiveCommand : TriggerAction<DependencyObject>
    {    

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InteractiveCommand), null);


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(InteractiveCommand), null);


        public static readonly DependencyProperty InvokeParameterProperty = DependencyProperty.Register(
            "InvokeParameter", typeof(object), typeof(InteractiveCommand), null);

        public static readonly DependencyProperty InputConverterProperty = DependencyProperty.Register(
            "Converter", typeof(IValueConverter), typeof(InteractiveCommand), null);



        private string commandName;

        public object InvokeParameter
        {
            get
            {
                return this.GetValue(InvokeParameterProperty);
            }
            set
            {
                this.SetValue(InvokeParameterProperty, value);
            }
        }


        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        public string CommandName
        {
            get
            {
                return this.commandName;
            }
            set
            {
                if (this.CommandName != value)
                {
                    this.commandName = value;
                }
            }
        }

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

        public object Sender { get; set; }

        protected override void Invoke(object parameter)
        {
            if (Command == null)
            {
                return;
            }

            if (parameter != null)
            {
                InvokeParameter = parameter;
            }

            if (Converter != null)
            {
                InvokeParameter = Converter.Convert(parameter, typeof(object), null, null);
            }
            
            if (this.AssociatedObject != null)
            {
                ICommand command = Command;
                if (command.CanExecute(this.CommandParameter))
                {
                    command.Execute(this.CommandParameter);
                }
            }
        }
    }
}
