using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
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
    public abstract class Spline
    {
        public List<Ellipse> DragEllipses { get; set; }
        public List<Line> LinesOfSpline { get; set; }
        public List<double> ControlPoints { get; set; }
        public List<double> Knots { get; set; }

        public Tuple<double, double, double, double> Range_MinX_MaxX_MinY_MaxY(PlotArea plotArea, WorldArea worldArea)
        {
            double minX = Knots.Min();
            double maxX = Knots.Max();
            double minY = ControlPoints.Min();
            double maxY = ControlPoints.Max();

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
