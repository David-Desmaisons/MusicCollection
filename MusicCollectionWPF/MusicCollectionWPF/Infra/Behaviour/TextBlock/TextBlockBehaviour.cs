using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MusicCollectionWPF.Infra.Behaviour
{
    public class TextBlockBehaviour
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached("ItemsSource",
             typeof(IEnumerable), typeof(TextBlockBehaviour), new PropertyMetadata(null, BehaviourChanged));

        public static IEnumerable GetItemsSource(DependencyObject element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(DependencyObject element, IEnumerable value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty RunStyleProperty = DependencyProperty.RegisterAttached("RunStyle",
             typeof(Style), typeof(TextBlockBehaviour), new PropertyMetadata(null));

        public static Style GetRunStyle(DependencyObject element)
        {
            return (Style)element.GetValue(RunStyleProperty);
        }

        public static void SetRunStyle(DependencyObject element, Style value)
        {
            element.SetValue(RunStyleProperty, value);
        }

        private static void BehaviourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock mi = d as TextBlock;

            if (mi == null)
                return; 
                      
            var nv = (IEnumerable)e.NewValue;

            if (nv!=null)
            {
                Update(nv, mi);
                INotifyCollectionChanged inc = nv as INotifyCollectionChanged;
                if (inc!=null)
                    inc.CollectionChanged += (o, ev) => Update(nv, mi);
            }
    
        }

        private static async void Update(IEnumerable coll, TextBlock iTextBlock)
        {
            await App.Current.Dispatcher.ExecuteAsync(() => UnsafeUpdate(coll.Cast<object>().ToList(), iTextBlock));
        }

        private static void UnsafeUpdate(IEnumerable<object> coll, TextBlock iTextBlock)
        {
            iTextBlock.Inlines.Clear();
            Style st = GetRunStyle(iTextBlock);
            var runs = coll.Select(el => new Run() { DataContext = el, Style = st }).ToList();

            if (runs.Count==0)
                    return;

            iTextBlock.Inlines.Add(runs[0]);
            
            for(int i=1;i<runs.Count-1;i++ )
            {
                iTextBlock.Inlines.Add(", ");
                iTextBlock.Inlines.Add(runs[i]);
            }

            if (runs.Count>1)
            { 
                iTextBlock.Inlines.Add(" & ");
                iTextBlock.Inlines.Add(runs[runs.Count - 1]);
            }
        }
    }
}
