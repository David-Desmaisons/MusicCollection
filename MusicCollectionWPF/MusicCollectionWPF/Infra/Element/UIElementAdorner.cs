using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MusicCollectionWPF.Infra
{
    internal class UIElementAdorner : Adorner, IDisposable
    {
        //#region Data

        UIElement _child = null;
        //double _offsetLeft = 0;
        //double _offsetTop = 0;

        //#endregion // Data

        #region Constructor

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="adornedElement">The element to which the adorner will be bound.</param>
        public UIElementAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            //adornedElement.LayoutUpdated += new EventHandler(_child_LayoutUpdated);
            IsClipEnabled=true;
            IsHitTestVisible = false;
        }

        public void Dispose()
        {
            //this.AdornedElement.LayoutUpdated -= new EventHandler(_child_LayoutUpdated);
        }

        #endregion // Constructor

        #region Public Interface

        #region Child

        /// <summary>
        /// Returns the child element hosted in the adorner.
        /// </summary>
        public UIElement Child
        {
            get { return _child; }
            set
            {
                if (value == _child)
                    return;

                if (_child != null)
                {
                    base.RemoveLogicalChild(_child);
                    base.RemoveVisualChild(_child);
                    //_child.LayoutUpdated += new EventHandler(_child_LayoutUpdated);
                }

                _child = value;

                if (_child != null)
                {
                    base.AddLogicalChild(_child);
                    base.AddVisualChild(_child);
                    //_child.LayoutUpdated += new EventHandler(_child_LayoutUpdated);
                }
            }
        }

        //void _child_LayoutUpdated(object sender, EventArgs e)
        //{
        //    UpdateLocation();
        //    //this.InvalidateVisual();
        //    //this.InvalidateMeasure();
        //}

        #endregion // Child

        #region GetDesiredTransform

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            //GeneralTransformGroup result = new GeneralTransformGroup();
            //result.Children.Add(base.GetDesiredTransform(transform));
            //result.Children.Add(new TranslateTransform(_offsetLeft, _offsetTop));
            //InvalidateVisual();

            //GeneralTransformGroup result = new GeneralTransformGroup();
            //result.Children.Add(transform);

            //this.RenderTransform = new MatrixTransform(transform.v);
            //result.Children.Add(new TranslateTransform(_offsetLeft, _offsetTop));
            //InvalidateVisual();
            //this.InvalidateArrange();
            //this.InvalidateMeasure();
            return base.GetDesiredTransform(transform);
        }

        #endregion // GetDesiredTransform

        //#region OffsetLeft

        ///// <summary>
        ///// Gets/sets the horizontal offset of the adorner.
        ///// </summary>
        //public double OffsetLeft
        //{
        //    get { return _offsetLeft; }
        //    set
        //    {
        //        _offsetLeft = value;
        //        UpdateLocation();
        //    }
        //}

        //#endregion // OffsetLeft

        //#region SetOffsets

        ///// <summary>
        ///// Updates the location of the adorner in one atomic operation.
        ///// </summary>
        //public void SetOffsets(double left, double top)
        //{
        //    _offsetLeft = left;
        //    _offsetTop = top;
        //    this.UpdateLocation();
        //}

        //#endregion // SetOffsets

        //#region OffsetTop

        ///// <summary>
        ///// Gets/sets the vertical offset of the adorner.
        ///// </summary>
        //public double OffsetTop
        //{
        //    get { return _offsetTop; }
        //    set
        //    {
        //        _offsetTop = value;
        //        UpdateLocation();
        //    }
        //}

        //#endregion // OffsetTop

        #endregion // Public Interface

        #region Protected Overrides

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (_child == null)
                return base.MeasureOverride(constraint);

            _child.Measure(constraint);

            InvalidateVisual();

            return _child.DesiredSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_child == null)
                return base.ArrangeOverride(finalSize);

            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if (_child != null)
                    list.Add(_child);
                return list.GetEnumerator();
            }
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        /// <summary>
        /// Override.  Always returns 1.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return _child == null ? 0 : 1; }
        }

        #endregion // Protected Overrides

        #region Private Helpers

        void UpdateLocation()
        {
            AdornerLayer adornerLayer = base.Parent as AdornerLayer;
            if (adornerLayer != null)
                adornerLayer.Update(base.AdornedElement);
        }

        #endregion // Private Helpers
    }
}
