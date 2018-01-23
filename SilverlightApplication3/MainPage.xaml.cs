using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;




namespace SilverlightApplication3
{
    public partial class MainPage : UserControl
    {
        /* konstanty */
        private const int W = 8;
        private const int H = 8;
        private const int W_DIV_2 = 4;
        private const int H_DIV_2 = 4;
        private const int PLOT_AREA_MIN_X = 0;
        private const int PLOT_AREA_MAX_X = 800;
        private const int PLOT_AREA_MIN_Y = 0;
        private const int PLOT_AREA_MAX_Y = 600;
        private const double OPTIMAL_SPACE = 1;
        private int WORLD_AREA_DEFAULT_MIN_XY = -10;
        private int WORLD_AREA_DEFAULT_MAX_XY = 10;

        public const double DEFAULT_DRAW_PRECISION = 0.001;
        public const double DEFAULT_REDRAWING_PRECISION = 0.1;
        public const double AVOID_CURSOR_X = 20;
        public const double AVOID_CURSOR_Y = 5;

        public double DrawPrecision { get; set; }
        public double RedrawingPrecision { get; set; }
       
        private SolidColorBrush WHITE_COLOR;
        private SolidColorBrush BLACK_COLOR;
        private SolidColorBrush CANVAS_TEXT_COLOR;


        // private const string BSPLINE_HEADER = "~bsplines~";
        // private const string HSPLINE_HEADER = "~hsplines~";

        private PlotArea _plotArea;
        private WorldArea _worldArea;
        private Parser _parser;
        private CanvasUtilities _canvasUtilities;
        private DragInfoLabel _dragInfoLabel;
        private bool _isCanvasBlack = false;

        /*************/

        /* instancne premenne */

      
        private bool _isMoving = false; // zistuje ci sa hybeme/nehybeme
        private UIElement _selectedEllipse; // do tejto premennej ukladame elipsu na ktory sme klikli
        private Spline _selectedSpline = null;
        private BSpline _selectedBSpline;
        private GlobalBSpline _selectedGlobalBSpline;
        private HermiteSpline _selectedHermiteSpline;
        private ClampedSpline _selectedClampedSpline;
       
        //  bool _knots_changing = false;
        //private double yCursorPositionInWA, xCursorPositionInWA;
        //private double _lastClickedPositionX, yCursorPosition;
        private SelectedDragItem SELECTED_ITEM_TYPE;


        // private int _redrawedCPIndex = 0;

        private SplineDrawer _engine;
       // private AuxiliaryDrawer _auxiliaryEngine;


        private Point _waZeroPointInPACoordinates;
        private int _changedPointIndex, _changedKnotPos;
        private double _leftEllipseX, _rightEllipseX;

        //Spline Lists
        private List<BSpline> _bSplineList;
        private List<GlobalBSpline> _globalBSplineList;
        private List<HermiteSpline> _hermiteSplineList;
        private List<ClampedSpline> _clampedSplineList;



        private OpenFileDialog _openFileDialog;
        private SaveFileDialog _saveFileDialog;
        // private object _selectedSpline;
        //private FunctionComboBoxViewModel functionComboBoxViewModel;
        /**********************/

        public MainPage()
        {
            InitializeComponent();
            this.DrawPrecision = DEFAULT_DRAW_PRECISION;
            this.RedrawingPrecision = DEFAULT_REDRAWING_PRECISION;
            //functionComboBoxViewModel = new FunctionComboBoxViewModel();
            _plotArea = new PlotArea(PLOT_AREA_MIN_X, PLOT_AREA_MAX_X, PLOT_AREA_MIN_Y, PLOT_AREA_MAX_Y);
            _worldArea = new WorldArea(WORLD_AREA_DEFAULT_MIN_XY, WORLD_AREA_DEFAULT_MAX_XY, WORLD_AREA_DEFAULT_MIN_XY, WORLD_AREA_DEFAULT_MAX_XY);
            // _pa.x_min = 0; _pa.x_max = 775; _pa.y_min = 0; _pa.y_max = 575; _wa.x_min = -10; _wa.x_max = 10; _wa.y_min = -10; _wa.y_max = 10;
            _engine = new SplineDrawer(canvas1, _plotArea, _worldArea, DrawPrecision);
            //_auxiliaryEngine = new AuxiliaryDrawer(canvas1);
            _isMoving = false;
            _waZeroPointInPACoordinates = TransformCoordinates.WorldAreaToPlotArea(0, 0, _plotArea, _worldArea);

            _parser = new Parser();
            InitSplineLists();
            _canvasUtilities = new CanvasUtilities(_plotArea,_worldArea);

            _openFileDialog = new OpenFileDialog();
            _saveFileDialog = new SaveFileDialog();
            InitColors();
            FunctionSelect_ComboBox.SelectedIndex = 0;
            _dragInfoLabel = new DragInfoLabel(CANVAS_TEXT_COLOR);
            //_dragInfoLabel.Margin = new Thickness(20, 30, 0, 0);
            //_dragInfoLabel.Content = "SDAS";
            canvas1.Children.Add(_dragInfoLabel);
            _dragInfoLabel.Visibility = Visibility.Collapsed;
            VersionLabel.Content = Version();

            try
            {
                this._saveFileDialog.DefaultExt = ".spline";
                this._openFileDialog.Filter = "Spline Files|*.spline";
                this._openFileDialog.FilterIndex = 2;
                this._saveFileDialog.Filter = "Spline Files|*.spline";
                this._saveFileDialog.FilterIndex = 2;
                // this._saveFileDialog.DefaultFileName = "newSplines.spline";
            }
            catch (Exception)
            {
              
            }
            DefaultCanvas();
            //var s = BSpline.ServiceKnots(new double[] {2,3.75,5},3,true);
            
           // _engine.BellFunctionOfDegree3(0, 1, 2, 3, 4);
           // _engine.BSplineBasisFunctionsOfDegree3(0, 1, 2, 3, 4, 5, 6, 7);
            //_engine.BSplineBasisFunction(new double[]{0, 1, 2, 3, 4, 5, 6, 7});
            // double[] knots = new double[] {-1,1,2,2.5, 3, 4, 5, 6};
            // Tuple<double[][], double[]> A = GlobalBSplineMatrix(3, knots);
            // double[][] B = A.Item1; //MathMyUtils.MatrixInvert(A.Item1);
            // //double[][] F = MathMyUtils.MatrixInvert(B);
            //// double[][] C = MathMyUtils.TransposeMatrix(B);

            //// //double[][] D = MathMyUtils.MatrixInvert(MathMyUtils.MultiplyMatrices(C, B));
            //// //double[][] E = MathMyUtils.MultiplyMatrices(B, C);
            ////// double[][] YMatrix = MathMyUtils.ArrayToMatrix(knotsFunctionValues);
            // for (int i = 0; i < B.Length; i++)
            // {
            //     String s = "";
            //     for (int j = 0; j < B[i].Length; j++)
            //     {

            //         s += B[i][j].ToString() + " | ";
            //     }
            //     System.Diagnostics.Debug.WriteLine(s);
            // }
        }

        public string Version()
        {
            string name = Assembly.GetExecutingAssembly().FullName;
            AssemblyName asmName = new AssemblyName(name);

            // http://www.dotnet247.com/247reference/msgs/45/225355.aspx
            return asmName.Version.Major + "." + asmName.Version.Minor + "." + asmName.Version.Build;
        }

        private void InitSplineLists()
        {
            _bSplineList = new List<BSpline>();
            _hermiteSplineList = new List<HermiteSpline>();
            _clampedSplineList = new List<ClampedSpline>();
            _globalBSplineList = new List<GlobalBSpline>();
        }

        private void InitColors()
        {
            WHITE_COLOR = new SolidColorBrush(Colors.White);
            BLACK_COLOR = new SolidColorBrush(Colors.Black);
            CANVAS_TEXT_COLOR = BLACK_COLOR;
        }

        

        private void canvas1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cliper.Rect = new Rect(0, 0, canvas1.ActualWidth, canvas1.ActualHeight); cliper.Rect = new Rect(0, 0, outterGrid.ActualWidth, outterGrid.ActualHeight);
        }


        private void canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            // pri kliknuti si ulozime poziciu bodu
            //Point point = e.GetPosition(null);
            
            // pri double kliku pridavame objekty
            switch (e.ClickCount)
            {
                case 2:
                    var cursorPosition = CursorPosition(e);
                    var cursorPositionInWA = TransformCoordinates.PlotAreaToWorldArea(cursorPosition, _plotArea, _worldArea);
                    if (_selectedSpline != null)
                    {
                        
                        if (_selectedSpline is BSpline)
                        {
                            //_selectedBSpline = (BSpline) _selectedSpline;
                            //var knots = _selectedBSpline.KnotsWithoutServiceKnots().ToList();
                            //var idx = ArrayMyUtils.FindFirstIndexInSortedListWhichValueIsBiggerThanDesiredNumber(knots, cursorPositionInWA.X);
                            //if (idx != -1)
                            //{
                            //    knots.Insert(idx, cursorPositionInWA.X);
                            //}
                            //else
                            //{
                            //    knots.Add(cursorPositionInWA.X);
                            //}
                            
                        }
                        else if (_selectedSpline is HermiteSpline)
                        {
                            _selectedHermiteSpline = (HermiteSpline)_selectedSpline;

                            for (int i = 0; i < _selectedHermiteSpline.LinesOfSpline.Count; i++)
                            {
                                var b = canvas1.Children.Remove(_selectedHermiteSpline.LinesOfSpline[i]);
                                if (!b)
                                {
                                    haha.Text = "cannot remove line";
                                }
                                else
                                {
                                    haha.Text = "line removed";
                                }
                            }

                            var knots = _selectedHermiteSpline.Knots;
                            var controlPoints = _selectedHermiteSpline.ControlPoints;
                            var derivations = _selectedHermiteSpline.Derivations;
                            
                            var idx =  ArrayMyUtils.FindFirstIndexInSortedListWhichValueIsBiggerThanDesiredNumber(knots, cursorPositionInWA.X);

                            if (idx != -1)
                            {
                                knots.Insert(idx, cursorPositionInWA.X);
                                controlPoints.Insert(idx, cursorPositionInWA.Y);
                                derivations.Insert(idx, 0);
                            }
                            else
                            {
                                knots.Add(cursorPositionInWA.X);
                                controlPoints.Add(cursorPositionInWA.Y);
                                derivations.Add(0);
                            }

                            //_selectedHermiteSpline.Knots = knots;
                            //_selectedHermiteSpline.ControlPoints = controlPoints;
                            //_selectedHermiteSpline.Derivations = derivations;
                            //_canvasUtilities.TotalRefreshHermiteSplineInCanvas(_selectedHermiteSpline, canvas1, _engine);
                            //_canvasUtilities.RemoveInCanvas(canvas1, _selectedHermiteSpline.LinesOfSpline);
                            //_canvasUtilities.RemoveInCanvas(canvas1, _selectedHermiteSpline.DerivationEllipses);
                            //_canvasUtilities.RemoveInCanvas(canvas1, _selectedHermiteSpline.DragEllipses);
                            //_hermiteSplineList.Remove(_selectedHermiteSpline);
                          
                           
                            var spline = _engine.InteractiveHermiteSpline(_selectedHermiteSpline.Knots.ToArray(), _selectedHermiteSpline.ControlPoints.ToArray(), _selectedHermiteSpline.Derivations.ToArray());
                            _hermiteSplineList.Add(spline);
                        }
                        else if(_selectedSpline is ClampedSpline)
                        {

                        }
                        else if (_selectedSpline is GlobalBSpline)
                        {

                        }
                    }

                   
                    break;
                // hybanie s objektami
                case 1:
                    var elements = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (UIElement)sender) as List<UIElement>;
                    for (int i = 0; i < elements.Count; i++)
                    {

                        if (elements[i] is Ellipse)
                        {

                            _selectedEllipse = elements[i];
                            
                            for (int j = 0; j < _bSplineList.Count; j++)
                            {
                                _selectedBSpline = _bSplineList[j];
                                _selectedSpline = _selectedBSpline;
                                _changedPointIndex = 0;
                                //if (_selectedBSpline.FunctionValues == null)
                                //{
                                var controlPointsCount = _selectedBSpline.ControlPoints.Count();
                                while (_changedPointIndex < controlPointsCount)
                                {
                                    if (_selectedEllipse.Equals(_selectedBSpline.DragEllipses[_changedPointIndex]))
                                    {
                                        
                                        haha.Text += " CP: " + _changedPointIndex + "   ";
                                        //_changedKnotPos = _changedPointIndex + 1;
                                        if (_changedPointIndex == 0)
                                        {
                                            _leftEllipseX = -1;
                                            _rightEllipseX = (double)_selectedBSpline.DragEllipses[_changedPointIndex + 1].GetValue(Canvas.LeftProperty);
                                        }
                                        else if (_changedPointIndex == _selectedBSpline.DragEllipses.Count-1)
                                        {
                                            _leftEllipseX = (double)_selectedBSpline.DragEllipses[_changedPointIndex - 1].GetValue(Canvas.LeftProperty);
                                            _rightEllipseX = TransformCoordinates.WorldAreaToPlotAreaX(_selectedBSpline.Knots[_changedPointIndex+2]-DEFAULT_REDRAWING_PRECISION,_plotArea,_worldArea);
                                            
                                        }
                                        else
                                        {
                                            _leftEllipseX = (double)_selectedBSpline.DragEllipses[_changedPointIndex - 1].GetValue(Canvas.LeftProperty);
                                            _rightEllipseX = (double)_selectedBSpline.DragEllipses[_changedPointIndex + 1].GetValue(Canvas.LeftProperty);
                                        }
                                        SELECTED_ITEM_TYPE = SelectedDragItem.BSplineEllipse;
                                        _isMoving = true;
                                        _changedKnotPos = _changedPointIndex + 1;
                                       
                                        // _selectedSpline = _redrawedBSpline;
                                        haha.Text += " Knot: " + _changedKnotPos + "   ";
                                        //goto DRAG; // preskocime nasledujuci for-cyklus
                                        _engine.DrawPrecision = RedrawingPrecision;
                                        _selectedEllipse.CaptureMouse();
                                        return;
                                    }
                                    _changedPointIndex++;
                                }
                                //}
                                //else
                                //{

                            }

                            for (int j = 0; j < _globalBSplineList.Count; j++)
                            {
                                _selectedGlobalBSpline = _globalBSplineList[j];
                                _selectedSpline = _selectedGlobalBSpline;
                                _changedPointIndex = 0;
                                 var redrawedGlobalBSplineCount = _selectedGlobalBSpline.ControlPoints.Count();
                               
                                if (_selectedEllipse.Equals(_selectedGlobalBSpline.LeftDerivationEllipse))
                                {
                                    SELECTED_ITEM_TYPE = SelectedDragItem.GBSplineLeftDerEllipse;
                                    _isMoving = true;
                                    _changedKnotPos = _changedPointIndex;
                                    _engine.DrawPrecision = RedrawingPrecision;
                                    _selectedEllipse.CaptureMouse();
                                    return;
                                }
                                else if (_selectedEllipse.Equals(_selectedGlobalBSpline.RightDerivationEllipse))
                                {
                                    SELECTED_ITEM_TYPE = SelectedDragItem.GBSplineRightDerEllipse;
                                    _isMoving = true;
                                    _changedKnotPos = _changedPointIndex;
                                    _engine.DrawPrecision = RedrawingPrecision;
                                    _selectedEllipse.CaptureMouse();
                                    return;
                                }
                                else
                                {
                                    while (_changedPointIndex < _selectedGlobalBSpline.FunctionValues.Count())
                                    {
                                        if (_selectedEllipse.Equals(_selectedGlobalBSpline.DragEllipses[_changedPointIndex]))
                                        {
                                            //haha.Text += " CP: " + _changedPointIndex + "   ";
                                            SELECTED_ITEM_TYPE = SelectedDragItem.GBSplineEllipse;
                                            
                                            // _selectedSpline = _redrawedBSpline;
                                           // haha.Text += " Knot: " + _changedKnotPos + "   ";
                                            //  goto DRAG; // preskocime nasledujuci for-cyklus

                                            _isMoving = true;
                                            _changedKnotPos = _changedPointIndex;
                                            _engine.DrawPrecision = RedrawingPrecision;
                                            _selectedEllipse.CaptureMouse();
                                            return;
                                        }
                                        _changedPointIndex++;
                                    }
                                }
                           
                               
                            }

                            for (int j = 0; j < _clampedSplineList.Count; j++)
                            {

                                _selectedClampedSpline = _clampedSplineList[j];
                                _selectedSpline = _selectedClampedSpline;
                                int redrawedCSplineCount = _selectedClampedSpline.ControlPoints.Count();
                                _changedPointIndex = 0;
                                if (_selectedEllipse.Equals(_selectedClampedSpline.LeftDerivationEllipse))
                                {
                                    SELECTED_ITEM_TYPE = SelectedDragItem.CSplineLeftDerEllipse;
                                    _isMoving = true;
                                    //_selectedSpline = _redrawedHermiteSpline;
                                    // goto DRAG;
                                    _engine.DrawPrecision = RedrawingPrecision;
                                    _selectedEllipse.CaptureMouse();
                                    return;
                                }
                                else if (_selectedEllipse.Equals(_selectedClampedSpline.RightDerivationEllipse))
                                {
                                    SELECTED_ITEM_TYPE = SelectedDragItem.CSplineRightDerEllipse;
                                    _isMoving = true;
                                    //_selectedSpline = _redrawedHermiteSpline;
                                    // goto DRAG;
                                    _engine.DrawPrecision = RedrawingPrecision;
                                    _selectedEllipse.CaptureMouse();
                                    return;
                                }
                                else
                                {

                                    while (_changedPointIndex < redrawedCSplineCount)
                                    {

                                        if (_selectedEllipse.Equals(_selectedClampedSpline.DragEllipses[_changedPointIndex]))
                                        {
                                            //haha.Text += _changedPointIndex + "   ";

                                            SELECTED_ITEM_TYPE = SelectedDragItem.CSplineEllipse;
                                            _isMoving = true;
                                            //_selectedSpline = _redrawedHermiteSpline;
                                            // goto DRAG;
                                            _engine.DrawPrecision = RedrawingPrecision;
                                            _selectedEllipse.CaptureMouse();
                                            return;

                                        }
                                        _changedPointIndex++;
                                        //   }

                                    }
                                }
                               

                            }

                            for (int j = 0; j < _hermiteSplineList.Count; j++)
                            {

                                _selectedHermiteSpline = _hermiteSplineList[j];
                                _selectedSpline = _selectedHermiteSpline;
                                int redrawedHSplineControlPointsCount = _selectedHermiteSpline.ControlPoints.Count();
                                _changedPointIndex = 0;
                                while (_changedPointIndex < redrawedHSplineControlPointsCount)
                                {

                                    if (_selectedEllipse.Equals(_selectedHermiteSpline.DragEllipses[_changedPointIndex]))
                                    {
                                        haha.Text += _changedPointIndex + "   ";
                                        if (_changedPointIndex == 0)
                                        {
                                            _leftEllipseX = -1;
                                            _rightEllipseX = (double)_selectedHermiteSpline.DragEllipses[_changedPointIndex + 1].GetValue(Canvas.LeftProperty);
                                        }
                                        else if (_changedPointIndex == redrawedHSplineControlPointsCount - 1)
                                        {
                                            _leftEllipseX = (double)_selectedHermiteSpline.DragEllipses[_changedPointIndex - 1].GetValue(Canvas.LeftProperty);
                                            _rightEllipseX = -1;
                                        }
                                        else
                                        {
                                            _leftEllipseX = (double)_selectedHermiteSpline.DragEllipses[_changedPointIndex - 1].GetValue(Canvas.LeftProperty);
                                            _rightEllipseX = (double)_selectedHermiteSpline.DragEllipses[_changedPointIndex + 1].GetValue(Canvas.LeftProperty);
                                        }
                                        SELECTED_ITEM_TYPE = SelectedDragItem.HSplineEllipse;
                                        _isMoving = true;
                                        //_selectedSpline = _redrawedHermiteSpline;
                                        // goto DRAG;
                                        _engine.DrawPrecision = RedrawingPrecision;
                                        _selectedEllipse.CaptureMouse();
                                        return;
                                    }
                                    else if(_selectedEllipse.Equals(_selectedHermiteSpline.DerivationEllipses[_changedPointIndex]))
                                    {
                                        SELECTED_ITEM_TYPE = SelectedDragItem.HSplineDerEllipse;
                                        _isMoving = true;
                                        _engine.DrawPrecision = RedrawingPrecision;
                                        _selectedEllipse.CaptureMouse();
                                        return;
                                    }
                                    _changedPointIndex++;
                                    //   }

                                }

                            }


                            //DRAG:
                            //_selectedEllipse.CaptureMouse();
                        }
                    }
                    
                    break;

            }

        }



        private void canvas1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ClickCount)
            {
                case 2:

                    break;
                case 1:


                    break;
            }
        }

        private void canvas1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
           


        }

        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {

            if (_isMoving)
            {

                //ActualPosition();
                //setSelectedItemTextBoxText(xCursorPositionInWA, yCursorPositionInWA,_lastClickedPositionX,yCursorPosition);

                switch (SELECTED_ITEM_TYPE)
                {
                    case SelectedDragItem.BSplineEllipse: //draging b-spline
                        DragBSpline(e);
                        break;

                    case SelectedDragItem.HSplineEllipse: // draging hSpline
                        DragHSpline(e);

                        break;
                    case SelectedDragItem.HSplineDerEllipse: // draging hSpline derivation
                        DragHSplineDerivation(e);
                        break;
                    case SelectedDragItem.GBSplineEllipse: //draging global bspline

                        DragGBSpline(e);
                        break;
                    case SelectedDragItem.GBSplineLeftDerEllipse: // draging global gbSpline left derivations
                        DragGBSplineLeftDerivation(e);
                        break;
                    case SelectedDragItem.GBSplineRightDerEllipse:// draging global gbSpline right derivations
                        DragGBSplineRightDerivation(e);
                        break;
                    case SelectedDragItem.CSplineEllipse: // draging cSpline

                        DragCSpline(e);

                        break;
                    case SelectedDragItem.CSplineLeftDerEllipse: // draging cSpline left derivations

                        DragCSplineLeftDerivation(e);

                        break;

                    case SelectedDragItem.CSplineRightDerEllipse: // draging cSpline right derivations

                        DragCSplineRightDerivation(e);

                        break;
                }

            }


        }


        private void canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMoving)
            {
                _selectedEllipse.ReleaseMouseCapture();
                _engine.DrawPrecision = DrawPrecision;
                ResetCanvas();
                _dragInfoLabel.Visibility = Visibility.Collapsed;
                _selectedEllipse = null;
            }
            _isMoving = false;

        }

        /* prvky gui ***************************************************************************************/
        protected void MethodTester(object sender, RoutedEventArgs e)
        //service method to help debugging
        {


        }

        protected void btn_Click(object sender, RoutedEventArgs e)
        {
            canvas1.Children.Clear();
            RemoveAllSplines();
            DefaultCanvas();
          
        }

        protected void runge_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double leftInterval = double.Parse(leftIntervalTextBox.Text);//, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                double rightInterval = double.Parse(rightIntervalTextBox.Text);//, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
                int numberOfPoints = int.Parse(number_of_points_TextBox.Text);
                int numberOfIntervals = int.Parse(number_of_intervals_TextBox.Text);
                int p = int.Parse(degree_runge_TextBox.Text);

                //this.haha.Text = this.haha.Text + leftInterval;
                _engine.RungeFunction(leftInterval, rightInterval, numberOfPoints, numberOfIntervals, p);
                //engine.polynomial_function(new double[] {1.0,1.0,1.0,1.0,1.0,1.0}, -5.0, 5.0);
            }
            catch (Exception)
            {
                this.haha.Text = "Incorect numeric input";
            }


        }

        protected void bellButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _engine.BellFunctionOfDegree3(double.Parse(bellTextBox0.Text), double.Parse(bellTextBox1.Text), double.Parse(bellTextBox2.Text), double.Parse(bellTextBox3.Text), double.Parse(bellTextBox4.Text));
            }
            catch (Exception)
            {
                this.haha.Text = "Incorect numeric input";
            }
        }

        protected void basisButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int p = int.Parse(degree_basis_TextBox.Text);

                double[] knots = _parser.StringOfDecimalsToArray(knots_basis_TextBox.Text);
                //if (p + 2 == knots.Length)
               // {
                    _engine.BSplineBasisFunction(p,knots);
                //}
                //else
                //{
                //    this.haha.Text = "Number of knots should be degree+2.";
               // }



            }
            catch (Exception)
            {
                this.haha.Text = "Incorect numeric input";
            }
        }

        protected void ChangeWAButtonClick(object sender, RoutedEventArgs e)
        {

            double newWAXMin;
            double newWAXMax;
            double newWAYMin;
            double newWAYMax;

            try
            {
                newWAXMin = double.Parse(x_min_TextBox.Text);
                newWAXMax = double.Parse(x_max_TextBox.Text);
                newWAYMin = double.Parse(y_min_TextBox.Text);
                newWAYMax = double.Parse(y_max_TextBox.Text);



                SetNewWorldArea(newWAXMin, newWAXMax, newWAYMin, newWAYMax);

            }
            catch (Exception)
            {
                this.haha.Text = "Incorect numeric input";
                return;
            }

        }

        private void GlobalBSplineButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {

                var degree = int.Parse(rungeBSplineDegreeTextBox.Text);
                var knots = _parser.StringOfDecimalsToArray(FunctionBSplineXTextBox.Text);
                double[] functionValues = null;
                double leftDer = 0;
                double rightDer = 0;
                try
                {
                    functionValues = _parser.StringOfDecimalsToArray(FunctionBSplineYTextBox.Text);
                    leftDer = double.Parse(FunctionBSplineLeftDerTextBox.Text);
                    rightDer = double.Parse(FunctionBSplineRightDerTextBox.Text);
                }
                catch (Exception) { haha.Text = "Incorect controlPoints coordinates or derivations"; }

                GlobalBSpline spline = null;
                if (FunctionSelect_ComboBox.SelectedValue.Equals(Function.Sinus))
                    spline = _engine.FunctionInteractiveGlobalBSpline(degree, knots, Function.Sinus);
                else if (FunctionSelect_ComboBox.SelectedValue.Equals(Function.Runge))
                    spline = _engine.FunctionInteractiveGlobalBSpline(degree, knots, Function.Runge);
                else if (FunctionSelect_ComboBox.SelectedValue.Equals(Function.UserDefined) && functionValues != null)
                    spline = _engine.InteractiveGlobalBSpline(degree, knots, functionValues, leftDer, rightDer);
                _globalBSplineList.Add(spline);
            }
            catch (Exception)
            {
                this.haha.Text = "Incorect numeric input";
            }
        }

        protected void hermite_Button_Click(object sender, RoutedEventArgs e)
        {
            double[] waX;
            double[] waY;
            double[] derivations;

            try
            {
                waX = _parser.StringOfDecimalsToArray(hermite_X_TextBox.Text);
                waY = _parser.StringOfDecimalsToArray(hermite_Y_TextBox.Text);
                derivations = _parser.StringOfDecimalsToArray(derivation_TextBox.Text);
                if (waX.Length != waY.Length || waX.Length!= derivations.Length)
                {
                    this.haha.Text = "Numbers of X and Y coordinates and derivations must be equal " + waX.Length + " " + waY.Length;
                    return;
                }
            }
            catch (Exception)
            {
                this.haha.Text = "Incorect numeric input";
                return;
            }

            HermiteSpline hSpline = _engine.InteractiveHermiteSpline(waX, waY, derivations);
            _hermiteSplineList.Add(hSpline);
        }

        protected void ClampedSplineButtonClick(object sender, RoutedEventArgs e)
        {
            double waLX;
            double waRX;
            double[] waY;
            double leftDer;
            double rightDer;

            try
            {
                waLX = double.Parse(clamped_XLeft_TextBox.Text);
                waRX = double.Parse(clamped_XRight_TextBox.Text);
                waY = _parser.StringOfDecimalsToArray(clamped_Y_TextBox.Text);
                leftDer = double.Parse(left_derivation_TextBox.Text);
                rightDer = double.Parse(right_derivation_TextBox.Text);
                if (waLX >= waRX)
                {
                    this.haha.Text = "Incorrect interval definition";
                    return;
                }
                if (waY.Length < 3)
                {
                    this.haha.Text = "Small number of definition points";
                    return;
                }

            }
            catch (Exception)
            {
                this.haha.Text = "Incorect numeric input";
                return;
            }

            ClampedSpline cSpline = _engine.InteractiveClampedSpline(waLX, waRX, waY, leftDer, rightDer);
            _clampedSplineList.Add(cSpline);
        }

        protected void spline_Button_Click(object sender, RoutedEventArgs e)
        {

            double[] WAInputKnots;
            double[] WAControlPoints;

            int degree;
            try
            {
                degree = int.Parse(degree_TextBox.Text);
                // int ControlPointsCount = 2 * degree + 4;
                //int KnotsCount = 2 * degree + 2;

                WAInputKnots = _parser.StringOfDecimalsToArray(spline_knots_TextBox.Text);
                WAControlPoints = _parser.StringOfDecimalsToArray(spline_cp_TextBox.Text);

                //ellipses = new List<Ellipse>();
                //if(WAControlPoints.Count)
                //WAKnots = EngineUtils.ServiceKnots(WAInputKnots, degree, true);
                _bSplineList.Add(_engine.InteractiveBSpline(degree, WAInputKnots, WAControlPoints));
            }
            catch (Exception)
            {
                this.haha.Text = "Incorect numeric input";
                return;
            }


        }


        protected void Zoom_out_button_click(object sender, RoutedEventArgs e)
        {
            var nxmin = _worldArea.XMin - 0.5;
            var nxmax = _worldArea.XMax + 0.5;
            var nymin = _worldArea.YMin - 0.5;
            var nymax = _worldArea.YMax + 0.5;
            SetNewWorldArea(nxmin, nxmax, nymin, nymax);
            x_min_TextBox.Text = nxmin.ToString();
            x_max_TextBox.Text = nxmax.ToString();
            y_min_TextBox.Text = nymin.ToString();
            y_max_TextBox.Text = nymax.ToString();
            //Zoom_slider.Value = ScaleTransform.ScaleX;

        }

        protected void Zoom_in_button_click(object sender, RoutedEventArgs e)
        {
            var nxmin = _worldArea.XMin + 0.5;
            var nxmax = _worldArea.XMax - 0.5;
            var nymin = _worldArea.YMin + 0.5;
            var nymax = _worldArea.YMax - 0.5;
            SetNewWorldArea(nxmin, nxmax, nymin, nymax);
            x_min_TextBox.Text = nxmin.ToString();
            x_max_TextBox.Text = nxmax.ToString();
            y_min_TextBox.Text = nymin.ToString();
            y_max_TextBox.Text = nymax.ToString();

            //Zoom_slider.Value = ScaleTransform.ScaleX;
        }



        protected void Zoom_reset_button_click(object sender, RoutedEventArgs e)
        {
            SetNewWorldArea(WORLD_AREA_DEFAULT_MIN_XY, WORLD_AREA_DEFAULT_MAX_XY, WORLD_AREA_DEFAULT_MIN_XY, WORLD_AREA_DEFAULT_MAX_XY);
            x_min_TextBox.Text = WORLD_AREA_DEFAULT_MIN_XY.ToString();
            x_max_TextBox.Text = WORLD_AREA_DEFAULT_MAX_XY.ToString();
            y_min_TextBox.Text = WORLD_AREA_DEFAULT_MIN_XY.ToString();
            y_max_TextBox.Text = WORLD_AREA_DEFAULT_MAX_XY.ToString();
        }

        void Zoom_slider_valueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //NOT IMPLEMENTED YET
            // SetNewWorldArea(;
        }


        protected void SaveToFileButtonClick(object sender, RoutedEventArgs e)
        {
            bool? dialogResult = this._saveFileDialog.ShowDialog();

            if (dialogResult.Value)
            {
                try
                {

                    using (Stream saveStream = _saveFileDialog.OpenFile())
                    using (StreamWriter saveWriter = new StreamWriter(saveStream))
                    {
                        _parser.SaveSplinesToFile(saveWriter, _bSplineList, _globalBSplineList, _hermiteSplineList, _clampedSplineList);

                    }
                }
                catch (Exception)
                {

                }
            }

        }

        protected void LoadFromFileButtonClick(object sender, RoutedEventArgs e)
        {

            // openFileDialog.Filter = FILE_FILTER;
            bool? dialogResult = this._openFileDialog.ShowDialog();
            if (dialogResult.Value)
            {
                try
                {
                    using (Stream openStream = _openFileDialog.File.OpenRead())
                    using (StreamReader openReader = new StreamReader(openStream))
                    {
                        _parser.LoadSplinesFromFile(openReader, _bSplineList, _globalBSplineList, _hermiteSplineList, _clampedSplineList, _engine);
                    }
                }
                catch (Exception)
                {

                }

            }
        }


        // Demo Buttons ///////////////////////////////////////////////////////

        protected void BellDemoButtonClick(object sender, RoutedEventArgs e)
        {
            _engine.BellFunctionOfDegree3(0, 1, 2, 3, 4);
        }
        protected void RungeDemoButtonClick(object sender, RoutedEventArgs e)
        {
            _engine.RungeFunction(-4, 4, 25, 7, 3);
        }

        protected void BasisDemoButtonClick(object sender, RoutedEventArgs e)
        {
            _engine.BSplineBasisFunction(3,new double[] { 0, 1, 2, 3, 4 });
        }
        protected void BSplineDemoButtonClick(object sender, RoutedEventArgs e)
        {
            int degree = 3;
            double[] knots = new double[] { -1, 0, 1, 2, 3, 4, 5 };
            double[] controlPoints = new double[] { 0, 2, -1, 1, -1, 1, -1, 0, 1 };
            double[] serviceKnots = BSpline.ServiceKnots(knots, degree, true);
            var spline = _engine.InteractiveBSpline(degree, knots, controlPoints);
            _bSplineList.Add(spline);

            //OptimalWorldArea();
        }

        protected void HSplineDemoButtonClick(object sender, RoutedEventArgs e)
        {
            double[] X = new double[] { -2, 0, 1, 4 };
            double[] Y = new double[] { 4, -6, 1, 8 };
            double[] ders = new double[] { -2, 0, 1, 2 };

            HermiteSpline spline = _engine.InteractiveHermiteSpline(X, Y, ders);
            _hermiteSplineList.Add(spline);
            //OptimalWorldArea(spline);
        }
        protected void GlobalBSplineDemoButtonClick(object sender, RoutedEventArgs e)
        {
            // var spline = _engine.FunctionApproximationBSpline(2, 0, 6, 51, 7, Function.Sinus);
            //var spline = _engine.FunctionInteractiveGlobalBSpline(3, new double[] { -1, 0, 1, 2 ,3, 4,5,6 }, Function.Sinus);
            var spline = _engine.InteractiveGlobalBSpline(3, new double[] { -1, 0, 1, 2, 3, 4, 5 }, new double[] { 2, 2, 2, 2, 2, 2, 2 }, 1, 2,false);
            _globalBSplineList.Add(spline);
            //OptimalWorldArea();
        }
        protected void CSplineDemoButtonClick(object sender, RoutedEventArgs e)
        {
            double x1 = -1;
            double x2 = 5;
            double[] Y = new double[] { 2, 2, 2, 2, 2, 2, 2 };
            //double[] Y = new double[] { 3, 2, -1, 1, 3, 0 };
            double leftDerivation = 1;
            double rightDerivation = 2;

            ClampedSpline spline = _engine.InteractiveClampedSpline(x1, x2, Y, leftDerivation, rightDerivation);
            _clampedSplineList.Add(spline);
            //OptimalWorldArea();
            //System.Diagnostics.Debug.WriteLine("CS ders: " + string.Join(",", spline.Derivations));
        }
        ///////////////////////////////////////////////////////////////////////

        
        protected void OptimalZoom_ButtonClick(object sender, RoutedEventArgs e)
        {
            OptimalWorldArea();
        }
        // not work properly yet
        public void OptimalWorldArea(Spline spline)
        {
            var splineRange = spline.Range_MinX_MaxX_MinY_MaxY(_plotArea, _worldArea);
            var optimalMinX = _worldArea.XMin;
            var optimalMaxX = _worldArea.XMax;
            var optimalMinY = _worldArea.YMin;
            var optimalMaxY = _worldArea.YMax;

            if (splineRange.Item1 < optimalMinX)
                optimalMinX = splineRange.Item1;
            else if (splineRange.Item2 > optimalMaxX)
                optimalMaxX = splineRange.Item2;
            else if (splineRange.Item3 < optimalMinY)
                optimalMinY = splineRange.Item3;
            else if (splineRange.Item4 > optimalMaxY)
                optimalMaxY = splineRange.Item4;

            SetNewWorldArea(optimalMinX - optimalMinX * OPTIMAL_SPACE, optimalMaxX + optimalMaxX*OPTIMAL_SPACE, optimalMinY - optimalMinY* OPTIMAL_SPACE, optimalMaxY + optimalMaxY*OPTIMAL_SPACE);
        }

        

        protected void doubleAccepted(object sender, KeyEventArgs e)
        {
            Parser.DoubleAccepted(sender, e);
        }
        protected void intAccepted(object sender, KeyEventArgs e)
        {
            Parser.IntAccepted(sender, e);
        }



        private void AttributesTextBox3_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void rungeBSplinenumberOfIntervalsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void setSelectedItemTextBoxText(double x, double y, double marginX, double marginY)
        {
            //SelectedItemCoordination_TextBox.Text = String.Format("{0:0.00}", x) + " : " + String.Format("{0:0.00}", y);
            // SelectedItemCoordination_TextBox.Margin = new Thickness(setX-10, setY-5, 0, 0);
        }

        private void SetCanvasWhite(object sender, RoutedEventArgs e)
        {
            SwapCanvasColor();
        }

        private void CurveThick(object sender, RoutedEventArgs e)
        {
            //var thickness = _auxiliaryEngine.LineThickness;
            try
            {
                _engine.CurveThickness(int.Parse(curveThickTextBox.Text));
                ResetCanvas();
                
            }
            catch(Exception)
            {
                haha.Text = "Setting curve thickness failed, check input value";
            }
               
        }

        
        ///////////////////////////////////////////////////////////////////////
        /***************************************************************************************************/


    }
}
