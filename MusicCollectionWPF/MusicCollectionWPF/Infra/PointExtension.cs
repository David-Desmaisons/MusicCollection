using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MusicCollectionWPF.Infra
{
    public static class PointExtension
    {
        static internal double Longueur(this Point eu)
        {
            return (Math.Sqrt(eu.X * eu.X + eu.Y * eu.Y));
        }

        static internal double Determinant(this Point eu, Point outro)
        {
            return eu.X * outro.Y - eu.Y * outro.X;
        }

        static internal double ProduitCartesien(this Point eu, Point outro)
        {
            return eu.X * outro.X + eu.Y * outro.Y;
        }

        static internal double GetAngle(this Point oldpoint, Point newpoint)
        {
            return Math.Asin(oldpoint.GetSin(newpoint)) * 180 / Math.PI;
        }

        static internal double GetSin(this Point oldpoint, Point newpoint)
        {
            double Det = oldpoint.Determinant(newpoint);
            return Det / (oldpoint.Longueur() * newpoint.Longueur());
        }

        static internal double GetCos(this Point oldpoint, Point newpoint)
        {
            double pc = oldpoint.ProduitCartesien(newpoint);
            return pc / (oldpoint.Longueur() * newpoint.Longueur());
        }

        static internal double GetFullAngle(this Point oldpoint, Point newpoint)
        {

            double sin = oldpoint.GetSin(newpoint);
            double cos = oldpoint.GetCos(newpoint);

            double asin = Math.Asin(sin) * 180 / Math.PI;
            double acos = Math.Acos(cos) * 180 / Math.PI;

            if (Math.Abs(asin-acos)<0.0001)
            {
                return asin;
            }
            else
            {
                if ((cos<0) & (sin>=0))
                    return acos;

                return 360 - acos;
            }

        }
    }
}
