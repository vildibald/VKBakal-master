using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

namespace SilverlightApplication3
{
    public class FunctionComboBoxModel
    {
        private Function _function;

        public Function Function
        {
            get
            {
                return _function;
            }
            set
            {
                _function = value;
            }
        }
    }

    public class FunctionComboBoxViewModel
    {
        public Dictionary<Function, string> EnumLookUp
        {
            get;
            private set;
        }

        public FunctionComboBoxModel Model {get; set;}

        public FunctionComboBoxViewModel()
        {
            Model = new FunctionComboBoxModel();
            EnumLookUp = new Dictionary<Function, string>();

            var fields = typeof(Function).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var enumValue = (Function)field.GetValue(null);
                var descriptionAttribute = (DescriptionAttribute)field.GetCustomAttributes(typeof(DescriptionAttribute), false).First();

                EnumLookUp.Add(enumValue, descriptionAttribute.Description);
            }
        }
    }
}
