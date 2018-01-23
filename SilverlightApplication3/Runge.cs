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
    public class Runge
    {
    // all coordinates in WORLD AREA
     //   internal List<Ellipse> _knots_ellipses { get; set; }

        public double[] _XCoordinates { get; set; }
        public double[] _YCoordinates { get; set; }

        public double[] _XCoordinatesOfKnots { get; set; }

        public int[] _KnotsIndexes { get; set; }
        public int _Degree { get; set; }



        public Runge(double[] xCoordinates, double[] yCoordinates, double[] xCoordinatesOfKnots, int[] knotsIndexes, int degree)
        {
            this._XCoordinates = xCoordinates;
            this._YCoordinates = yCoordinates;
            this._XCoordinatesOfKnots = xCoordinatesOfKnots;
            this._KnotsIndexes = knotsIndexes;
            this._Degree = degree;
        }


    }
}
