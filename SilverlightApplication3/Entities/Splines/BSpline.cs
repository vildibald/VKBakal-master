using System;
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
using System.Collections.Generic;

namespace SilverlightApplication3
{
    public class BSpline : Spline
    {

       // public List<double> ControlPoints { get; set; }
       // public bool isInteractive { get; set; }
        public int Degree { get; set; }
       
        //public List<Ellipse> AuxEllipses { get; set; }
       
        
       // public bool AreKnotsChangeable { get; set; }
       // public List<int> IntervalLineIndexes { get; set; }

        //public List<double> FunctionValues { get; set; }


        public BSpline(int degree, List<double> controlPoints, List<double> knots, List<Ellipse> ellipses)
        {
            this.LinesOfSpline = new List<Line>();
           this.Degree=degree;
            this.DragEllipses = ellipses;
            this.ControlPoints = controlPoints;
            this.Knots = knots;
          //  this.AuxEllipses = new List<Ellipse>();
           // IntervalLineIndexes = new List<int>();
            //isInteractive = false;
        }

        public BSpline(int degree, List<double> controlPoints, List<double> knots, List<Line> lines, List<Ellipse> ellipses)
        {
            this.LinesOfSpline = lines;
            this.Degree = degree;
            this.DragEllipses = ellipses;
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            //this.AreKnotsChangeable = false;
            //this.AuxEllipses = new List<Ellipse>();
            //IntervalLineIndexes = new List<int>();
            //isInteractive = false;
        }

        public BSpline(int degree, List<double> controlPoints, List<double> knots)
        {
            this.LinesOfSpline = new List<Line>();
            this.DragEllipses = new List<Ellipse>();
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            this.Degree = degree;
            //this.AreKnotsChangeable = false;
            //this.AuxEllipses = new List<Ellipse>();
            //IntervalLineIndexes = new List<int>();
            //isInteractive = false;
        }

        public BSpline(int degree)
        {
            LinesOfSpline = new List<Line>();
           this.Degree=degree;
            DragEllipses = new List<Ellipse>();
            ControlPoints = new List<double>();
            Knots = new List<double>();
            //this.AreKnotsChangeable = false;
            //this.AuxEllipses = new List<Ellipse>();
            //IntervalLineIndexes = new List<int>();
            //isInteractive = false;
        }

        public BSpline(int degree, List<double> controlPoints, List<double> knots, List<Line> linesOfSpline)
        {
            // TODO: Complete member initialization
            this.Degree = degree;
            this.ControlPoints = controlPoints;
            this.Knots = knots;
            this.LinesOfSpline = linesOfSpline;
            //isInteractive = false;
        }

        //public BSpline(int degree, List<double> controlPoints, List<double> knots, 
        //                List<Line> linesOfSpline)
        //{
        //    // TODO: Complete member initialization
        //    this.Degree = degree;
        //    this.ControlPoints = controlPoints;
        //    this.Knots = knots;
        //    this.LinesOfSpline = linesOfSpline;
        //    //this.IntervalLineIndexes = intervalLineIndexes;
        //    isInteractive = false;
        //}

        public BSpline(int degree, double[] controlPoints, double[] knots)
        {
           
            this.Degree = degree;
            this.ControlPoints = controlPoints.ToList();
            this.Knots = knots.ToList();
        }

        //public void add_def(Ellipse cp_n_knots_ellipse, double cp, double knot)
        //{
           
        //    DragEllipses.Add(cp_n_knots_ellipse);
        //    ControlPoints.Add(cp);
        //    knots.Add(knot);

        //}

        //public void add_line(Line line)
        //{
        //    Lines.Add(line);
        //}


        //public void add_lines(List<Line> lines)
        //{
        //    Lines.AddRange(lines);
        //}     

        internal static double[] ServiceKnots(double[] knots, int degree, bool generateBothSides)
        {
            int length = knots.Length;

            double max = knots[length-1];
            double min = knots[0];
            double h = (max - min) / (length - 1);
            double[] serviceKnots;
            if (generateBothSides)
            {
                serviceKnots = new double[2 * degree + length];
                Array.Copy(knots, 0, serviceKnots, degree, length);
                for (int i = 0; i < degree; i++)
                {
                    serviceKnots[i] = min - (degree  - i) * h;
                    serviceKnots[degree + length + i] = max + (i + 1) * h;
                }
            }
            else
            {
                serviceKnots = new double[degree + length];
                Array.Copy(knots, 0, serviceKnots, degree, length);
                for (int i = 0; i < degree; i++)
                {
                    serviceKnots[i] = min - (degree + 1 - (i + 1)) * h;
                }
            }

            return serviceKnots;
        }

        internal double[] KnotsWithoutServiceKnots()
        {
            var knotsLength = this.Knots.Count-2*this.Degree;
            var knots = new double[knotsLength];
            for (int i = 0; i < knotsLength; i++)
            {
                knots[i] = this.Knots[i + this.Degree];
            }
            return knots;
        }
    }
}
