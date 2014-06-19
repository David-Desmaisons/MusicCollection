using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using MusicCollection.Infra;

namespace MusicCollectionWPF.Infra
{
    public class SlicerHelper
    {
        private Point _FP;
        private Point _FP2;
        private Point _DestPoint;
        private Point _FP3;
        private double _firstangle;
        private double _secondangle;
        private double _BigRay;
        private double _SmallRay;

        public SlicerHelper(Point Center, double SmallRay, double BigRay, double firstangle, double secondangle)
        {
            _firstangle = firstangle;
            _secondangle = secondangle;
            _BigRay = BigRay;
            _SmallRay = SmallRay;

            _FP = new Point(Center.X + SmallRay * Math.Cos(firstangle), Center.Y - SmallRay * Math.Sin(firstangle));
            _FP2 = new Point(Center.X + BigRay * Math.Cos(firstangle), Center.Y - BigRay * Math.Sin(firstangle));
            _DestPoint = new Point(Center.X + BigRay * Math.Cos(secondangle), Center.Y - BigRay * Math.Sin(secondangle));
            _FP3 = new Point(Center.X + SmallRay * Math.Cos(secondangle), Center.Y - SmallRay * Math.Sin(secondangle));
        }

        public Point FirstPoint
        {
            get { return _FP; }
        }

        public IEnumerable<PathSegment> GetSlicePathSegments(bool IsStroke, bool smoothe)
        {
            yield return new LineSegment(_FP2, IsStroke) { IsSmoothJoin = smoothe };
            bool lag = _secondangle - _firstangle > Math.PI;
            yield return new ArcSegment() { IsStroked = IsStroke, IsSmoothJoin = smoothe, Point = _DestPoint, SweepDirection = SweepDirection.Counterclockwise, Size = new Size(_BigRay, _BigRay), IsLargeArc = lag };
            yield return new LineSegment(_FP3, IsStroke) { IsSmoothJoin = IsStroke };
            yield return new ArcSegment() { IsStroked = IsStroke, IsSmoothJoin = smoothe, Point = _FP, SweepDirection = SweepDirection.Clockwise, Size = new Size(_SmallRay, _SmallRay), IsLargeArc = lag };
        }

        public PathGeometry GetPathGeometry(bool IsStroke, bool smoothe)
        {
            PathFigure pf = new PathFigure() { StartPoint = FirstPoint, IsClosed = true };
            pf.Segments.AddCollection(GetSlicePathSegments(IsStroke, smoothe));
            var l = new List<PathFigure>(); l.Add(pf);
            return new PathGeometry(l);
        }
    }
}
