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
    public class ClampedSpline : Spline
    {
       
        public List<double> Derivations { get; set; }
        public Ellipse LeftDerivationEllipse { get; set; }
        public Ellipse RightDerivationEllipse { get; set; }
       

        public ClampedSpline(HermiteSpline hSpline)
        {
            this.ControlPoints = hSpline.ControlPoints;
            this.Knots = hSpline.Knots;
            this.LinesOfSpline = hSpline.LinesOfSpline;
            this.DragEllipses = hSpline.DragEllipses;
        }

        public ClampedSpline(List<double> knots, List<double> controlPoints, List<double> derivations)
        {
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            this.Derivations = derivations;   
          
        }

        public ClampedSpline(List<double> knots, List<double> controlPoints, List<Ellipse> ellipses) 
        {
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            this.DragEllipses = ellipses;
        }

        public ClampedSpline(List<double> knots, List<double> controlPoints,  List<double> derivations, List<Ellipse> ellipses)
           
        {
            this.Knots = knots;
            this.ControlPoints = controlPoints;           
            
            this.Derivations = derivations;            
            this.DragEllipses = ellipses;
        }

        public ClampedSpline(List<double> knots, List<double> controlPoints, List<double> derivations, List<Ellipse> ellipses, List<Line> linesOfSpline)
            
        {

            
            this.Derivations = derivations;
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            this.DragEllipses = ellipses;
            this.LinesOfSpline = linesOfSpline;
            
        }

        public ClampedSpline(List<double> knots, List<double> controlPoints, List<Ellipse> ellipses, List<Line> linesOfSpline)
        {
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            this.DragEllipses = ellipses;
            this.LinesOfSpline = linesOfSpline;
        }

        public ClampedSpline(List<double> knots, List<double> controlPoints, List<Line> linesOfSpline)
        {
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            this.LinesOfSpline = linesOfSpline;
           
        }

        public ClampedSpline(List<double> knots, List<double> controlPoints, List<double> derivations, List<Line> linesOfSpline)
        {
           
            this.Derivations = derivations;
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            this.LinesOfSpline = linesOfSpline;
        }

        public ClampedSpline()
        {
            // TODO: Complete member initialization
        }

   

    }
}
