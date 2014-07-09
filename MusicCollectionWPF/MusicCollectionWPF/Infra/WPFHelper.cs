using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace MusicCollectionWPF.Infra
{
    static public class WPFHelper
    {

        public static DependencyObject GetParent(DependencyObject obj)
        {
            if (obj == null)
                return null;

            ContentElement ce = obj as ContentElement;
            if (ce != null)
            {
                DependencyObject parent = ContentOperations.GetParent(ce);
                if (parent != null)
                    return parent;

                FrameworkContentElement fce = ce as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            return VisualTreeHelper.GetParent(obj);
        }

        public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                T objTest = obj as T;
                if (objTest != null)
                    return objTest;
                obj = GetParent(obj);
            }

            return null;
        }

        public static T GetFirstVisualChild<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                return null;

            if (depObj is T)
                return (T)depObj;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                T childItem = GetFirstVisualChild<T>(child);
                if (childItem != null)
                {
                    return childItem;
                }
            }


            return null;
        }

        public static IEnumerable<T> GetVisualChild<T>(this DependencyObject depObj) where T: class
            //where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            if (depObj is T)
                yield return depObj as T;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                var childItems = GetVisualChild<T>(child);
                foreach(T childItem in childItems)
                {
                    yield return childItem;
                }
            }
       }

        public static VisualBrush CreateBrush(this UIElement elt, double width, double height)
        {
            VisualBrush brush = new VisualBrush(elt);
            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
            brush.Viewbox = new Rect(0, 0, width, height);
            brush.ViewboxUnits = BrushMappingMode.Absolute;
            return brush;
        }

        public static Rectangle CreateSnapshot(this UIElement elt)
        {
            Rectangle res = new Rectangle() { Height = elt.RenderSize.Height, Width = elt.RenderSize.Width };

            res.Fill = elt.CreateFrozenBrush();
            res.Stretch = Stretch.Fill;
            return res;
        }

        static public ImageBrush CreateFrozenBrush(this UIElement element)
        {
            ImageBrush res = new ImageBrush(BitmapFrame.Create(element.CreateFrozenImageSource()));
            res.Stretch = Stretch.None;
            res.AlignmentX = AlignmentX.Left;
            res.AlignmentY = AlignmentY.Top;
            res.TileMode = TileMode.None;

            return res;
        }

        static public BitmapSource CreateFrozenImageSource(this UIElement element)
        {
            return element.CreateFrozenImageSource(element.RenderSize.Width, element.RenderSize.Height);
        }


        static public BitmapSource CreateFrozenImageSource(this UIElement element, double width, double height)
        {
            VisualBrush elementBrush = new VisualBrush(element);

            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawRectangle(elementBrush, null, new Rect(new Point(), new Point(width, height)));
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);

            bitmap.Render(visual);
            bitmap.Freeze();

            return bitmap;
        }

        public static Storyboard PrepareStoryboardTransitionTo(Storyboard animator, IEnumerable<UIElement> second, Duration iDuration)
        {
            foreach (UIElement secs in second)
            {
                DoubleAnimation nextAnim = new DoubleAnimation();
                Storyboard.SetTarget(nextAnim, secs);
                Storyboard.SetTargetProperty(nextAnim, new PropertyPath("Opacity"));
                nextAnim.Duration = iDuration;
                nextAnim.From = 0;
                nextAnim.To = 1;
                animator.Children.Add(nextAnim);
            }

            return animator;
        }


        public static Storyboard PrepareStoryboardTransitionTo(this UIElement first, Storyboard animator, UIElement second, Duration iDuration)
        {

            DoubleAnimation prevAnim = new DoubleAnimation();
            Storyboard.SetTarget(prevAnim, first);
            Storyboard.SetTargetProperty(prevAnim, new PropertyPath("Opacity"));
            prevAnim.Duration = iDuration;
            prevAnim.From = 1;
            prevAnim.To = 0;

            DoubleAnimation nextAnim = new DoubleAnimation();
            Storyboard.SetTarget(nextAnim, second);
            Storyboard.SetTargetProperty(nextAnim, new PropertyPath("Opacity"));
            nextAnim.Duration = iDuration;
            nextAnim.From = 0;
            nextAnim.To = 1;

            animator.Children.Add(prevAnim);
            animator.Children.Add(nextAnim);

            return animator;
        }



    }
}
