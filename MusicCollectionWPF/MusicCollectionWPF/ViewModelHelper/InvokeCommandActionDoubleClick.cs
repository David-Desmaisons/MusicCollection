using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

//Adapted from MVVMLigth
// Copyright © GalaSoft Laurent Bugnion 2009-2014

namespace MusicCollectionWPF.ViewModelHelper
{
    public class InvokeCommandActionDoubleClick : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
       "CommandParameter", typeof(object), typeof(InvokeCommandActionDoubleClick), new PropertyMetadata(null, CommandParameterPropertyChanged));


        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private static void CommandParameterPropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var sender = s as InvokeCommandActionDoubleClick;
            if (sender == null)
                return;

            if (sender.AssociatedObject == null)
                return;

            sender.EnableDisableElement();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand),
                    typeof(InvokeCommandActionDoubleClick),new PropertyMetadata(null, CommandPropertyChanged));

        private static void CommandPropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            OnCommandChanged(s as InvokeCommandActionDoubleClick, e);
        }

        /// <summary>
        /// Identifies the <see cref="MustToggleIsEnabled" /> dependency property
        /// </summary>
        public static readonly DependencyProperty MustToggleIsEnabledProperty = DependencyProperty.Register(
            "MustToggleIsEnabled", typeof(bool), typeof(InvokeCommandActionDoubleClick),
            new PropertyMetadata(false, CommandParameterPropertyChanged));
    
        public bool MustToggleIsEnabled
        {
            get { return (bool)GetValue(MustToggleIsEnabledProperty); }
            set  { SetValue(MustToggleIsEnabledProperty, value); }
        }

      
        /// <summary>
        /// Called when this trigger is attached to a FrameworkElement.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            EnableDisableElement();
        }

        private FrameworkElement GetAssociatedObject()
        {
            return AssociatedObject as FrameworkElement;
        }

        /// <summary>
        /// Provides a simple way to invoke this trigger programatically
        /// without any EventArgs.
        /// </summary>
        public void Invoke()
        {
            Invoke(null);
        }

        /// <summary>
        /// Executes the trigger.
        /// <para>To access the EventArgs of the fired event, use a RelayCommand&lt;EventArgs&gt;
        /// and leave the CommandParameter and CommandParameterValue empty!</para>
        /// </summary>
        /// <param name="parameter">The EventArgs of the fired event.</param>
        protected override void Invoke(object parameter)
        {
            if (AssociatedElementIsDisabled())
                return;

            MouseButtonEventArgs mea = parameter as MouseButtonEventArgs;
            if ((mea==null) || (mea.ClickCount!=2))
                return;

            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }

        private static void OnCommandChanged( InvokeCommandActionDoubleClick element, DependencyPropertyChangedEventArgs e)
        {
            if (element == null)
                return;

            if (e.OldValue != null)
            {
                ((ICommand)e.OldValue).CanExecuteChanged -= element.OnCommandCanExecuteChanged;
            }

            var command = (ICommand)e.NewValue;

            if (command != null)
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;

            element.EnableDisableElement();
        }

        private bool AssociatedElementIsDisabled()
        {
            var element = GetAssociatedObject();
            return AssociatedObject == null || (element != null  && !element.IsEnabled);
        }

        private void EnableDisableElement()
        {
            var element = GetAssociatedObject();
            if (element == null)
                return;

            if (MustToggleIsEnabled && Command != null)
                element.IsEnabled = Command.CanExecute(CommandParameter);
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            EnableDisableElement();
        }
    }
}
