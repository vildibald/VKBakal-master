using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverlightApplication3
{
    class IncorectSplineInputException : Exception
    {
        public IncorectSplineInputException()
            :base()
        {

        }

        public IncorectSplineInputException(String s)
            :base(s)
        {
            System.Diagnostics.Debug.WriteLine("IncorectSplineInputExcetion: "+s);
        }
    }
}
