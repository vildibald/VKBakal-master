using System;
using System.Linq;
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
    public partial class MainPage
    {
        private void resetDragInfoLabel()
        {
            _dragInfoLabel = new DragInfoLabel(CANVAS_TEXT_COLOR);
            _dragInfoLabel.Visibility = Visibility.Collapsed;
            canvas1.Children.Add(_dragInfoLabel);
        }
        public void SwapCanvasColor()
        {
            if (_isCanvasBlack)
            {
                canvas1.Background = WHITE_COLOR;
                CANVAS_TEXT_COLOR = BLACK_COLOR;
                resetDragInfoLabel();
                _isCanvasBlack = false;
            }
            else
            {
                canvas1.Background = BLACK_COLOR;
                CANVAS_TEXT_COLOR = WHITE_COLOR;
                resetDragInfoLabel();
                _isCanvasBlack = true;
            }
        }
        public void OptimalWorldArea()
        {
            var optimalMinX = double.MaxValue;
            var optimalMaxX = double.MinValue;
            var optimalMinY = double.MaxValue;
            var optimalMaxY = double.MinValue;

            for (int i = 0; i < _bSplineList.Count; i++)
            {
                var splineRange = _bSplineList[i].Range_MinX_MaxX_MinY_MaxY(_plotArea, _worldArea);
                if (splineRange.Item1 < optimalMinX)
                    optimalMinX = splineRange.Item1;
                if (splineRange.Item2 > optimalMaxX)
                    optimalMaxX = splineRange.Item2;
                if (splineRange.Item3 < optimalMinY)
                    optimalMinY = splineRange.Item3;
                if (splineRange.Item4 > optimalMaxY)
                    optimalMaxY = splineRange.Item4;

            }

            for (int i = 0; i < _clampedSplineList.Count; i++)
            {
                var splineRange = _clampedSplineList[i].Range_MinX_MaxX_MinY_MaxY(_plotArea, _worldArea);
                if (splineRange.Item1 < optimalMinX)
                    optimalMinX = splineRange.Item1;
                if (splineRange.Item2 > optimalMaxX)
                    optimalMaxX = splineRange.Item2;
                if (splineRange.Item3 < optimalMinY)
                    optimalMinY = splineRange.Item3;
                if (splineRange.Item4 > optimalMaxY)
                    optimalMaxY = splineRange.Item4;

            }



            for (int i = 0; i < _hermiteSplineList.Count; i++)
            {
                var splineRange = _hermiteSplineList[i].Range_MinX_MaxX_MinY_MaxY(_plotArea, _worldArea);
                if (splineRange.Item1 < optimalMinX)
                    optimalMinX = splineRange.Item1;
                if (splineRange.Item2 > optimalMaxX)
                    optimalMaxX = splineRange.Item2;
                if (splineRange.Item3 < optimalMinY)
                    optimalMinY = splineRange.Item3;
                if (splineRange.Item4 > optimalMaxY)
                    optimalMaxY = splineRange.Item4;

            }

            for (int i = 0; i < _globalBSplineList.Count; i++)
            {
                var splineRange = _globalBSplineList[i].Range_MinX_MaxX_MinY_MaxY(_plotArea, _worldArea);
                if (splineRange.Item1 < optimalMinX)
                    optimalMinX = splineRange.Item1;
                if (splineRange.Item2 > optimalMaxX)
                    optimalMaxX = splineRange.Item2;
                if (splineRange.Item3 < optimalMinY)
                    optimalMinY = splineRange.Item3;
                if (splineRange.Item4 > optimalMaxY)
                    optimalMaxY = splineRange.Item4;

            }

            if (optimalMinX == double.MaxValue)
                optimalMinX = _worldArea.XMin;
            if (optimalMaxX == double.MinValue)
                optimalMaxX = _worldArea.XMax;
            if (optimalMinY == double.MaxValue)
                optimalMinY = _worldArea.YMin;
            if (optimalMaxY == double.MinValue)
                optimalMaxY = _worldArea.YMax;



            SetNewWorldArea(Math.Round(optimalMinX - OPTIMAL_SPACE, 2), Math.Round(optimalMaxX + OPTIMAL_SPACE, 2), Math.Round(optimalMinY - OPTIMAL_SPACE, 2), Math.Round(optimalMaxY + OPTIMAL_SPACE, 2));
        }

        public void SetNewWorldArea(double newXMin, double newXMax, double newYMin, double newYMax)
        {
            //if (newXMin < 0.5 || newYMin < 0.5 || newXMax < 0.5 || newYMax < 0.5)
            //{
            //    haha.Text = "Too high zoom";
            //    return;
            //}

            WorldArea oldWA = _worldArea;
            _worldArea = new WorldArea(newXMin, newXMax, newYMin, newYMax);

            _waZeroPointInPACoordinates = TransformCoordinates.WorldAreaToPlotArea(0, 0, _plotArea, _worldArea);
            _engine = new SplineDrawer(canvas1, _plotArea, _worldArea, DrawPrecision);
            x_min_TextBox.Text = newXMin.ToString();
            x_max_TextBox.Text = newXMax.ToString();
            y_min_TextBox.Text = newYMin.ToString();
            y_max_TextBox.Text = newYMax.ToString();

            ResetCanvas();

        }
        public void ResetCanvas()
        {
            canvas1.Children.Clear();

            DefaultCanvas();


            GlobalBSpline globalBSpline;
            for (int i = 0; i < _globalBSplineList.Count; i++)
            {
                globalBSpline = _globalBSplineList[i];

                //_globalBSplineList[i] = globalBSpline.isInteractive ? _engine.InteractiveGlobalBSpline(globalBSpline.Degree,
                //    globalBSpline.Knots.ToArray(), globalBSpline.ControlPoints.ToArray(), globalBSpline.FunctionValues.ToArray(), true)
                //    :
                //    _engine.GlobalBSpline(globalBSpline.Degree,
                //    globalBSpline.Knots.ToArray(), globalBSpline.ControlPoints.ToArray(), true);
                _globalBSplineList[i] = _engine.InteractiveGlobalBSpline(globalBSpline.Degree,
                    globalBSpline.Knots.ToArray(), globalBSpline.FunctionValues.ToArray(), globalBSpline.LeftDerivation, globalBSpline.RightDerivation, true);


            }

            BSpline bSpline;
            for (int i = 0; i < _bSplineList.Count; i++)
            {
                bSpline = _bSplineList[i];
                _bSplineList[i] = _engine.InteractiveBSpline(bSpline.Degree, bSpline.Knots.ToArray(), bSpline.ControlPoints.ToArray(), true);
                //    gbSpline.Knots.ToArray(), gbSpline.ControlPoints.ToArray(), true)
                //_bSplineList[i] = gbSpline.isInteractive ? _engine.InteractiveBSpline(gbSpline.Degree,
                //    gbSpline.Knots.ToArray(), gbSpline.ControlPoints.ToArray(), true)
                   // :
                   // _engine.BSpline(gbSpline.Degree,
                   // gbSpline.Knots.ToArray(), gbSpline.ControlPoints.ToArray(), true);

            }
            HermiteSpline hSpline;
            for (int i = 0; i < _hermiteSplineList.Count; i++)
            {

                hSpline = _hermiteSplineList[i];
                _hermiteSplineList[i] = _engine.InteractiveHermiteSpline(hSpline.Knots.ToArray(), hSpline.ControlPoints.ToArray(), hSpline.Derivations.ToArray());
            }
            ClampedSpline cSpline;
            for (int i = 0; i < _clampedSplineList.Count; i++)
            {

                cSpline = _clampedSplineList[i];
                _clampedSplineList[i] = _engine.InteractiveClampedSpline(cSpline.Knots.ToArray(), cSpline.ControlPoints.ToArray(), cSpline.Derivations[0], cSpline.Derivations[cSpline.Derivations.Count - 1]);
            }
        }

        public void RemoveAllSplines()
        {
            _bSplineList.Clear();
            _hermiteSplineList.Clear();
            _globalBSplineList.Clear();
            _clampedSplineList.Clear();
        }
        private void DefaultCanvas()
        {
            Line horizontal = new Line();
            Line vertical = new Line();
            TextBlock min_X_TextBlock = new TextBlock();
            TextBlock max_X_TextBlock = new TextBlock();
            TextBlock max_Y_TextBlock = new TextBlock();
            TextBlock min_Y_TextBlock = new TextBlock();
            SolidColorBrush color = new SolidColorBrush();
            color.Color = Color.FromArgb(255, 170, 170, 0);

            horizontal.X1 = TransformCoordinates.WorldAreaToPlotAreaX(_worldArea.XMin, _plotArea, _worldArea);
            horizontal.Y1 = _waZeroPointInPACoordinates.Y;
            horizontal.X2 = TransformCoordinates.WorldAreaToPlotAreaX(_worldArea.XMax, _plotArea, _worldArea);
            horizontal.Y2 = _waZeroPointInPACoordinates.Y;

            vertical.X1 = _waZeroPointInPACoordinates.X;
            vertical.Y1 = TransformCoordinates.WorldAreaToPlotAreaY(_worldArea.YMin, _plotArea, _worldArea);
            vertical.X2 = _waZeroPointInPACoordinates.X;
            vertical.Y2 = TransformCoordinates.WorldAreaToPlotAreaY(_worldArea.YMax, _plotArea, _worldArea);


            horizontal.StrokeThickness = 1;
            vertical.StrokeThickness = 1;
            horizontal.Stroke = color;
            vertical.Stroke = color;
            Canvas.SetZIndex(horizontal, 1);

            min_X_TextBlock.Text = _worldArea.XMin.ToString();
            max_X_TextBlock.Text = _worldArea.XMax.ToString();
            max_Y_TextBlock.Text = _worldArea.YMax.ToString();
            min_Y_TextBlock.Text = _worldArea.YMin.ToString();
            min_X_TextBlock.Height = 14;
            max_X_TextBlock.Height = 14;
            max_Y_TextBlock.Height = 14;
            min_Y_TextBlock.Height = 14;
            min_X_TextBlock.Width = 30;
            max_X_TextBlock.Width = 30;
            max_Y_TextBlock.Width = 30;
            min_Y_TextBlock.Width = 30;
            min_X_TextBlock.Foreground = color;
            max_X_TextBlock.Foreground = color;
            max_Y_TextBlock.Foreground = color;
            min_Y_TextBlock.Foreground = color;
            Canvas.SetLeft(min_X_TextBlock, _plotArea.XMin + 10);
            Canvas.SetLeft(max_X_TextBlock, _plotArea.XMax - 30);
            Canvas.SetLeft(max_Y_TextBlock, _plotArea.XMax / 2 + 12);
            Canvas.SetLeft(min_Y_TextBlock, _plotArea.XMax / 2 + 12);
            Canvas.SetTop(min_X_TextBlock, _plotArea.YMax / 2 - 23);
            Canvas.SetTop(max_X_TextBlock, _plotArea.YMax / 2 - 23);
            Canvas.SetTop(max_Y_TextBlock, _plotArea.YMin + 10);
            Canvas.SetTop(min_Y_TextBlock, _plotArea.YMax - 20);
            Canvas.SetZIndex(horizontal, 1);
            Canvas.SetZIndex(horizontal, 1);
            Canvas.SetZIndex(horizontal, 1);
            Canvas.SetZIndex(horizontal, 1);

            _dragInfoLabel = new DragInfoLabel(CANVAS_TEXT_COLOR);
            _dragInfoLabel.Visibility = Visibility.Collapsed;

            canvas1.Children.Add(_dragInfoLabel);
            canvas1.Children.Add(horizontal);
            canvas1.Children.Add(vertical);
            canvas1.Children.Add(min_X_TextBlock);
            canvas1.Children.Add(min_Y_TextBlock);
            canvas1.Children.Add(max_X_TextBlock);
            canvas1.Children.Add(max_Y_TextBlock);
        }

        private void DragBSpline(MouseEventArgs e)
        {
            
            if (!UniformBSpline_CheckBox.IsChecked.Value)
            {

                // _lastClickedPositionX = e.GetPosition(LayoutRoot).X - _point.X;
                // yCursorPosition = e.GetPosition(LayoutRoot).Y - _point.Y;

                var cursorPosition = CursorPosition(e);
                var xCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaX(cursorPosition.X + W_DIV_2,_plotArea,_worldArea);
                var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(cursorPosition.Y + H_DIV_2, _plotArea, _worldArea);

                if (cursorPosition.X <= _leftEllipseX + DEFAULT_REDRAWING_PRECISION)
                {
                    cursorPosition.X = _leftEllipseX + DEFAULT_REDRAWING_PRECISION;
                    // CanvasMyUtils.RemoveCPFromHermiteSpline(_redrawedHermiteSpline,_changedCPPos,canvas1,_engine);
                    return;
                }
               // else if (cursorPosition.X >= _rightEllipseX - DEFAULT_REDRAWING_PRECISION && _rightEllipseX != -1)
                else if (cursorPosition.X >= _rightEllipseX - DEFAULT_REDRAWING_PRECISION)// && _stupidFastHackToResolveBugWithBSplineWhenLastEllipseIsDragged)
                {
                    cursorPosition.X = _rightEllipseX - DEFAULT_REDRAWING_PRECISION;
                    //CanvasMyUtils.RemoveCPFromHermiteSpline(_redrawedHermiteSpline, _changedCPPos, canvas1, _engine);
                    return;
                }

                _selectedEllipse.SetValue(Canvas.LeftProperty, cursorPosition.X);
                _selectedEllipse.SetValue(Canvas.TopProperty, cursorPosition.Y);


                //try
                //{

                   
                    _selectedBSpline.Knots[_changedKnotPos] = xCursorPositionInWA;
                    
                    _selectedBSpline.ControlPoints[_changedPointIndex] = yCursorPositionInWA;
                    //if (_selectedBSpline.Knots[_changedKnotPos] >= _selectedBSpline.Knots[_changedKnotPos + 1])
                    //{
                    //    var swap = _selectedBSpline.Knots[_changedKnotPos + 1];
                    //    _selectedBSpline.Knots[_changedKnotPos + 1] = _selectedBSpline.Knots[_changedKnotPos];
                    //    _selectedBSpline.Knots[_changedKnotPos] = swap;
                    //    var idx= _selectedBSpline.DragEllipses.IndexOf(_selectedEllipse as Ellipse);
                    //    var swapEl = _selectedBSpline.DragEllipses[idx + 1];
                    //    _selectedBSpline.DragEllipses[idx + 1] = _selectedBSpline.DragEllipses[idx];
                    //    _selectedBSpline.DragEllipses[idx] = swapEl;
                    //}

                //}
                //catch (Exception) { }

                    _dragInfoLabel.WriteAndSetCoordinates(_changedPointIndex, xCursorPositionInWA, yCursorPositionInWA, cursorPosition.X + AVOID_CURSOR_X, cursorPosition.Y + H_DIV_2);
                _dragInfoLabel.Visibility = Visibility.Visible;
                //CanvasMyUtils.OptimizedRefreshBSplineInCanvas(_selectedBSpline, _changedCPPos, canvas1, _engine, false);
                _canvasUtilities.RefreshBSplineInCanvas(_selectedBSpline, canvas1, _engine);

            }
            else
            {
                // yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(yCursorPosition + H_DIV_2, _plotArea, _worldArea);

                //yCursorPosition = e.GetPosition(LayoutRoot).Y - _point.Y;
                var yCursorPosition = CursorPositionY(e);
                _selectedEllipse.SetValue(Canvas.TopProperty, yCursorPosition);
                //try
                //{
                    var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(yCursorPosition+ H_DIV_2, _plotArea, _worldArea);
                    _selectedBSpline.ControlPoints[_changedPointIndex] = yCursorPositionInWA;
                    

                //}
                //catch (Exception) { }
                    var selectedKnotPositionInPA = TransformCoordinates.WorldAreaToPlotAreaX(_selectedBSpline.Knots[_changedKnotPos], _plotArea, _worldArea);
                    _dragInfoLabel.WriteAndSetCoordinates(_changedPointIndex, _selectedBSpline.Knots[_changedKnotPos], yCursorPositionInWA, selectedKnotPositionInPA + AVOID_CURSOR_X, yCursorPosition + H_DIV_2);
                _dragInfoLabel.Visibility = Visibility.Visible;
                _canvasUtilities.RefreshBSplineInCanvas(_selectedBSpline, canvas1, _engine);
            }
        }

        private void DragHSpline(MouseEventArgs e)
        {
           
            // yCursorPosition = e.GetPosition(LayoutRoot).Y - _point.Y;
            // _lastClickedPositionX = e.GetPosition(LayoutRoot).X - _point.X;
            var cursorPosition = CursorPosition(e);
            var xCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaX(cursorPosition.X + W_DIV_2, _plotArea, _worldArea);
            var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(cursorPosition.Y + H_DIV_2, _plotArea, _worldArea);

            if (HermiteRegularCheckBox.IsChecked.Value)
            {
                if (cursorPosition.X <= _leftEllipseX + DEFAULT_REDRAWING_PRECISION)
                {
                    cursorPosition.X = _leftEllipseX + DEFAULT_REDRAWING_PRECISION;
                    // CanvasMyUtils.RemoveCPFromHermiteSpline(_redrawedHermiteSpline,_changedCPPos,canvas1,_engine);
                    return;
                }
                else if (cursorPosition.X >= _rightEllipseX - DEFAULT_REDRAWING_PRECISION && _rightEllipseX != -1)
                {
                    cursorPosition.X = _rightEllipseX - DEFAULT_REDRAWING_PRECISION;
                    //CanvasMyUtils.RemoveCPFromHermiteSpline(_redrawedHermiteSpline, _changedCPPos, canvas1, _engine);
                    return;
                }
            }
            _selectedEllipse.SetValue(Canvas.TopProperty, cursorPosition.Y);
            _selectedEllipse.SetValue(Canvas.LeftProperty, cursorPosition.X);

           // try
            //{
              
                _selectedHermiteSpline.ControlPoints[_changedPointIndex] = yCursorPositionInWA;
              
                _selectedHermiteSpline.Knots[_changedPointIndex] = xCursorPositionInWA;

            //}
            //catch (Exception) { }
            // _selectedEllipse.SetValue(Canvas.TopProperty, e.GetPosition(LayoutRoot).controlPoints - _point.controlPoints);
            _dragInfoLabel.WriteAndSetCoordinates(_changedPointIndex, xCursorPositionInWA, yCursorPositionInWA, cursorPosition.X + AVOID_CURSOR_X, cursorPosition.Y + H_DIV_2);
            _dragInfoLabel.Visibility = Visibility.Visible;
            _canvasUtilities.RefreshHermiteSplineInCanvas(_selectedHermiteSpline, canvas1, _engine);
            //_canvasUtilities.TotalRefreshHermiteSplineInCanvas(_selectedHermiteSpline, canvas1, _engine);
            // }
        }

        private void DragHSplineDerivation(MouseEventArgs e)
        {
            var yCursorPosition = CursorPositionY(e);
            _selectedEllipse.SetValue(Canvas.TopProperty, yCursorPosition);
            var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(yCursorPosition, _plotArea, _worldArea);
            var xCursorPositionInWA = _selectedHermiteSpline.Knots[_changedPointIndex] - SplineDrawer.DERIVATIVE_AUX_KNOT_DISTANCE;
            _selectedHermiteSpline.Derivations[_changedPointIndex] = 
                MathOperations.DirectionOfLinearFunction(xCursorPositionInWA,yCursorPositionInWA,_selectedHermiteSpline.Knots[_changedPointIndex],_selectedHermiteSpline.ControlPoints[_changedPointIndex]);

            var selectedKnotPositionInPA = TransformCoordinates.WorldAreaToPlotAreaX(_selectedHermiteSpline.Knots[_changedPointIndex], _plotArea, _worldArea);
            _dragInfoLabel.WriteAndSetCoordinates(_changedPointIndex, _selectedHermiteSpline.Derivations[_changedPointIndex], xCursorPositionInWA, yCursorPositionInWA, selectedKnotPositionInPA + AVOID_CURSOR_X, yCursorPosition + H_DIV_2);
            _dragInfoLabel.Visibility = Visibility.Visible;

            _canvasUtilities.RefreshHermiteSplineInCanvas(_selectedHermiteSpline, canvas1, _engine);
        }

        private void DragGBSpline(MouseEventArgs e)
        {
         
            var yCursorPosition= CursorPositionY(e);
            _selectedEllipse.SetValue(Canvas.TopProperty, yCursorPosition);
            //try
            //{
                var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(yCursorPosition + H_DIV_2, _plotArea, _worldArea);
                _selectedGlobalBSpline.FunctionValues[_changedPointIndex] = yCursorPositionInWA;

            //}
            //catch (Exception) { }
            // _selectedEllipse.SetValue(Canvas.TopProperty, e.GetPosition(LayoutRoot).controlPoints - _point.controlPoints);
                var selectedKnotPositionInPA = TransformCoordinates.WorldAreaToPlotAreaX(_selectedGlobalBSpline.Knots[_changedPointIndex], _plotArea, _worldArea);
            _dragInfoLabel.WriteAndSetCoordinates(_changedPointIndex, _selectedGlobalBSpline.Knots[_changedPointIndex] + _selectedGlobalBSpline.Degree, yCursorPositionInWA, selectedKnotPositionInPA + AVOID_CURSOR_X, yCursorPosition + H_DIV_2);
            _dragInfoLabel.Visibility = Visibility.Visible;
            _canvasUtilities.RefreshGlobalBSplineInCanvas(_selectedGlobalBSpline, canvas1, _engine);
            // }
        }

        private void DragGBSplineLeftDerivation(MouseEventArgs e)
        {
            var yCursorPosition = CursorPositionY(e);

            _selectedEllipse.SetValue(Canvas.TopProperty, yCursorPosition);

            var degree = _selectedGlobalBSpline.Degree;
            var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(yCursorPosition + H_DIV_2, _plotArea, _worldArea);
            var xCursorPositionInWA = _selectedGlobalBSpline.Knots[degree] - SplineDrawer.DERIVATIVE_AUX_KNOT_DISTANCE;
            var xCursorPosition = TransformCoordinates.WorldAreaToPlotAreaX(xCursorPositionInWA, _plotArea, _worldArea);
            _selectedGlobalBSpline.LeftDerivation = MathOperations.DirectionOfLinearFunction(xCursorPositionInWA, yCursorPositionInWA, _selectedGlobalBSpline.Knots[degree], _selectedGlobalBSpline.FunctionValues[0]);
            _dragInfoLabel.WriteAndSetCoordinates(_selectedGlobalBSpline.LeftDerivation, xCursorPositionInWA, yCursorPositionInWA, xCursorPosition + AVOID_CURSOR_X, yCursorPosition + H_DIV_2);
            _dragInfoLabel.Visibility = Visibility.Visible;

            _canvasUtilities.RefreshGlobalBSplineInCanvas(_selectedGlobalBSpline, canvas1, _engine);
        }

        private void DragGBSplineRightDerivation(MouseEventArgs e)
        {
            var yCursorPosition = CursorPositionY(e);

             _selectedEllipse.SetValue(Canvas.TopProperty, yCursorPosition);

            var degree = _selectedGlobalBSpline.Degree;
            var lastKnotIdx=_selectedGlobalBSpline.Knots.Count-degree-1;
            var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(yCursorPosition + H_DIV_2, _plotArea, _worldArea);
            var xCursorPositionInWA = _selectedGlobalBSpline.Knots[lastKnotIdx] + SplineDrawer.DERIVATIVE_AUX_KNOT_DISTANCE;
            var xCursorPosition = TransformCoordinates.WorldAreaToPlotAreaX(xCursorPositionInWA, _plotArea, _worldArea);
            _selectedGlobalBSpline.RightDerivation = MathOperations.DirectionOfLinearFunction( _selectedGlobalBSpline.Knots[lastKnotIdx], _selectedGlobalBSpline.FunctionValues.Last(),xCursorPositionInWA, yCursorPositionInWA);
            _dragInfoLabel.WriteAndSetCoordinates(_selectedGlobalBSpline.RightDerivation, xCursorPositionInWA, yCursorPositionInWA,xCursorPosition + AVOID_CURSOR_X, yCursorPosition + H_DIV_2);
            _dragInfoLabel.Visibility = Visibility.Visible;

            _canvasUtilities.RefreshGlobalBSplineInCanvas(_selectedGlobalBSpline, canvas1, _engine);
        }
        private void DragCSpline(MouseEventArgs e)
        {
            var cursorPositionY = CursorPositionY(e);


            _selectedEllipse.SetValue(Canvas.TopProperty, cursorPositionY);
            //_selectedEllipse.SetValue(Canvas.LeftProperty, _lastClickedPositionX);

            var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(cursorPositionY + H_DIV_2, _plotArea, _worldArea);
            _selectedClampedSpline.ControlPoints[_changedPointIndex] = yCursorPositionInWA;
            var xCursorPositionInWA = _selectedClampedSpline.Knots[_changedPointIndex];
            var xCursorPosition = TransformCoordinates.WorldAreaToPlotAreaX(xCursorPositionInWA, _plotArea, _worldArea);
            _dragInfoLabel.WriteAndSetCoordinates(_changedPointIndex, _selectedClampedSpline.Knots[_changedPointIndex], yCursorPositionInWA, xCursorPosition + AVOID_CURSOR_X, cursorPositionY + H_DIV_2);
            _dragInfoLabel.Visibility = Visibility.Visible;

            _canvasUtilities.RefreshClampedSplineInCanvas(_selectedClampedSpline, canvas1, _engine);
            // }
        }

        private void DragCSplineLeftDerivation(MouseEventArgs e)
        {
            var cursorPositionY =  CursorPositionY(e);

            _selectedEllipse.SetValue(Canvas.TopProperty, cursorPositionY);


            var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(cursorPositionY + H_DIV_2, _plotArea, _worldArea);
            var xCursorPositionInWA = _selectedClampedSpline.Knots[0] - SplineDrawer.DERIVATIVE_AUX_KNOT_DISTANCE;
            var xCursorPosition = TransformCoordinates.WorldAreaToPlotAreaX(xCursorPositionInWA, _plotArea, _worldArea);
            _selectedClampedSpline.Derivations[0] = MathOperations.DirectionOfLinearFunction(xCursorPositionInWA, yCursorPositionInWA, _selectedClampedSpline.Knots[0], _selectedClampedSpline.ControlPoints[0]);
            _dragInfoLabel.WriteAndSetCoordinates(_selectedClampedSpline.Derivations[0], xCursorPositionInWA, yCursorPositionInWA, xCursorPosition + AVOID_CURSOR_X, cursorPositionY + H_DIV_2);
            _dragInfoLabel.Visibility = Visibility.Visible;

            _canvasUtilities.RefreshClampedSplineInCanvas(_selectedClampedSpline, canvas1, _engine);
            // }
        }

        private void DragCSplineRightDerivation(MouseEventArgs e)
        {
            var cursorPositionY = CursorPositionY(e);

            _selectedEllipse.SetValue(Canvas.TopProperty, cursorPositionY);
            var yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(cursorPositionY + H_DIV_2, _plotArea, _worldArea);
            var xCursorPositionInWA = _selectedClampedSpline.Knots.Last() + SplineDrawer.DERIVATIVE_AUX_KNOT_DISTANCE;
            _selectedClampedSpline.Derivations[_selectedClampedSpline.Derivations.Count - 1] = MathOperations.DirectionOfLinearFunction(_selectedClampedSpline.Knots.Last(), _selectedClampedSpline.ControlPoints.Last(), xCursorPositionInWA, yCursorPositionInWA);
            var xCursorPosition = TransformCoordinates.WorldAreaToPlotAreaX(xCursorPositionInWA, _plotArea, _worldArea);
            _dragInfoLabel.WriteAndSetCoordinates(_selectedClampedSpline.Derivations[_selectedClampedSpline.Derivations.Count - 1], xCursorPositionInWA, yCursorPositionInWA, xCursorPosition + AVOID_CURSOR_X, cursorPositionY + H_DIV_2);
            _dragInfoLabel.Visibility = Visibility.Visible;
            _canvasUtilities.RefreshClampedSplineInCanvas(_selectedClampedSpline, canvas1, _engine);
        }

        public Point CursorPosition(MouseEventArgs e)
        {
            return new Point(e.GetPosition(LayoutRoot).X - W, e.GetPosition(LayoutRoot).Y - H);

        }

        public double CursorPositionY(MouseEventArgs e)
        {
            return e.GetPosition(LayoutRoot).Y - H;
        }

        public double CursorPositionX(MouseEventArgs e)
        {
            return e.GetPosition(LayoutRoot).Y - H;
        }

        //private void ActualPosition()
        //{
        //    _lastClickedPositionX = (double)_selectedEllipse.GetValue(Canvas.LeftProperty) + W;
        //    cursorPositionY = (double)_selectedEllipse.GetValue(Canvas.TopProperty) + H;


        //    //if (_lastClickedPositionX + W >= canvas1.Width)
        //    //{
        //    //    _selectedEllipse.SetValue(Canvas.LeftProperty, canvas1.Width - W);
        //    //    _lastClickedPositionX = canvas1.Width - W;

        //    //}
        //    //if (_lastClickedPositionX <= 0)
        //    //{
        //    //    _selectedEllipse.SetValue(Canvas.LeftProperty, (double)0);
        //    //    _lastClickedPositionX = 0;

        //    //}
        //    //if (yCursorPosition + H >= canvas1.Height)
        //    //{
        //    //    _selectedEllipse.SetValue(Canvas.TopProperty, canvas1.Height - H);
        //    //    yCursorPosition = canvas1.Height;

        //    //}
        //    //if (yCursorPosition <= 0)
        //    //{
        //    //    _selectedEllipse.SetValue(Canvas.TopProperty, (double)0);
        //    //    yCursorPosition = 0;

        //    //}

        //    xCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaX(_lastClickedPositionX + W_DIV_2, _plotArea, _worldArea);
        //    yCursorPositionInWA = TransformCoordinates.PlotAreaToWorldAreaY(cursorPositionY + H_DIV_2, _plotArea, _worldArea);
        //}

    }
}
