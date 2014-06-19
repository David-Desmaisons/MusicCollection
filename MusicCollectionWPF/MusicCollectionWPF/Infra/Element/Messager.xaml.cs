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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicCollectionWPF.Infra
{
    /// <summary>
    /// Interaction logic for Messager.xaml
    /// </summary>
    public partial class Messager : UserControl
    {

        public static readonly DependencyProperty TransitionTimeProperty = DependencyProperty.Register("TransitionTime", typeof(TimeSpan), typeof(Messager),
       new PropertyMetadata(TimeSpan.FromSeconds(1)));

        public TimeSpan TransitionTime
        {
            get { return (TimeSpan)GetValue(TransitionTimeProperty); }
            set { SetValue(TransitionTimeProperty, value); }
        }

        private string _Next;
        private Queue<string> _Queue;
        private bool _IsChanging = false;

        public Messager()
        {
            _Queue = new Queue<string>();
            InitializeComponent();
        }

        public void AddMessage(string iMessage)
        {
            _Queue.Enqueue(iMessage);
            PushMessage();
        }

        private void PushMessage()
        {
            if (_IsChanging)
                return;

            if (_Queue.Count==0)
                return;
      
            _IsChanging = true;
      
            _Next = _Queue.Dequeue();
            Next.Content = _Next;

            Storyboard sb = new Storyboard();

            DoubleAnimation db = new DoubleAnimation();
            db.From = 0;
            db.To = -30;
            db.Duration = TransitionTime;

            Storyboard.SetTarget(db, this.Panel);
            Storyboard.SetTargetProperty(db, new PropertyPath("RenderTransform.Y"));

            sb.Children.Add(db);

            EventHandler handler = null;
            handler = delegate
            {
                Current.Content = Next.Content; 
                Transf.Y = 0;
                sb.Completed -= handler;
                sb.Remove();                       
                _IsChanging = false;
                PushMessage();
            };

            sb.Completed += handler;

            sb.Begin();
           
        }

    }
}
