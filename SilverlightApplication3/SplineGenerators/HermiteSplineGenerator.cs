using System;
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
    internal class HermiteSplineGenerator
    {
        internal double HermiteSplineFunctionValue(double x, double x0, double y0, double x1, double y1, double d0, double d1)
        {
            double xMinusRight = x - x1;
            double xMinusLeft = x - x0;
            double leftMinusRight = x0 - x1;
            double rightMinusLeft = -leftMinusRight;

            double BF11 = (Math.Pow(xMinusRight, 2) * (1 - (2 * (xMinusLeft)) / (leftMinusRight))) / Math.Pow(leftMinusRight, 2);
            double BF12 = (Math.Pow(xMinusLeft, 2) * (1 - (2 * (xMinusRight)) / (rightMinusLeft))) / Math.Pow(rightMinusLeft, 2);
            double BF21 = (Math.Pow(xMinusRight, 2) * (xMinusLeft)) / Math.Pow(leftMinusRight, 2);
            double BF22 = (Math.Pow(xMinusLeft, 2) * (xMinusRight)) / Math.Pow(rightMinusLeft, 2);

            return y0 * BF11 + d0 * BF21 + y1 * BF12 + d1 * BF22;
        }

        //internal double DualHermiteBasis(double x, double x0, double y0, double x1, double y1, double x2, double y2, double leftDerivation, double d2)
        //{

        //}

    }
}
