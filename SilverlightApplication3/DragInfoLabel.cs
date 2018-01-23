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

    public class DragInfoLabel : Label
    {
        private static readonly double WIDTH = 55;
        private static readonly double HEIGHT = 40;
        private static readonly double FONT_SIZE = 9;
        private static readonly SolidColorBrush BACKGROUND = new SolidColorBrush(Color.FromArgb(96,64,64,64));//new SolidColorBrush(Colors.Transparent);
        private static readonly Cursor DEFAULT_CURSOR = Cursors.Arrow;
        

        public DragInfoLabel(SolidColorBrush foregroundColor)
            : base()
        {
            this.Width = WIDTH;
            this.Height = HEIGHT;
            this.FontSize = FONT_SIZE;
            this.Background = BACKGROUND;
            this.Cursor = DEFAULT_CURSOR;
            this.Foreground = foregroundColor;
        }

        //public DragElement()
        //    : base()
        //{
        //    this.Background = BACKGROUND;
        //    this.Width = WIDTH;
        //    this.Height = HEIGHT;
        //    this.FontSize = FONT_SIZE;
           
        //    this.Cursor = DEFAULT_CURSOR;
        //}


        public DragInfoLabel(int index, double x, double y, SolidColorBrush foregroundColor)
            : base()
        {
           // this._index = index;
            this.Foreground = foregroundColor;
            this.Background = BACKGROUND;
            this.Content = "Idx: "+index + "\n X: "+ x + "\n Y:" + y;
            this.Width = WIDTH;
            this.Height = HEIGHT;
            this.FontSize = FONT_SIZE;
            
            this.Cursor = DEFAULT_CURSOR;
        }

        public void WriteAndSetCoordinates(int index, double x, double y, double setX, double setY)
        {
            this.Content = "Idx: " + index + "\n X: " + String.Format("{0:0.000}", x) + "\n Y: " + String.Format("{0:0.000}", y);
            this.Margin = new Thickness(setX, setY, 0, 0);
        }

        public void WriteAndSetCoordinates(double derivation, double x, double y, double setX, double setY)
        {
            this.Content = "Der: " + String.Format("{0:0.000}", derivation) + "\n X: " + String.Format("{0:0.000}", x) + "\n Y: " + String.Format("{0:0.000}", y);
            this.Margin = new Thickness(setX, setY, 0, 0);
        }

        public void WriteAndSetCoordinates(int index, double derivation, double x, double y, double setX, double setY)
        {
            this.Content = "Idx: " + index + "\n Der: " + String.Format("{0:0.000}", derivation) + "\n X: " + String.Format("{0:0.000}", x) + "\n Y: " + String.Format("{0:0.000}", y);
            this.Margin = new Thickness(setX, setY, 0, 0);
        }
    }
}
