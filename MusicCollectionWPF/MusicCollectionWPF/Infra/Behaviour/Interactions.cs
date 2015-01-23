using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MusicCollection.Infra;
//using System.Windows.Interactivity;

namespace  MusicCollectionWPF.Infra.Behaviour
{
    public class Behaviors : List<System.Windows.Interactivity.Behavior>
    {
    }

    public class Triggers : List<System.Windows.Interactivity.TriggerBase>
    {
    }

    public static class Interactions
    {

        #region Behaviour

        public static Behaviors GetBehaviors(DependencyObject obj)
        {
            return (Behaviors)obj.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject obj, Behaviors value)
        {
            obj.SetValue(BehaviorsProperty, value);
        }

        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", typeof(Behaviors), typeof(Interactions), new UIPropertyMetadata(null, OnPropertyBehaviorsChanged));

        private static void OnPropertyBehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behaviors = System.Windows.Interactivity.Interaction.GetBehaviors(d);
            behaviors.AddCollection(e.NewValue as Behaviors);
        }

        #endregion

        #region Trigger

        public static Triggers GetTriggers(DependencyObject obj)
        {
            return (Triggers)obj.GetValue(TriggersProperty);
        }

        public static void SetTriggers(DependencyObject obj, Triggers value)
        {
            obj.SetValue(TriggersProperty, value);
        }

        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers", typeof(Triggers), typeof(Interactions), new UIPropertyMetadata(null, OnPropertyTriggersChanged));

        private static void OnPropertyTriggersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var triggers = System.Windows.Interactivity.Interaction.GetTriggers(d);
            triggers.AddCollection(e.NewValue as Triggers);
        }

        #endregion
    }
}
