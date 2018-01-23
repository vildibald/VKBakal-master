using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightApplication3
{
    public class HermiteSpline
    {

        public List<double> PointsX { get; set; }
        public List<double> PointsY { get; set; }
        public List<double> DerivationsX { get; set; }
        public List<double> DerivationsY { get; set; }
        public List<Line> LinesOfSpline { get; set; }

        public List<Ellipse> _Ellipses { get; set; }

        public HermiteSpline(List<double> pointsX,List<double> pointsY)
        {
            this.PointsX = pointsX;
            this.PointsY = pointsY;
            this.DerivationsX = new List<double>();
            this.DerivationsY = new List<double>();

            this.LinesOfSpline = new List<Line>();
            this._Ellipses = new List<Ellipse>();
        }

        public HermiteSpline(List<double> pointsX, List<double> pointsY, List<double> derivationsX, List<double> derivationsY)
        {
            this.PointsX = pointsX;
            this.PointsY = pointsY;
            this.DerivationsX = new List<double>();
            this.DerivationsY = new List<double>();   
            this.LinesOfSpline = new List<Line>();
            this._Ellipses = new List<Ellipse>();
        }

        public HermiteSpline(List<double> pointsX, List<double> pointsY, List<Ellipse> ellipses)
        {
            this.PointsX = pointsX;
            this.PointsY = pointsY;
            this.DerivationsX = new List<double>();
            this.DerivationsY = new List<double>();
            this.LinesOfSpline = new List<Line>();
            this._Ellipses = ellipses;
        }

        public HermiteSpline(List<double> pointsX, List<double> pointsY, List<double> derivationsX, List<double> derivationsY,List<Ellipse> ellipses)
        {
            this.PointsX = pointsX;
            this.PointsY = pointsY;
            this.DerivationsX = new List<double>();
            this.DerivationsY = new List<double>();
            this.LinesOfSpline = new List<Line>();
            this._Ellipses = ellipses;
        }

        public HermiteSpline(List<double> pointsX, List<double> pointsY, List<double> derivationsX, List<double> derivationsY, List<Ellipse> ellipses, List<Line> linesOfSpline)
        {
            this.PointsX = pointsX;
            this.PointsY = pointsY;
            this.DerivationsX = new List<double>();
            this.DerivationsY = new List<double>();
            this.LinesOfSpline = linesOfSpline;
            this._Ellipses = ellipses;
        }

        public HermiteSpline(List<double> pointsX, List<double> pointsY, List<Ellipse> ellipses, List<Line> linesOfSpline)
        {
           
            this.PointsX = pointsX;
            this.PointsY = pointsY;
            this._Ellipses = ellipses;
            this.LinesOfSpline = linesOfSpline;
            this.DerivationsX = new List<double>();
            this.DerivationsY = new List<double>();
        }

        


    }
}
