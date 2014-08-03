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

        public static Geometry BorderFromPoints(double haw, double hah, double taw,double tah,double arcvalue)
        {
            //double Margin = 10;
            //double before = Margin + arcvalue;
            double startheaderxmargin = 20;
            double yupmargin = hah / 2;
            double ymargin = 5;
            double xmargin = 2;

            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(haw + startheaderxmargin, yupmargin);

            Size ArcSize = new Size(arcvalue, arcvalue);

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(new LineSegment(new Point(taw - arcvalue, yupmargin), true));
            myPathSegmentCollection.Add(new ArcSegment(new Point(taw - xmargin, yupmargin + arcvalue), ArcSize, 90, false, SweepDirection.Clockwise, true));

            myPathSegmentCollection.Add(new LineSegment(new Point(taw - xmargin, tah - arcvalue - ymargin), true));
            myPathSegmentCollection.Add(new ArcSegment(new Point(taw - arcvalue - xmargin, tah - ymargin), ArcSize, 90, false, SweepDirection.Clockwise, true));

            myPathSegmentCollection.Add(new LineSegment(new Point(arcvalue, tah - ymargin), true));
            myPathSegmentCollection.Add(new ArcSegment(new Point(0, tah - ymargin - arcvalue), ArcSize, 90, false, SweepDirection.Clockwise, true));

            myPathSegmentCollection.Add(new LineSegment(new Point(0, arcvalue + yupmargin), true));
            myPathSegmentCollection.Add(new ArcSegment(new Point(arcvalue / 2, yupmargin + arcvalue / 2), ArcSize, 45, false, SweepDirection.Clockwise, true));
    
            myPathFigure.Segments = myPathSegmentCollection;

            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);

            PathGeometry pg = new PathGeometry();
            pg.Figures = myPathFigureCollection;
            return pg;
        }
    }
}
