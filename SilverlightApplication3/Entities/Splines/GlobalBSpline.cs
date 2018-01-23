using System;
using System.Linq;
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
    public class GlobalBSpline : BSpline
    {
        public List<double> FunctionValues { get; set; }
        public double LeftDerivation { get; set; }
        public double RightDerivation { get; set; }
        public Ellipse LeftDerivationEllipse { get; set; }
        public Ellipse RightDerivationEllipse { get; set; }

        public List<Ellipse> ControlPointsEllipses { get; set; }
        
       public GlobalBSpline(int degree, List<double> controlPoints, List<double> knots, List<double> functionValues, double leftDerivation, double rightDerivation, 
                        List<Line> linesOfSpline)
           :base(degree, controlPoints,knots,linesOfSpline)
        {
            this.FunctionValues = functionValues;
            this.LeftDerivation = leftDerivation;
            this.RightDerivation = rightDerivation;
        }

       public GlobalBSpline(int degree, List<double> controlPoints, List<double> knots,
                       List<Line> linesOfSpline)
           : base(degree, controlPoints, knots, linesOfSpline)
       {
         
       }

       public GlobalBSpline(int degree, List<double> controlPoints, List<double> knots, List<double> functionValues, double leftDerivation, double rightDerivation, List<Ellipse> ellipses)
           : base(degree, controlPoints, knots, ellipses)
        {
            this.FunctionValues = functionValues;
            this.LeftDerivation = leftDerivation;
            this.RightDerivation = rightDerivation;
        }

       public new Tuple<double, double, double, double> Range_MinX_MaxX_MinY_MaxY(PlotArea plotArea, WorldArea worldArea)
       {
           
           double minX = double.MaxValue;
           double maxX = double.MinValue;
           double minY = double.MaxValue;
           double maxY = double.MinValue;

           for (int i = 0; i < LinesOfSpline.Count; i++)
           {
               var X1 = TransformCoordinates.PlotAreaToWorldAreaX(LinesOfSpline[i].X1, plotArea, worldArea);
               var X2 = TransformCoordinates.PlotAreaToWorldAreaX(LinesOfSpline[i].X2, plotArea, worldArea);
               var Y1 = TransformCoordinates.PlotAreaToWorldAreaY(LinesOfSpline[i].Y1, plotArea, worldArea);
               var Y2 = TransformCoordinates.PlotAreaToWorldAreaY(LinesOfSpline[i].Y2, plotArea, worldArea);
               if (X1 < minX)
                   minX = X1;
               if (X2 > maxX)
                   maxX = X2;
               if (Y1 < minY)
                   minY = Y1;
               if (Y2 > maxY)
                   maxY = Y2;
           }

           //for (int i = 0; i < DragEllipses.Count; i++)
           //{
           //    var X = TransformCoordinates.PlotAreaToWorldAreaX(DragEllipses[i]., plotArea, worldArea);
           //    var Y = TransformCoordinates.PlotAreaToWorldAreaY(DragEllipses[i].X2, plotArea, worldArea);
           //}

           return Tuple.Create(minX, maxX, minY, maxY);
       }
    }
}
