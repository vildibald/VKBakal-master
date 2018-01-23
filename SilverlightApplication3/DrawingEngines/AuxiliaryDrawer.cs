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
    public class AuxiliaryDrawer
    {
        private Canvas _canvas;
        private const int W = 8;
        private const int H = 8;
        public double LineThickness { get; set; }
        public const double DEFAULT_LINE_THICKNESS = 2;
        
        //private TransformCoordinates _transformCoordinates;

        public AuxiliaryDrawer(Canvas canvas)
        {
            this._canvas = canvas;
            this.LineThickness = DEFAULT_LINE_THICKNESS;
        
        }
        public void DrawSquare(int left, int top)
        {
            Rectangle stvorec = new Rectangle();
            stvorec.Width = W;
            stvorec.Height = H;
            SolidColorBrush farba = new SolidColorBrush();
            farba.Color = Color.FromArgb(200, 128, 20, 20);
            stvorec.Fill = farba;
            Canvas.SetLeft(stvorec, left);
            Canvas.SetTop(stvorec, top);
            Canvas.SetZIndex(stvorec, 5);
            _canvas.Children.Add(stvorec);
        }

        public Line DrawLine(double left1, double top1, double left2, double top2, SolidColorBrush color)
        {
            Line ciara = new Line();
            ciara.X1 = left1; ciara.Y1 = top1; ciara.X2 = left2; ciara.Y2 = top2;

            ciara.StrokeThickness = LineThickness;
            ciara.Stroke = color;
            Canvas.SetZIndex(ciara, -1);
            _canvas.Children.Add(ciara);
            return ciara;
        }

        public Line DrawLine(Point a, Point b, SolidColorBrush color)
        {
            return DrawLine(a.X, a.Y, b.X, b.Y, color);
        }

        public void DrawTextBlock(int left, int right)
        {
            TextBox tb = new TextBox();
            tb.Width = 60; tb.Height = 30;
            SolidColorBrush farba = new SolidColorBrush();
            farba.Color = Color.FromArgb(200, 40, 127, 127);
            tb.Background = farba;
            Canvas.SetLeft(tb, left);
            Canvas.SetTop(tb, right);
            _canvas.Children.Add(tb);
        }

        public Ellipse DrawEllipse(double left, double top, SolidColorBrush color)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = W;
            ellipse.Height = H;
            ellipse.Fill = color;
            DrawEllipse(left, top, ellipse);
            return ellipse;
        }



        public void DrawEllipse(double left, double top,Ellipse ellipse)
        {
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);
            Canvas.SetZIndex(ellipse, 0);
            _canvas.Children.Add(ellipse);
        }
        public Ellipse DrawEllipse(Point point, SolidColorBrush color)
        {
            return DrawEllipse(point.X, point.Y, color);
        }

    }
}
