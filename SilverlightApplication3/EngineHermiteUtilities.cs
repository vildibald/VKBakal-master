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
    internal class EngineHermiteUtilities
    {
        internal double HermiteBasis(double x, double left, double top, double leftTopDerivation, double right, double bottom, double rightBottomDerivaation)
        {
            double xMinusRight = x - right;
            double xMinusLeft = x - left;
            double leftMinusRight = left - right;
            double rightMinusLeft = -leftMinusRight;

            double BF1Left = (Math.Pow(xMinusRight, 2) * (1 - (2 * (xMinusLeft)) / (leftMinusRight))) / Math.Pow(leftMinusRight, 2);
            double BF1Right = (Math.Pow(xMinusLeft, 2) * (1 - (2 * (xMinusRight)) / (rightMinusLeft))) / Math.Pow(rightMinusLeft, 2);
            double BF2Left = (Math.Pow(xMinusRight, 2) * (xMinusLeft)) / Math.Pow(leftMinusRight, 2);
            double BF2Right = (Math.Pow(xMinusLeft, 2) * (xMinusRight)) / Math.Pow(rightMinusLeft, 2);

            return top * BF1Left + leftTopDerivation * BF2Left + bottom * BF1Right + rightBottomDerivaation * BF2Right;
        }

    }
}
