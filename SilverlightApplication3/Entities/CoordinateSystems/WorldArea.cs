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
    public struct WorldArea
    {
        public double XMin;// { get; set; }
        public double XMax;// { get; set; }
        public double YMin;// { get; set; }
        public double YMax;// { get; set; }

        public WorldArea(double xMin,double xMax,double yMin,double yMax)
        {
            this.XMin = xMin;
            this.XMax = xMax;
            this.YMin = yMin;
            this.YMax = yMax;
        }
    }
}