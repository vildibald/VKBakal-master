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
    public sealed class TransformCoordinates
    {

        public static Point WorldAreaToPlotArea(double x, double y,PlotArea plotArea,WorldArea worldArea)
        {
            double px, py;
            px = plotArea.XMin + (((x - worldArea.XMin) * (plotArea.XMax - plotArea.XMin)) / (worldArea.XMax - worldArea.XMin));
            py = plotArea.YMax- (((y - worldArea.YMin) * (plotArea.YMax - plotArea.YMin)) / (worldArea.YMax - worldArea.YMin));
            return new Point(px, py);
            
        }


        public static Point PlotAreaToWorldArea(double x, double y, PlotArea plotArea, WorldArea worldArea)
        {
            double wx, wy;
            wx = worldArea.XMin + (((x - plotArea.XMin) * (worldArea.XMax - worldArea.XMin)) / (plotArea.XMax - plotArea.XMin));
            wy = worldArea.YMin - (((y - plotArea.YMax) * (worldArea.YMax - worldArea.YMin)) / (plotArea.YMax - plotArea.YMin));
            return new Point(wx, wy);
        }


        public static Point WorldAreaToPlotArea(Point point, PlotArea plotArea, WorldArea worldArea)
        {
            return WorldAreaToPlotArea(point.X, point.Y, plotArea, worldArea);
        }

        public static Point PlotAreaToWorldArea(Point point, PlotArea plotArea, WorldArea worldArea)
        {
            return PlotAreaToWorldArea(point.X, point.Y,plotArea,worldArea);
        }


        public static double WorldAreaToPlotAreaX(double x, PlotArea plotArea, WorldArea worldArea)
        {
            return plotArea.XMin + (((x - worldArea.XMin) * (plotArea.XMax - plotArea.XMin)) / (worldArea.XMax - worldArea.XMin));
        }
        
        public static double WorldAreaToPlotAreaY(double y, PlotArea plotArea, WorldArea worldArea)
        {
            return plotArea.YMax- (((y - worldArea.YMin) * (plotArea.YMax - plotArea.YMin)) / (worldArea.YMax - worldArea.YMin));
        }

        public static double PlotAreaToWorldAreaX(double x, PlotArea plotArea, WorldArea worldArea)
        {
            return worldArea.XMin + (((x - plotArea.XMin) * (worldArea.XMax - worldArea.XMin)) / (plotArea.XMax - plotArea.XMin)); ;
        }

        public static double PlotAreaToWorldAreaY(double y, PlotArea plotArea, WorldArea worldArea)
        {
            return worldArea.YMin - (((y - plotArea.YMax) * (worldArea.YMax - worldArea.YMin)) / (plotArea.YMax - plotArea.YMin));
        }

    }
}
