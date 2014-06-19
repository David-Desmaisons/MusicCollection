using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows.Threading;
using System.ComponentModel;


namespace MusicCollectionWPF.Infra
{
    public class CustoStarSlider : Slider
    {

        private void DoNothing(Object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private const int _StarNumber = 5;

        //public static double ValueFromPosition(Point pos, Slider d)
        //{
        //    double starnumber = (pos.X) / (50);
        //    starnumber = (starnumber < 0.3) ? 0 : Math.Truncate(starnumber + 1);

        //    return (starnumber * (d.Maximum - d.Minimum) / _StarNumber);
        //}

        private void StarSlider_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel st = sender as StackPanel;
            //Value = StarConverter.ValueFromPosition(e.GetPosition(Im), this);

            Point p = e.GetPosition(st);
            double starnumber = (p.X) / (50);
            starnumber = (starnumber < 0.3) ? 0 : Math.Truncate(starnumber + 1);
            Value = (starnumber * (Maximum - Minimum) / _StarNumber);

            e.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            StackPanel s = (StackPanel)GetTemplateChild("Stars");
  
            s.PreviewMouseUp+=StarSlider_MouseRightButtonDown;
            s.MouseUp+=DoNothing; 
            s.PreviewMouseDown+=DoNothing;
            s.MouseDown += DoNothing;
        }
    }
}
