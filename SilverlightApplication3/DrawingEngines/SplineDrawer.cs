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
    public class SplineDrawer
    {

        private Canvas _canvas;
        private PlotArea _plotArea;
        private WorldArea _worldArea;

        private Point _previous;
        private const int W = 8;
        private const int H = 8;
        private const int W_DIV_2 = W / 2;
        private const int H_DIV_2 = H / 2;
        internal const double DERIVATIVE_AUX_KNOT_DISTANCE = 0.15;
        
        private SolidColorBrush KNOT_COLOR;
        private SolidColorBrush AUX_KNOT_COLOR;
        private SolidColorBrush KNOT_COLOR2;
        private SolidColorBrush AUX_KNOT_COLOR2;
        private SolidColorBrush KNOT_COLOR3;

        private SolidColorBrush SPLINE_COLOR_1_A;
        private SolidColorBrush SPLINE_COLOR_1_B;
        private SolidColorBrush SPLINE_COLOR_1;
        private SolidColorBrush AUX_KNOT_COLOR3;

        private SolidColorBrush SPLINE_COLOR_2_A;
        private SolidColorBrush SPLINE_COLOR_2_B;

        private SolidColorBrush SPLINE_COLOR_2;

        private AuxiliaryDrawer _auxiliaryEngine;
        private Point _waZeroPointInPACoordinates;
        private BSplineGenerator _bSplineEngine;
        private GlobalBSplineGenerator _globalBSplineEngine;
        private HermiteSplineGenerator _hermiteSplineEngine;
        private ClampedSplineGenerator _clampedSplineEngine;

        private double _minX;
        private double _maxX;
        private double _minY;
        private double _maxY;

        //public double 

       public double DrawPrecision { get; set; }
        //private double DrawPrecisionDiv2;

       private void Initialize(Canvas canvas, PlotArea pa, WorldArea wa, double drawPrecision)
       {
           this._canvas = canvas;
           this._plotArea = pa;
           this._worldArea = wa;
           this.DrawPrecision = drawPrecision;
           _waZeroPointInPACoordinates = TransformCoordinates.WorldAreaToPlotArea(0, 0, _plotArea, _worldArea);
           InitColors();
           this._bSplineEngine = new BSplineGenerator();
           this._hermiteSplineEngine = new HermiteSplineGenerator();
           this._clampedSplineEngine = new ClampedSplineGenerator();
           this._globalBSplineEngine = new GlobalBSplineGenerator();
            
       }

       public SplineDrawer(Canvas canvas, PlotArea pa, WorldArea wa, double drawPrecision)
       {
           Initialize(canvas, pa, wa, drawPrecision);
           //DrawPrecisionDiv2 = DrawPrecisionDiv2 / 2;
           _auxiliaryEngine = new AuxiliaryDrawer(canvas);

       }

        public SplineDrawer(Canvas canvas, PlotArea pa, WorldArea wa, double drawPrecision, AuxiliaryDrawer auxiliaryEngine)
        {
            Initialize(canvas, pa, wa, drawPrecision);
            _auxiliaryEngine = auxiliaryEngine;
            
        }

        public void CurveThickness(int value)
        {
            _auxiliaryEngine.LineThickness = value;
        }


        private void InitColors()
        {
            AUX_KNOT_COLOR3 = new SolidColorBrush(Color.FromArgb(96, 96, 64, 64));
            AUX_KNOT_COLOR2 = new SolidColorBrush(Color.FromArgb(127, 127, 127, 20));
            KNOT_COLOR2 = new SolidColorBrush(Color.FromArgb(192, 20, 127, 20));
            KNOT_COLOR = new SolidColorBrush(Color.FromArgb(192, 127, 127, 20));
            KNOT_COLOR3 = new SolidColorBrush(Color.FromArgb(96, 96, 96, 64));
            AUX_KNOT_COLOR = new SolidColorBrush(Color.FromArgb(127, 127, 20, 20));
            //////AUX_KNOT_COLOR = KNOT_COLOR;
            //////AUX_KNOT_COLOR2 = KNOT_COLOR2;
            SPLINE_COLOR_1_A = new SolidColorBrush(Color.FromArgb(255, 20, 20, 235));
            SPLINE_COLOR_1_B = new SolidColorBrush(Color.FromArgb(255, 235, 20, 20));
            ////// SPLINE_COLOR_1_A = new SolidColorBrush(Color.FromArgb(255, 235, 235, 20));
             //////SPLINE_COLOR_1_B = new SolidColorBrush(Color.FromArgb(255, 235, 20, 235));

            SPLINE_COLOR_2_A = SPLINE_COLOR_1_A;//new SolidColorBrush(Color.FromArgb(255, 127, 127, 127));
            SPLINE_COLOR_2_B = SPLINE_COLOR_1_B;//new SolidColorBrush(Color.FromArgb(255, 235, 20, 235));
            //SPLINE_COLOR_ON_BLACK_CANVAS_1 = new SolidColorBrush(Color.FromArgb(255, 235, 235, 235));
            //SPLINE_COLOR_ON_BLACK_CANVAS_2 = new SolidColorBrush(Color.FromArgb(255, 235, 20, 235));
            SPLINE_COLOR_1 = SPLINE_COLOR_1_A;
            SPLINE_COLOR_2 = SPLINE_COLOR_2_A;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public void PolynomialFunction(double[] arguments, double leftInterval, double rightInterval)
        {
            _previous = new Point();
            bool canMakeLine = false;
            double y;
            SolidColorBrush color = new SolidColorBrush();
            color.Color = Color.FromArgb(200, 192, 192, 192);
            do
            {
                y = arguments[5] * Math.Pow(leftInterval, 5) + arguments[4] * Math.Pow(leftInterval, 4) + arguments[3] * Math.Pow(leftInterval, 3) +
                    arguments[2] * Math.Pow(leftInterval, 2) + arguments[1] * leftInterval + arguments[0];
                Point current = TransformCoordinates.WorldAreaToPlotArea(leftInterval, y, _plotArea, _worldArea);
                if (canMakeLine)
                {
                    _auxiliaryEngine.DrawLine(_previous.X, _previous.Y, current.X, current.Y, color);
                }
                leftInterval = leftInterval + DrawPrecision;
                canMakeLine = true;
                _previous = current;
            } while (leftInterval <= rightInterval);

        }



        public void BSplineOfDegree3(double knot, double cp0, double cp1, double cp2, double cp3)
        {
            double[] vv = { knot - 3, knot - 2, knot - 1, knot, knot + 1, knot + 2, knot + 3, knot + 4 };
            BSplineOfDegree3(vv, cp0, cp1, cp2, cp3);
        }

        public void BSplineOfDegree3(double[] knot, double cp0, double cp1, double cp2, double cp3)
        {


            double x = knot[3];
            double y = 0;
            Point previous = new Point();
            Point current = new Point();
            SolidColorBrush color = new SolidColorBrush();
            color.Color = Color.FromArgb(200, 0, 0, 0);

            do
            {
                y = cp3 * Math.Pow(x - knot[3], 3)
                    + cp2 * (Math.Pow(x - knot[2], 3) - ((Math.Pow(x - knot[3], 3) * (knot[2] - knot[4]) * (knot[2] - knot[5]) * (knot[2] - knot[6])) / ((knot[3] - knot[4]) * (knot[3] - knot[5]) * (knot[3] - knot[6]))))
                    + cp1 * (Math.Pow(x - knot[1], 3) - ((Math.Pow(x - knot[2], 3) * (knot[1] - knot[3]) * (knot[1] - knot[4]) * (knot[1] - knot[5])) / ((knot[2] - knot[3]) * (knot[2] - knot[4]) * (knot[2] - knot[5]))) -
                    ((Math.Pow(x - knot[3], 3) * (knot[1] - knot[2]) * (knot[1] - knot[4]) * (knot[1] - knot[5])) / ((knot[3] - knot[2]) * (knot[3] - knot[4]) * (knot[3] - knot[5]))))
                    + cp0 * (Math.Pow(x - knot[0], 3) - ((Math.Pow(x - knot[1], 3) * (knot[0] - knot[2]) * (knot[0] - knot[3]) * (knot[0] - knot[4])) / ((knot[1] - knot[2]) * (knot[1] - knot[3]) * (knot[1] - knot[4]))) -
                    ((Math.Pow(x - knot[2], 3) * (knot[0] - knot[1]) * (knot[0] - knot[3]) * (knot[0] - knot[4])) / ((knot[2] - knot[1]) * (knot[2] - knot[3]) * (knot[2] - knot[4]))) -
                    ((Math.Pow(x - knot[3], 3) * (knot[0] - knot[1]) * (knot[0] - knot[2]) * (knot[0] - knot[4])) / ((knot[3] - knot[1]) * (knot[3] - knot[2]) * (knot[3] - knot[4]))));
                current = TransformCoordinates.WorldAreaToPlotArea(x, y, _plotArea, _worldArea);
                if (x != knot[3])
                {

                    _auxiliaryEngine.DrawLine(previous, current, color);
                }

                previous = current;
                x += 0.01;
            } while (x <= knot[4]);

        }

        public void BellFunctionOfDegree3(double v1, double v2, double v3, double v4, double v5)
        {
            double x = v1;
            double y = 0;
            Point previous = new Point();
            Point current = new Point();
            bool canMakeLine = false;


            do
            {
                if (x <= v2)
                {
                    y = Math.Pow(x - v1, 3);
                    current = TransformCoordinates.WorldAreaToPlotArea(x, y, _plotArea, _worldArea);
                    if (canMakeLine)
                    {
                        SolidColorBrush color = new SolidColorBrush();
                        color.Color = Color.FromArgb(200, 255, 0, 0);
                        _auxiliaryEngine.DrawLine(previous, current, color);
                    }
                    else
                    {
                        //  color.Color = Color.FromArgb(200, 80, 80, 80);
                        //  line(plotAreaMin_X, 0, v1, 0, color);
                        //  line(v4, 0, plotAreaMax_X, 0, color);
                        canMakeLine = true;
                    }

                }
                else if (x <= v3)
                {
                    y = Math.Pow(x - v1, 3) - ((Math.Pow(x - v2, 3) * (v1 - v3) * (v1 - v4) * (v1 - v5)) / ((v2 - v3) * (v2 - v4) * (v2 - v5)));
                    current = TransformCoordinates.WorldAreaToPlotArea(x, y, _plotArea, _worldArea);
                    SolidColorBrush color = new SolidColorBrush();
                    color.Color = Color.FromArgb(200, 0, 255, 0);
                    _auxiliaryEngine.DrawLine(previous.X, previous.Y, current.X, current.Y, color);
                }
                else if (x <= v4)
                {
                    y = Math.Pow(x - v1, 3) - ((Math.Pow(x - v2, 3) * (v1 - v3) * (v1 - v4) * (v1 - v5)) / ((v2 - v3) * (v2 - v4) * (v2 - v5))) - ((Math.Pow(x - v3, 3) * (v1 - v2) * (v1 - v4) * (v1 - v5)) / ((v3 - v2) * (v3 - v4) * (v3 - v5)));
                    current = TransformCoordinates.WorldAreaToPlotArea(x, y, _plotArea, _worldArea);
                    SolidColorBrush color = new SolidColorBrush();
                    color.Color = Color.FromArgb(200, 0, 0, 255);
                    _auxiliaryEngine.DrawLine(previous.X, previous.Y, current.X, current.Y, color);
                }
                else
                {
                    y = Math.Pow(x - v1, 3) - ((Math.Pow(x - v2, 3) * (v1 - v3) * (v1 - v4) * (v1 - v5)) / ((v2 - v3) * (v2 - v4) * (v2 - v5))) - ((Math.Pow(x - v3, 3) * (v1 - v2) * (v1 - v4) * (v1 - v5)) / ((v3 - v2) * (v3 - v4) * (v3 - v5))) - ((Math.Pow(x - v4, 3) * (v1 - v2) * (v1 - v3) * (v1 - v5)) / ((v4 - v2) * (v4 - v3) * (v4 - v5)));
                    current = TransformCoordinates.WorldAreaToPlotArea(x, y, _plotArea, _worldArea);
                    SolidColorBrush color = new SolidColorBrush();
                    color.Color = Color.FromArgb(200, 0, 255, 255);
                    _auxiliaryEngine.DrawLine(previous.X, previous.Y, current.X, current.Y, color);
                }

                previous = current;
                x += DrawPrecision;
            } while (x <= v5);
        }

        public void BSplineBasisFunctionsOfDegree3(double v1, double v2, double v3, double v4, double v5, double v6, double v7, double v8)
        {
            double x = 0;
            double y1 = 0;
            double y2 = 0;
            double y3 = 0;
            double y4 = 0;

            Point previous1 = new Point();
            Point current1 = new Point();
            Point previous2 = new Point();
            Point current2 = new Point();
            Point previous3 = new Point();
            Point current3 = new Point();
            Point previous4 = new Point();
            Point current4 = new Point();

            bool canMakeLine = false;

            do
            {
                y1 = Math.Pow(x - v1, 3) - ((Math.Pow(x - v2, 3) * (v1 - v3) * (v1 - v4) * (v1 - v5)) / ((v2 - v3) * (v2 - v4) * (v2 - v5))) -
                    ((Math.Pow(x - v3, 3) * (v1 - v2) * (v1 - v4) * (v1 - v5)) / ((v3 - v2) * (v3 - v4) * (v3 - v5))) -
                    ((Math.Pow(x - v4, 3) * (v1 - v2) * (v1 - v3) * (v1 - v5)) / ((v4 - v2) * (v4 - v3) * (v4 - v5)));
                y2 = Math.Pow(x - v2, 3) - ((Math.Pow(x - v3, 3) * (v2 - v4) * (v2 - v5) * (v2 - v6)) / ((v3 - v4) * (v3 - v5) * (v3 - v6))) -
                    ((Math.Pow(x - v4, 3) * (v2 - v3) * (v2 - v5) * (v2 - v6)) / ((v4 - v3) * (v4 - v5) * (v4 - v6)));
                y3 = Math.Pow(x - v3, 3) - ((Math.Pow(x - v4, 3) * (v3 - v5) * (v3 - v6) * (v3 - v7)) / ((v4 - v5) * (v4 - v6) * (v4 - v7)));
                y4 = Math.Pow(x - v4, 3);
                current1 = TransformCoordinates.WorldAreaToPlotArea(x, y1, _plotArea, _worldArea);
                current2 = TransformCoordinates.WorldAreaToPlotArea(x, y2, _plotArea, _worldArea);
                current3 = TransformCoordinates.WorldAreaToPlotArea(x, y3, _plotArea, _worldArea);
                current4 = TransformCoordinates.WorldAreaToPlotArea(x, y4, _plotArea, _worldArea);

                if (canMakeLine)
                {
                    SolidColorBrush color1 = new SolidColorBrush();
                    SolidColorBrush color2 = new SolidColorBrush();
                    SolidColorBrush color3 = new SolidColorBrush();
                    SolidColorBrush color4 = new SolidColorBrush();
                    SolidColorBrush color = new SolidColorBrush();
                    color1.Color = Color.FromArgb(200, 255, 0, 0);
                    _auxiliaryEngine.DrawLine(previous1.X, previous1.Y, current1.X, current1.Y, color1);

                    color2.Color = Color.FromArgb(200, 0, 255, 0);
                    _auxiliaryEngine.DrawLine(previous2.X, previous2.Y, current2.X, current2.Y, color2);

                    color3.Color = Color.FromArgb(200, 0, 0, 255);
                    _auxiliaryEngine.DrawLine(previous3.X, previous3.Y, current3.X, current3.Y, color3);

                    color4.Color = Color.FromArgb(200, 0, 255, 255);
                    _auxiliaryEngine.DrawLine(previous4.X, previous4.Y, current4.X, current4.Y, color4);


                }
                else
                {
                    canMakeLine = true;
                }
                previous1 = current1;
                previous2 = current2;
                previous3 = current3;
                previous4 = current4;

                x += DrawPrecision;
            } while (x <= 1);
        }

        /// <summary>
        /// Calculate and draws b-spline with visible knots/control knots where parameters are in WORLD AREA coordinate system.
        /// </summary>
        /// <param name="knots"></param>
        /// <param name="int"></param>
        /// <param name="controlPoints"></param>
        public BSpline InteractiveBSpline(int degree, double[] knots, double[] controlPoints)
        {
            return InteractiveBSpline(degree, knots, controlPoints, false);
        }


        public BSpline InteractiveBSpline(int degree, double[] knots, double[] controlPoints, bool containsServiceKnots)
        {

            var ellipses = new List<Ellipse>();
            var serviceKnots = containsServiceKnots ? knots : SilverlightApplication3.BSpline.ServiceKnots(knots, degree, true);

            double PAControlPoint;
            double PAKnot;

            for (int i = 0; i < controlPoints.Length; i++)
            {

                PAKnot = TransformCoordinates.WorldAreaToPlotAreaX(serviceKnots[i + 1], _plotArea, _worldArea);
                PAControlPoint = TransformCoordinates.WorldAreaToPlotAreaY(controlPoints[i], _plotArea, _worldArea);

                if (i < degree - 1)
                {
                    ellipses.Add(_auxiliaryEngine.DrawEllipse(PAKnot - W_DIV_2, PAControlPoint - H_DIV_2, AUX_KNOT_COLOR));

                }
                else
                {
                    ellipses.Add(_auxiliaryEngine.DrawEllipse(PAKnot - W_DIV_2, PAControlPoint - H_DIV_2, KNOT_COLOR));
                }

            }
            var bSpline = BSpline(degree, serviceKnots, controlPoints, true);
            bSpline.DragEllipses = ellipses;
            //gbSpline.isInteractive = true;
            return bSpline;
        }

        /// <summary>
        /// Calculate and draws b-spline where parameters are in WORLD AREA coordinate system
        /// </summary>
        /// <param name="int"></param>
        /// <param name="knots"></param>
        /// <param name="controlPoints"></param>
        /// <param name="containsServiceKnots">If knots doesn't contains service ones, method calculate them.</param>
        public BSpline BSpline(int degree, double[] knots, double[] controlPoints, bool containsServiceKnots)
        {
            var lines = BSplineLines(degree, knots, controlPoints, containsServiceKnots);
            return new BSpline(degree, controlPoints.ToList(), knots.ToList(), lines);
        }

        public void SwitchSplineColor_1()
        {
            SPLINE_COLOR_1 = SPLINE_COLOR_1.Equals(SPLINE_COLOR_1_A) ? SPLINE_COLOR_1_B : SPLINE_COLOR_1_A;
        }

        public void SwitchSplineColor_2()
        {
            SPLINE_COLOR_2 = SPLINE_COLOR_2.Equals(SPLINE_COLOR_2_A) ? SPLINE_COLOR_2_B : SPLINE_COLOR_2_A;
        }

        public List<Line> BSplineLines(int degree, double[] knots, double[] controlPoints, bool containsServiceKnots)
        {
            if (containsServiceKnots)
            {
                return PartialBSplineLines(degree, knots, controlPoints, 0, controlPoints.Length - degree);
            }
            else
            {
                var auxKnots = SilverlightApplication3.BSpline.ServiceKnots(knots, degree, true);
                return PartialBSplineLines(degree, auxKnots, controlPoints, 0, controlPoints.Length - degree);
            }

        }

        internal List<Line> PartialBSplineLines(int degree, double[] knotsWithGeneratedServiceKnots, double[] controlPoints, int fromIdx, int toIdx)
        {
            var lines = new List<Line>();
            var controlPointsCount = controlPoints.Length;
            var isDegreeEven = degree % 2 == 0;
            double x = 0;
            double y = 0;
            var previous = new Point();
            var current = new Point();
            //EngineUtils.ServiceKnots(knots, degree, true);
            SetDefaultSplineColor_1();

            for (int i = fromIdx; i < toIdx; i++)
            {
                bool canDraw = false;
                x = knotsWithGeneratedServiceKnots[i + degree];

                while (x <= knotsWithGeneratedServiceKnots[i + degree + 1])
                {
                    
                    y = 0;
                    for (int j = 0; j <= degree; j++)
                    {
                        try
                        {
                            double[] momentarilyUsedKnots = new double[degree + 2];


                            Array.Copy(knotsWithGeneratedServiceKnots, j + i, momentarilyUsedKnots, 0, degree + 2);
                            if (x >= knotsWithGeneratedServiceKnots[i + degree + 1] - DrawPrecision) x = knotsWithGeneratedServiceKnots[i + degree + 1];
                                y += controlPoints[i + j] * _bSplineEngine.Bell(x, degree, momentarilyUsedKnots, isDegreeEven, j + 1);//degree + 2); 
                            
                        }
                        catch (Exception) { }
                    }

                    //current = isDegreeEven ? TransformCoordinates.WorldAreaToPlotArea(x, y, PA, WA) : TransformCoordinates.WorldAreaToPlotArea(x, -y, PA, WA);
                    current = TransformCoordinates.WorldAreaToPlotArea(x, -y, _plotArea, _worldArea);
                   // if (x != knotsWithGeneratedServiceKnots[i + degree])
                    if (canDraw)
                    {
                        lines.Add(_auxiliaryEngine.DrawLine(previous, current, SPLINE_COLOR_1));
                    }
                    canDraw = true;
                    //else { 
                    // _auxiliaryEngine.DrawEllipse(current.knots-W_DIV_2,current.controlPoints-H_DIV_2,_SplineColor);
                    //}

                    previous = current;
                    x += DrawPrecision;

                }
                SwitchSplineColor_1();
            }

            return lines;
        }

        /// <summary>
        /// Calculate and draws runge function where parameters are in WORLD AREA coordinate system
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="numberOfPoints"></param>
        /// <param name="numberOfInterval"></param>
        /// /// <param name="degree"></param>
        public Ellipse[] RungeFunction(double left, double right, int numberOfPoints, int numberOfInterval, int degree)
        {
            SolidColorBrush ellipseColor = new SolidColorBrush(Color.FromArgb(192, 20, 20, 127));
            SolidColorBrush knotEllipseColor = new SolidColorBrush(Color.FromArgb(192, 20, 127, 127));
            Ellipse[] ellipses = new Ellipse[numberOfPoints];
            double h = (right - left) / (numberOfPoints - 1);
            double x = left;
            double y = 0;


            Point point;

            int[] knotsIndexes = new int[numberOfInterval + 1];

            for (int i = 0; i < knotsIndexes.Length; i++)
            {
                knotsIndexes[i] = i * (numberOfPoints - 1) / numberOfInterval;
            }

            int j = 0;
            for (int i = 0; i < numberOfPoints; i++)
            {
                y = 1 / (1 + x * x);

                point = TransformCoordinates.WorldAreaToPlotArea(x, y, _plotArea, _worldArea);

                if (i == knotsIndexes[j])
                {
                    ellipses[i] = _auxiliaryEngine.DrawEllipse(point, knotEllipseColor);
                    j++;
                }
                else
                {
                    ellipses[i] = _auxiliaryEngine.DrawEllipse(point, ellipseColor);
                }
                x += h;
            }



            return ellipses;
        }




        public BSpline RegressBSpline(int degree, double[] pointsX, double[] pointsY, double[] inputKnots)
        {
            var lines = new List<Line>();
            Tuple<double[], int[]> knotsAndIndexesInPointsX;
            Tuple<double[], double[]> controlPointsAndAuxKnots;
            //double x = 0;
            //double y = 0;
           // var previous = new Point();
            //var current = new Point();
            int numberOfIntervals = inputKnots.Length - 1;

            knotsAndIndexesInPointsX = ArrayMyUtils.FindNearestValuesAndIndexes(pointsX, inputKnots);
            controlPointsAndAuxKnots = _bSplineEngine.FunctionApproximationControlPointsAndAuxiliaryKnots(degree, pointsX, pointsY, knotsAndIndexesInPointsX.Item1, knotsAndIndexesInPointsX.Item2);
            
            return InteractiveBSpline(degree, controlPointsAndAuxKnots.Item1, controlPointsAndAuxKnots.Item2,true);
        }



        public BSpline FunctionApproximationBSpline(int degree, double left, double right, int numberOfPoints, int numberOfIntervals, Function functionToApproximate)
        {
            //var lines = new List<Line>();
            FunctionDefinition functionDefinition;
           // double x = left;
            //double y = 0;
            //double[] BSF = new double[numberOfIntervals];
            functionDefinition = _bSplineEngine.FunctionApproximationDefinition(degree, left, right, numberOfPoints, numberOfIntervals, functionToApproximate);

            var controlPointsAndAuxKnots = _bSplineEngine.FunctionApproximationControlPointsAndAuxiliaryKnots(functionDefinition);

            return InteractiveBSpline(functionDefinition.Degree, controlPointsAndAuxKnots.Item2, controlPointsAndAuxKnots.Item1, true);
            
        }

        public GlobalBSpline InteractiveGlobalBSpline(int degree, double[] knots, double[] referencePoints,double leftDerivation, double rightDerivation)
        {
            return InteractiveGlobalBSpline(degree, knots, referencePoints,leftDerivation,rightDerivation, false);
        }
        public GlobalBSpline InteractiveGlobalBSpline(int degree, double[] knots, double[] referencePoints,double leftDerivation, double rightDerivation, bool containsServiceKnots)
        {
            var controlPointsAndAuxKnots =
                _globalBSplineEngine.GlobalControlPointsAndAuxiliaryKnots(degree, knots, referencePoints,leftDerivation,rightDerivation, containsServiceKnots);
            var spline = InteractiveGlobalBSpline(degree, controlPointsAndAuxKnots.Item2, controlPointsAndAuxKnots.Item1, referencePoints, true);
                       
            spline.LeftDerivation = leftDerivation;
            spline.RightDerivation = rightDerivation;

            Ellipse leftDerEll, rightDerEll;
            double pax, pay;
            if (containsServiceKnots)
            {
                var auxKnot = knots[degree] - DERIVATIVE_AUX_KNOT_DISTANCE;
                var auxReferencePoint = MathOperations.LinearFunction(auxKnot, knots[degree], referencePoints.First(), leftDerivation);

                pax = TransformCoordinates.WorldAreaToPlotAreaX(auxKnot, _plotArea, _worldArea);
                pay = TransformCoordinates.WorldAreaToPlotAreaY(auxReferencePoint, _plotArea, _worldArea);
                leftDerEll = _auxiliaryEngine.DrawEllipse(pax - W_DIV_2, pay - H_DIV_2, AUX_KNOT_COLOR3);

                auxKnot = knots[degree + referencePoints.Length - 1] + DERIVATIVE_AUX_KNOT_DISTANCE;
                auxReferencePoint = MathOperations.LinearFunction(auxKnot, knots[degree + referencePoints.Length-1], referencePoints.Last(), rightDerivation);

                pax = TransformCoordinates.WorldAreaToPlotAreaX(auxKnot, _plotArea, _worldArea);
                pay = TransformCoordinates.WorldAreaToPlotAreaY(auxReferencePoint, _plotArea, _worldArea);
                rightDerEll = _auxiliaryEngine.DrawEllipse(pax - W_DIV_2, pay - H_DIV_2, AUX_KNOT_COLOR3);
            }
            else
            {
                var auxKnot = knots.First() - DERIVATIVE_AUX_KNOT_DISTANCE;
                var auxReferencePoint = MathOperations.LinearFunction(auxKnot, knots.First(), referencePoints.First(), leftDerivation);

                pax = TransformCoordinates.WorldAreaToPlotAreaX(auxKnot, _plotArea, _worldArea);
                pay = TransformCoordinates.WorldAreaToPlotAreaY(auxReferencePoint, _plotArea, _worldArea);
                leftDerEll = _auxiliaryEngine.DrawEllipse(pax - W_DIV_2, pay - H_DIV_2, AUX_KNOT_COLOR3);

                auxKnot = knots.Last() + DERIVATIVE_AUX_KNOT_DISTANCE;
                auxReferencePoint = MathOperations.LinearFunction(auxKnot, knots.Last(), referencePoints.Last(), rightDerivation);

                pax = TransformCoordinates.WorldAreaToPlotAreaX(auxKnot, _plotArea, _worldArea);
                pay = TransformCoordinates.WorldAreaToPlotAreaY(auxReferencePoint, _plotArea, _worldArea);
                rightDerEll = _auxiliaryEngine.DrawEllipse(pax - W_DIV_2, pay - H_DIV_2, AUX_KNOT_COLOR3);
            }
            spline.LeftDerivationEllipse = leftDerEll;
            spline.RightDerivationEllipse = rightDerEll;
            return spline;
        }


        public GlobalBSpline FunctionInteractiveGlobalBSpline(int degree, double[] knots, Function function)
        {
            FunctionDefinition functionDefinition;
            functionDefinition = _globalBSplineEngine.CalculateFunctionDefinition(degree, knots, function);
            var controlPointsAndAuxKnots = _globalBSplineEngine.GlobalControlPointsAndAuxiliaryKnots(functionDefinition);
            return InteractiveGlobalBSpline(degree, controlPointsAndAuxKnots.Item2, controlPointsAndAuxKnots.Item1, functionDefinition.YCoordinates , true);
            //return InteractiveBSpline(degree, controlPointsAndAuxKnots.Item2, controlPointsAndAuxKnots.Item1, true);
        }

        public ClampedSpline InteractiveClampedSpline(double[] knots, double[] controlPoints, double leftDerivation, double rightDerivation){
            var ellipses = new List<Ellipse>(knots.Length);
            Ellipse leftDerEll, rightDerEll;
            
            double pax, pay;
            
            var auxKnot = knots.First() - DERIVATIVE_AUX_KNOT_DISTANCE;
            var auxControlPoint = MathOperations.LinearFunction(auxKnot,knots.First(),controlPoints.First(),leftDerivation);
            
            pax = TransformCoordinates.WorldAreaToPlotAreaX(auxKnot, _plotArea, _worldArea);
            pay = TransformCoordinates.WorldAreaToPlotAreaY(auxControlPoint, _plotArea, _worldArea);
            leftDerEll = _auxiliaryEngine.DrawEllipse(pax - W_DIV_2, pay - H_DIV_2, AUX_KNOT_COLOR2);

            auxKnot = knots.Last() + DERIVATIVE_AUX_KNOT_DISTANCE;
            auxControlPoint = MathOperations.LinearFunction(auxKnot, knots.Last(), controlPoints.Last(), rightDerivation);

            pax = TransformCoordinates.WorldAreaToPlotAreaX(auxKnot, _plotArea, _worldArea);
            pay = TransformCoordinates.WorldAreaToPlotAreaY(auxControlPoint, _plotArea, _worldArea);
            rightDerEll = _auxiliaryEngine.DrawEllipse(pax - W_DIV_2, pay - H_DIV_2, AUX_KNOT_COLOR2);

            for (int i = 0; i < knots.Length; i++)
            {
                pax = TransformCoordinates.WorldAreaToPlotAreaX(knots[i], _plotArea, _worldArea);
                pay = TransformCoordinates.WorldAreaToPlotAreaY(controlPoints[i], _plotArea, _worldArea);
                ellipses.Add(_auxiliaryEngine.DrawEllipse(pax - W_DIV_2, pay - H_DIV_2, KNOT_COLOR2));
            }

            // List<Line> linesOfSpline = HermiteSplineLines(knots,controlPoints);
            ClampedSpline spline = ClampedSpline(knots, controlPoints, leftDerivation, rightDerivation);
            spline.DragEllipses = ellipses;
            spline.LeftDerivationEllipse = leftDerEll;
            spline.RightDerivationEllipse = rightDerEll;
            return spline;

        }

        public ClampedSpline InteractiveClampedSpline(double leftKnot, double rightKnot, double[] controlPoints, double leftDerivation, double rightDerivation)
        {
            var length = controlPoints.Length;
            var knots = ArrayMyUtils.GrowingArray(length,leftKnot,rightKnot);


            return InteractiveClampedSpline(knots, controlPoints, leftDerivation, rightDerivation);
        }

        public ClampedSpline ClampedSpline(double[] knots, double[] controlPoints, double leftDerivation, double rightDerivation)
        {
            var derivations = _clampedSplineEngine.ClampedDerivations(controlPoints, leftDerivation, rightDerivation,knots[1]-knots[0]);
            var linesOfSpline = ClampedSplineLines(knots, controlPoints, derivations);
           
            return new ClampedSpline(knots.ToList(), controlPoints.ToList(), derivations.ToList(), linesOfSpline);
        }

        internal Tuple<List<double>,List<Line>> ClampedSplineDerivationsAndLines(double[] knots, double[] controlPoints, double leftDerivation, double rightDerivation)
        {
            var derivations = _clampedSplineEngine.ClampedDerivations(controlPoints, leftDerivation, rightDerivation, knots[1] - knots[0]);
            var lines =  ClampedSplineLines(knots, controlPoints, derivations);
            return Tuple.Create(derivations.ToList(),lines);
        }

        public List<Line> ClampedSplineLines(double[] knots, double[] controlPoints, double[] derivations)
        {
            var lines = new List<Line>();
            double y = 0;
            var previous = new Point();
            var current = new Point();

            SetDefaultSplineColor_2();
            for (int i = 0; i < knots.Length - 1; i++)
            {
                bool canDraw = false;
                double x = knots[i];
                while (x <= knots[i + 1])
                {
                    if (x >= knots[i + 1] - DrawPrecision)
                        x = knots[i + 1];
  
                    y = _clampedSplineEngine.HermiteSplineFunctionValue(x, knots[i], controlPoints[i], knots[i + 1], controlPoints[i + 1], derivations[i], derivations[i + 1]);
                    current = TransformCoordinates.WorldAreaToPlotArea(x, y, _plotArea, _worldArea);
                    if (canDraw)
                    {
                        lines.Add(_auxiliaryEngine.DrawLine(previous, current, SPLINE_COLOR_2));
                    }
                    canDraw = true;
                    previous = current;
                    x += DrawPrecision;


                }
                SwitchSplineColor_2();
            }
            return lines;
        }

        internal GlobalBSpline InteractiveGlobalBSpline(int degree, double[] knots, double[] controlPoints, double[] referencePoints, bool containsServiceKnots)
        {
            var ellipses = new List<Ellipse>();
            var cpEllipses = new List<Ellipse>();
            var serviceKnots = containsServiceKnots ? knots : SilverlightApplication3.BSpline.ServiceKnots(knots, degree, true);

            double PAReferencePoint;
            double PAKnot;

         
            for (int i = 0; i < referencePoints.Length; i++)
            {

                PAKnot = TransformCoordinates.WorldAreaToPlotAreaX(serviceKnots[degree+i], _plotArea, _worldArea);
                PAReferencePoint = TransformCoordinates.WorldAreaToPlotAreaY(referencePoints[i], _plotArea, _worldArea);
                ellipses.Add(_auxiliaryEngine.DrawEllipse(PAKnot - W_DIV_2, PAReferencePoint - H_DIV_2, KNOT_COLOR2));

            }
            double PAControlPoint;


            for (int i = 0; i < controlPoints.Length; i++)
            {

                PAKnot = TransformCoordinates.WorldAreaToPlotAreaX(serviceKnots[i + 1], _plotArea, _worldArea);
                PAControlPoint = TransformCoordinates.WorldAreaToPlotAreaY(controlPoints[i], _plotArea, _worldArea);

                if (i < degree - 1)
                {
                    ellipses.Add(_auxiliaryEngine.DrawEllipse(PAKnot - W_DIV_2, PAControlPoint - H_DIV_2, AUX_KNOT_COLOR3));

                }
                else
                {
                    ellipses.Add(_auxiliaryEngine.DrawEllipse(PAKnot - W_DIV_2, PAControlPoint - H_DIV_2, KNOT_COLOR3));
                }

            }

            var gbSpline = GlobalBSpline(degree, serviceKnots, controlPoints, true);
            gbSpline.DragEllipses = ellipses;
            gbSpline.ControlPointsEllipses = cpEllipses;
           // gbSpline.isInteractive = true;
            gbSpline.FunctionValues = referencePoints.ToList();
            return gbSpline;
                

        }

        public GlobalBSpline GlobalBSpline(int degree, double[] knots, double[] controlPoints, bool containsServiceKnots)
        {
            var lines = new List<Line>();

            int controlPointsCount = controlPoints.Length;


            bool isDegreeEven = degree % 2 == 0;

            int k = controlPointsCount - degree;
            //int numberOfEquations = knots.Length - 1;
            double[] auxKnots = containsServiceKnots ? knots : SilverlightApplication3.BSpline.ServiceKnots(knots, degree, true);
            var intervalIndexesInLines = new List<int>(knots.Length);
            double x = 0;
            double y = 0;
            Point previous = new Point();
            Point current = new Point();
            SetDefaultSplineColor_2();
            for (int i = 0; i < k; i++)
            {

                //  double[] usedControlPoints = new double[degree + 1];
                x = auxKnots[i + degree];
                //Array.Copy(controlPoints, i, usedControlPoints, 0, degree + 1);
                intervalIndexesInLines.Add(Math.Max(lines.Count - 1, 0));
                bool canDraw = false;
                while (x <= auxKnots[i + degree + 1])
                {

                    y = 0;
                    for (int j = 0; j <= degree; j++)
                    {
                        double[] momentarilyUsedKnots = new double[degree + 2];
                        Array.Copy(auxKnots, j + i, momentarilyUsedKnots, 0, degree + 2);
                        if (x >= auxKnots[i + degree + 1] - DrawPrecision) x = auxKnots[i + degree + 1];
                        //y += usedControlPoints[j] * EngineUtils.Bell(momentarilyUsedKnots, x, degree, isDegreeEven, j + 1);
                        y += controlPoints[j + i] * _bSplineEngine.Bell(x, degree, momentarilyUsedKnots, isDegreeEven, j + 1);
                    }

                    //current = isDegreeEven ? TransformCoordinates.WorldAreaToPlotArea(x, y, PA, WA) : TransformCoordinates.WorldAreaToPlotArea(x, -y, PA, WA);
                    current = TransformCoordinates.WorldAreaToPlotArea(x, -y, _plotArea, _worldArea);
                    if (canDraw)
                    {
                        lines.Add(_auxiliaryEngine.DrawLine(previous, current, SPLINE_COLOR_2));
                    }
                    canDraw = true;
                    //else { 
                    // _auxiliaryEngine.DrawEllipse(current.knots-W_DIV_2,current.controlPoints-H_DIV_2,_SplineColor);
                    //}

                    previous = current;
                    x += DrawPrecision;

                }
                SwitchSplineColor_2();
            }
            intervalIndexesInLines.Add(lines.Count - 1);
            for (int i = 0; i < intervalIndexesInLines.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine(intervalIndexesInLines[i].ToString());
            }
            return new GlobalBSpline(degree, controlPoints.ToList(), knots.ToList(), lines);
        }

        public void BSplineBasisFunction(int degree,double[] knots)
        {

            //int degree = knots.Length - 2;
            bool isDegreeEven = degree % 2 == 0;

            double[] Y = new double[knots.Length - 1];
            double[] auxKnots = SilverlightApplication3.BSpline.ServiceKnots(knots, degree, false);

            List<Point> previous = new List<Point>();
            List<Point> current = new List<Point>();

            SolidColorBrush color = new SolidColorBrush();
            color.Color = Color.FromArgb(200, 0, 0, 0);

            for (int i = 0; i < knots.Length - 1; i++)
            {
                previous.Add(new Point());
                current.Add(new Point());
            }
            double x = 0;

            double[] usedKnots = new double[degree + 2];
            do
            {
                for (int i = 0; i < Y.Length; i++)
                {

                    Array.Copy(auxKnots, i, usedKnots, 0, degree + 2);
                    // Array.Reverse(used_knots);
                    Y[i] = _bSplineEngine.Bell(x, degree, usedKnots, isDegreeEven, i + 1);
                    current[i] = isDegreeEven ? TransformCoordinates.WorldAreaToPlotArea(x, Y[i], _plotArea, _worldArea) : TransformCoordinates.WorldAreaToPlotArea(x, -Y[i], _plotArea, _worldArea);
                    //  current[i] = TransformCoordinates.world_to_plot_area(x, -controlPoints[i], _pa, _wa); //calculate_bell returns correct value (99.999% sure), but if I remove '-' 

                    //   current[i] = TransformCoordinates.world_to_plot_area(x, controlPoints[i], _pa, _wa);

                    //basis will be upside down (no idea why) :)
                    if (x != 0)
                    {

                        _auxiliaryEngine.DrawLine(previous[i], current[i], color);
                    }

                    previous[i] = current[i];
                }

                x += DrawPrecision;
            } while (x <= 1);
        }

        public HermiteSpline InteractiveHermiteSpline(double[] knots, double[] controlPoints, double[] derivations)
        {
            var ellipses = new List<Ellipse>(knots.Length);
            var derivationEllipses = new List<Ellipse>(derivations.Length);

            double pax, pay;
            double auxKnot, auxFunctionValue;
            for (int i = 0; i < knots.Length; i++)
            {
                pax = TransformCoordinates.WorldAreaToPlotAreaX(knots[i], _plotArea, _worldArea);
                pay = TransformCoordinates.WorldAreaToPlotAreaY(controlPoints[i], _plotArea, _worldArea);
                ellipses.Add(_auxiliaryEngine.DrawEllipse(pax - W_DIV_2, pay - H_DIV_2, KNOT_COLOR2));
                auxKnot = knots[i] - DERIVATIVE_AUX_KNOT_DISTANCE;
                auxFunctionValue = MathOperations.LinearFunction(auxKnot, knots[i], controlPoints[i], derivations[i]);
                pax = TransformCoordinates.WorldAreaToPlotAreaX(auxKnot, _plotArea, _worldArea);
                pay = TransformCoordinates.WorldAreaToPlotAreaY(auxFunctionValue, _plotArea, _worldArea);
                derivationEllipses.Add(_auxiliaryEngine.DrawEllipse(pax-W_DIV_2,pay-H_DIV_2,AUX_KNOT_COLOR2));

            }
            // List<Line> linesOfSpline = HermiteSplineLines(knots,controlPoints);
            HermiteSpline spline = HermiteSpline(knots, controlPoints, derivations);
            spline.DragEllipses = ellipses;
            spline.DerivationEllipses = derivationEllipses;
            return spline;
        }

        public HermiteSpline HermiteSpline(double[] knots, double[] controlPoints, double[] derivations)
        {
            //if (knots.Length != controlPoints.Length) throw new IncorectSplineInputExcetion("Number of knots cooridnates must be the same as number of controlPoints coordinates")

            var lines = HermiteSplineLines(knots, controlPoints, derivations);
            return new HermiteSpline(knots.ToList(), controlPoints.ToList(),derivations.ToList(), lines);
        }

       

        public List<Line> HermiteSplineLines(double[] knots, double[] controlPoints, double[] derivations)
        {
            //if (knots.Length != controlPoints.Length) throw new IncorectSplineInputExcetion("Number of knots cooridnates must be the same as number of controlPoints coordinates")

            var lines = new List<Line>();
            double y = 0;
            var previous = new Point();
            var current = new Point();

            SetDefaultSplineColor_1();
            for (int i = 0; i < knots.Length - 1; i++)
            {
                bool canDraw = false;
                double x = knots[i];
                while (x <= knots[i + 1])
                {
                    if (x >= knots[i + 1] - DrawPrecision) x = knots[i + 1];
                    y = _hermiteSplineEngine.HermiteSplineFunctionValue(x, knots[i], controlPoints[i], knots[i + 1], controlPoints[i + 1], derivations[i], derivations[i+1]);
                    current = TransformCoordinates.WorldAreaToPlotArea(x, y, _plotArea, _worldArea);
                    if (canDraw)
                    {
                        lines.Add(_auxiliaryEngine.DrawLine(previous, current, SPLINE_COLOR_1));
                    }
                    canDraw = true;
                    previous = current;
                    x += DrawPrecision;


                }
                SwitchSplineColor_1();
            }
            return lines;
        }


        internal List<Line> GlobalBSplineLines(int degree, double[] knots, double[] referencePoints,double leftDerivation, double rightDerivation, bool containsServiceKnots)
        {

            var controlPointsAndAuxKnots = _globalBSplineEngine.GlobalControlPointsAndAuxiliaryKnots(degree, knots, referencePoints,leftDerivation,rightDerivation, containsServiceKnots);
            return BSplineLines(degree,controlPointsAndAuxKnots.Item2,controlPointsAndAuxKnots.Item1, containsServiceKnots);           
        }

        public void SetDefaultSplineColor_1()
        {
            SPLINE_COLOR_1 = SPLINE_COLOR_1_A;
        }
        public void SetDefaultSplineColor_2()
        {
            SPLINE_COLOR_2 = SPLINE_COLOR_2_A;
        }



        
    }
}
