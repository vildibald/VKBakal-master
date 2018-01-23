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
    public class HermiteSpline : Spline
    {

        public List<double> Derivations {get; set; }
        public List<Ellipse> DerivationEllipses {get; set; }
        
        //public List<double> DerivationsX { get; set; }
        //public List<double> Derivations { get; set; }
        //public List<Line> LinesOfSpline { get; set; }

        //public List<Ellipse> DragEllipses { get; set; }

        protected HermiteSpline(HermiteSpline hSpline)
        {
            this.Knots = hSpline.Knots;
            this.LinesOfSpline = hSpline.LinesOfSpline;
            this.Derivations = hSpline.Derivations;
            this.DragEllipses = hSpline.DragEllipses;
            this.ControlPoints = hSpline.ControlPoints;
            
        }

        public HermiteSpline(List<double> knots,List<double> controlPoints)
        {
            this.Knots = knots;
            this.ControlPoints = controlPoints;
            //this.DerivationsX = new List<double>();
            //this.Derivations = new List<double>();

            this.LinesOfSpline = new List<Line>();
            this.DragEllipses = new List<Ellipse>();
        }

        public HermiteSpline(List<double> knots, List<double> controlPoints, List<double> derivations)
        {
            this.Knots = knots;
            this.ControlPoints = controlPoints;
            //this.DerivationsX = new List<double>();
            //this.Derivations = new List<double>();
            this.Derivations = derivations;
            this.LinesOfSpline = new List<Line>();
            this.DragEllipses = new List<Ellipse>();
        }

        public HermiteSpline(List<double> knots, List<double> controlPoints, List<Ellipse> ellipses)
        {
            this.Knots = knots;
            this.ControlPoints = controlPoints;
            //this.DerivationsX = new List<double>();
            //this.Derivations = new List<double>();
            this.LinesOfSpline = new List<Line>();
            this.DragEllipses = ellipses;
        }

        public HermiteSpline(List<double> knots, List<double> controlPoints, List<double> derivations,List<Ellipse> ellipses)
        {
            this.Knots = knots;
            this.ControlPoints = controlPoints;
            this.Derivations = derivations;
            //this.DerivationsX = new List<double>();
            //this.Derivations = new List<double>();
            this.LinesOfSpline = new List<Line>();
            this.DragEllipses = ellipses;
        }

        public HermiteSpline(List<double> knots, List<double> controlPoints, List<double> derivations, List<Ellipse> ellipses, List<Line> linesOfSpline)
        {
            this.Knots = knots;
            this.ControlPoints = controlPoints;
            this.Derivations = derivations;
            //this.DerivationsX = new List<double>();
            //this.Derivations = new List<double>();
            this.LinesOfSpline = linesOfSpline;
            this.DragEllipses = ellipses;
        }

        public HermiteSpline(List<double> knots, List<double> controlPoints, List<Ellipse> ellipses, List<Line> linesOfSpline)
        {

            this.Knots = knots;
            this.ControlPoints = controlPoints;
            this.DragEllipses = ellipses;
            this.LinesOfSpline = linesOfSpline;
            //this.DerivationsX = new List<double>();
            //this.Derivations = new List<double>();
        }

        public HermiteSpline(List<double> knots, List<double> controlPoints, List<Line> linesOfSpline)
        {

            this.Knots = knots;
            this.ControlPoints = controlPoints;
            this.DragEllipses = new List<Ellipse>();
            this.LinesOfSpline = linesOfSpline;
            //this.DerivationsX = new List<double>();
            //this.Derivations = new List<double>();
        }

        public HermiteSpline(List<double> knots, List<double> controlPoints, List<double> derivations, List<Line> linesOfSpline)
        {
            
            this.Knots = knots;
            this.ControlPoints = controlPoints;
           this.LinesOfSpline = linesOfSpline;
           this.Derivations = derivations;

        }

        


    }
}
