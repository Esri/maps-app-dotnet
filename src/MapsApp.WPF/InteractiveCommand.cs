using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Esri.ArcGISRuntime.ExampleApps.MapsApp.WPF
{
    public class InteractiveCommand : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter)
        {
            //if (base.AssociatedObject != null)
            //{
            //    ICommand command = this.ResolveCommand();
            //    if ((command != null) && command.CanExecute(parameter))
            //    {
            //        command.Execute(parameter);
            //    }
            //}
        }

    }
}
