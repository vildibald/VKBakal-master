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
    public class ListMyUtils
    {
        public static List<T> InitializedList<T>(int capacity, T value)
        {
            List<T> list = new List<T>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                list.Add(value);
            }
            return list;
        }
    }
}
