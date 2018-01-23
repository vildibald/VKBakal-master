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

    public class FunctionDefinition
    {
    // all coordinates in WORLD AREA
        //internal List<Ellipse> _knots_ellipses { get; set; }

        public double[] XCoordinates { get; set; }
        public double[] YCoordinates { get; set; }
        public double LeftDerivation {get; set;}
        public double RightDerivation{ get; set; }


        public double[] Knots { get; set; }

        public int[] KnotsIndexes { get; set; }
        //public double[] ControlPoints { get; set; }

      
        public int Degree { get; set; }

        public Function FunctionName { get; set; }

        public FunctionDefinition(int degree, double[] xCoordinates, double[] yCoordinates, double leftDerivation, double rightDerivation)
        {
            this.XCoordinates = xCoordinates;
            this.YCoordinates = yCoordinates;
            
            this.Degree = degree;
            this.LeftDerivation = leftDerivation;
            this.RightDerivation = rightDerivation;
        }

        public FunctionDefinition(double[] xCoordinates, double[] yCoordinates,double leftDerivation, double rightDerivation)
        {
            this.XCoordinates = xCoordinates;
            this.YCoordinates = yCoordinates;
            this.LeftDerivation = leftDerivation;
            this.RightDerivation = rightDerivation;
        }

        public FunctionDefinition(int degree, double[] yCoordinates, double[] knots, double leftDerivation, double rightDerivation, Function functionName)
        {
            this.Degree = degree;
            this.YCoordinates = yCoordinates;
            this.Knots = knots;
            this.FunctionName = functionName;
            this.LeftDerivation = leftDerivation;
            this.RightDerivation = rightDerivation;
        }



        public FunctionDefinition(double[] xCoordinates, double[] yCoordinates,double leftDerivation, double rightDerivation, Function functionName)
        {
            this.XCoordinates = xCoordinates;
            this.YCoordinates = yCoordinates;
            this.FunctionName = functionName;
            this.LeftDerivation = leftDerivation;
            this.RightDerivation = rightDerivation;
        }

       

        public FunctionDefinition(double[] xCoordinates, double[] yCoordinates)
        {
            this.XCoordinates = xCoordinates;
            this.YCoordinates = yCoordinates;
        }

        public FunctionDefinition(int degree, double[] yCoordinates, double[] knots, Function functionName)
        {
            this.Degree = degree;
            this.YCoordinates = yCoordinates;
            this.Knots = knots;
            this.FunctionName = functionName;
        }

        public FunctionDefinition(int degree, double[] xCoordinates, double[] yCoordinates, double[] knots, int[] knotsIndexes, Function functionName)
        {
            this.XCoordinates = xCoordinates;
            this.YCoordinates = yCoordinates;
            this.Knots = knots;
            this.KnotsIndexes = knotsIndexes;
            this.Degree = degree;
            this.FunctionName = functionName;
        }

        public FunctionDefinition(int degree, double[] xCoordinates, double[] yCoordinates, double[] knots, int[] knotsIndexes)
        {
            this.XCoordinates = xCoordinates;
            this.YCoordinates = yCoordinates;
            this.Knots = knots;
            this.KnotsIndexes = knotsIndexes;
            this.Degree = degree;
           
        }

        
        public FunctionDefinition(double[] xCoordinates, double[] yCoordinates, Function functionName)
        {
            this.XCoordinates = xCoordinates;
            this.YCoordinates = yCoordinates;
            this.FunctionName = functionName;
        }

        //public BSpline toBSpline()
        //{
        //    return new BSpline(Degree, ControlPoints, Knots);
        //}
    }
}
