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
using System.Collections.Generic;

namespace SilverlightApplication3
{
    public class CanvasUtilities
    {
        private PlotArea _plotArea;
        private WorldArea _worldArea;

        public CanvasUtilities(PlotArea plotArea, WorldArea worldArea)
        {
            this._plotArea = plotArea;
            this._worldArea = worldArea;
        }

        public void RefreshBSplineInCanvas(BSpline changedSpline, Canvas canvas, SplineDrawer engine)
        {
            RemoveInCanvas(canvas, changedSpline.LinesOfSpline);
            List<Line> lines = engine.BSplineLines(changedSpline.Degree, changedSpline.Knots.ToArray(), changedSpline.ControlPoints.ToArray(), true);
            changedSpline.LinesOfSpline = lines;
        }

        //public void OptimizedRefreshBSplineInCanvas(BSpline changedSpline, int changedCPIndex, Canvas canvas, SplineDrawer engine, bool isUniform)
        //{
        //    // not working correctly right now
        //    var degree = changedSpline.Degree;
        //    var fromIdx = isUniform ? changedCPIndex - degree : changedCPIndex - degree - 3;
        //    fromIdx = fromIdx - 1 > 0 ? fromIdx : 0;
        //    var from = changedSpline.IntervalLineIndexes[fromIdx];
        //    var toIdx = changedCPIndex + 1;
        //    // toIdx = toIdx < changedSpline.IntervalLineIndexes.Count - 1 ? toIdx : changedSpline.IntervalLineIndexes.Count - 1;
        //    //var to = changedSpline.IntervalLineIndexes[toIdx];
        //    int to;
        //    if (toIdx <= changedSpline.IntervalLineIndexes.Count - 1)
        //    {
        //        to = changedSpline.IntervalLineIndexes[toIdx];
        //    }
        //    else
        //    {
        //        toIdx = changedSpline.IntervalLineIndexes.Count - 1;
        //        to = changedSpline.LinesOfSpline.Count - 1;
        //    }
        //    var linesCount = changedSpline.LinesOfSpline.Count;


        //    var redrawedLines = engine.PartialBSplineLines(changedSpline.Degree, changedSpline.Knots.ToArray(), changedSpline.ControlPoints.ToArray(), fromIdx, toIdx);
        //    //var howManyLinesAdded = redrawedLines.Count - linesCount;
        //    //if (howManyLinesAdded > 0)
        //    //{
        //    //    to += howManyLinesAdded;

        //    //}
        //    RemoveInCanvas(canvas, changedSpline.LinesOfSpline, from, to);
        //    ArrayMyUtils.ReplaceInList(redrawedLines, changedSpline.LinesOfSpline, from);
        //    //System.Diagnostics.Debug.WriteLine(howManyLinesAdded.ToString());
        //    //EngineUtils.ReplaceInList(linesAndIntervalIndexes.Item2, changedSpline.IntervalLineIndexes, fromIdx);

        //}

        private void RemoveInCanvas(Canvas canvas, List<Line> lines, int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                canvas.Children.Remove(lines[i]);
            }
        }

        public void RefreshHermiteSplineInCanvas(HermiteSpline changedSpline, Canvas canvas, SplineDrawer engine)
        {
            //treba zmazat vsetky liny meneneho splinu
            RemoveInCanvas(canvas, changedSpline.LinesOfSpline);

            List<Line> lines = engine.HermiteSplineLines(changedSpline.Knots.ToArray(), changedSpline.ControlPoints.ToArray(), changedSpline.Derivations.ToArray());
            changedSpline.LinesOfSpline = lines;
        }

        public void TotalRefreshHermiteSplineInCanvas(HermiteSpline changedSpline, Canvas canvas, SplineDrawer engine)
        {
           
            RemoveInCanvas(canvas, changedSpline.LinesOfSpline);
            RemoveInCanvas(canvas, changedSpline.DerivationEllipses);
            RemoveInCanvas(canvas, changedSpline.DragEllipses);

            var spline = engine.InteractiveHermiteSpline(changedSpline.Knots.ToArray(),changedSpline.ControlPoints.ToArray(),changedSpline.Derivations.ToArray());
            //changedSpline.LinesOfSpline = spline.LinesOfSpline;
            //changedSpline.DerivationEllipses = spline.DerivationEllipses;
            //changedSpline.DragEllipses = spline.DragEllipses;
            //List<Line> lines = engine.HermiteSplineLines(changedSpline.Knots.ToArray(), changedSpline.ControlPoints.ToArray(), changedSpline.Derivations.ToArray());
            //changedSpline.LinesOfSpline = lines;
        }

        public void RefreshClampedSplineInCanvas(ClampedSpline changedSpline, Canvas canvas, SplineDrawer engine)
        {
            //treba zmazat vsetky liny meneneho splinu
            RemoveInCanvas(canvas, changedSpline.LinesOfSpline);

            var dersAndLines = engine.ClampedSplineDerivationsAndLines(changedSpline.Knots.ToArray(), changedSpline.ControlPoints.ToArray(), changedSpline.Derivations[0], changedSpline.Derivations[changedSpline.Derivations.Count-1]);
            changedSpline.Derivations = dersAndLines.Item1;
            changedSpline.LinesOfSpline = dersAndLines.Item2;
            //Canvas.SetLeft(changedSpline.LeftDerivationEllipse,MathOperations.);
           // var leftDerWACoord = 
           //     MathOperations.LinearFunction(changedSpline.Knots[0]-1,changedSpline.Knots[0],changedSpline.ControlPoints[0],changedSpline.Derivations[0]);
           // Canvas.SetTop(changedSpline.LeftDerivationEllipse,TransformCoordinates.WorldAreaToPlotAreaY(leftDerWACoord,_plotArea,_worldArea));

        }

        public void RemoveInCanvas(Canvas canvas, List<Line> lines)
        {

            for (int i = 0; i < lines.Count; i++)
            {
                canvas.Children.Remove(lines[i]);
            }

        }

        public void SetEllipseCoordinates(Canvas canvas,Ellipse ellipse, double left, double top)
        {
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);
        }

        private void RemoveBorderEllipsesInCanvas(Canvas canvas, SolidColorBrush color)
        {

            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i] is Ellipse & (canvas.Children[i] as Ellipse).Fill.Equals(color))
                {
                    canvas.Children.RemoveAt(i);
                    
                }
            }

        }

        public void RemoveInCanvas(Canvas canvas, List<Ellipse> ellipses)
        {

            for (int i = 0; i < ellipses.Count; i++)
            {
                canvas.Children.Remove(ellipses[i]);
            }

        }

        public void RemoveCPFromHermiteSpline(HermiteSpline changedSpline, int changedCPIndex, Canvas canvas, SplineDrawer engine)
        {
            try
            {
                //TREBA PORIESIT DERIVACIE
                RemoveInCanvas(canvas, changedSpline.DragEllipses);
                RemoveInCanvas(canvas, changedSpline.LinesOfSpline);
                changedSpline.Knots.RemoveAt(changedCPIndex);
                changedSpline.ControlPoints.RemoveAt(changedCPIndex);

                changedSpline = engine.InteractiveHermiteSpline(changedSpline.Knots.ToArray(), changedSpline.ControlPoints.ToArray(), changedSpline.Derivations.ToArray());
            }
            catch (ArgumentOutOfRangeException) { }
        }

        public void RefreshGlobalBSplineInCanvas(GlobalBSpline changedSpline, Canvas canvas, SplineDrawer engine)
        {
            RemoveInCanvas(canvas, changedSpline.LinesOfSpline);
           // RemoveInCanvas(canvas, changedSpline.ControlPointsEllipses);
           // RemoveInCanvas(canvas, changedSpline.DragEllipses);
           // canvas.Children.Remove(changedSpline.LeftDerivationEllipse);
            //canvas.Children.Remove(changedSpline.RightDerivationEllipse);
            List<Line> lines = engine.GlobalBSplineLines(changedSpline.Degree, changedSpline.Knots.ToArray(), changedSpline.FunctionValues.ToArray(),changedSpline.LeftDerivation,changedSpline.RightDerivation, true);
            changedSpline.LinesOfSpline = lines;
            //changedSpline = engine.InteractiveGlobalBSpline(changedSpline.Degree,
            //    changedSpline.Knots.ToArray(),
            //    changedSpline.FunctionValues.ToArray(),changedSpline.LeftDerivation, changedSpline.RightDerivation,
            //    true);
        }

        
    }
}
