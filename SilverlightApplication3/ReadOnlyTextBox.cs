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
    public class ReadOnlyTextBox : TextBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            e.Handled = true;
            base.OnKeyDown(e);
        }
    }
}
