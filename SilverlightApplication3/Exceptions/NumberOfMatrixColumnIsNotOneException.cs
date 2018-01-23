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
    public class NumberOfMatrixColumnIsNotOneException : Exception
    {
        public NumberOfMatrixColumnIsNotOneException()
        {
            
        }

        public NumberOfMatrixColumnIsNotOneException(String s)
        {
            System.Diagnostics.Debug.WriteLine("NumberOfMatrixColumnIsNotOneException: " + s);
        }

    }
}
