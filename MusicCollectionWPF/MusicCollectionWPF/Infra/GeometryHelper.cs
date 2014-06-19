using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace MusicCollectionWPF.Infra
{
    public static class GeometryHelper
    {

        public static Geometry BorderFromPoints(double haw, double taw,double tah)
        {
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(haw+45,20);

            Size ArcSize = new Size(20,20);

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(new LineSegment(new Point(taw-30, 20), true));
            myPathSegmentCollection.Add(new ArcSegment(new Point(taw-10, 40), ArcSize,90,false,SweepDirection.Clockwise,true));

            myPathSegmentCollection.Add(new LineSegment(new Point(taw - 10, tah - 30), true));
            myPathSegmentCollection.Add(new ArcSegment(new Point(taw - 30, tah - 10), ArcSize, 90, false, SweepDirection.Clockwise, true));

            myPathSegmentCollection.Add(new LineSegment(new Point( 30, tah - 10), true));
            myPathSegmentCollection.Add(new ArcSegment(new Point( 10, tah - 30), ArcSize, 90, false, SweepDirection.Clockwise, true));

            myPathSegmentCollection.Add(new LineSegment(new Point(10, 40), true));
            myPathSegmentCollection.Add(new ArcSegment(new Point(30, 20), ArcSize, 90, false, SweepDirection.Clockwise, true));
    
            myPathFigure.Segments = myPathSegmentCollection;

            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);

            PathGeometry pg = new PathGeometry();
            pg.Figures = myPathFigureCollection;
            return pg;
        }
    }
}
