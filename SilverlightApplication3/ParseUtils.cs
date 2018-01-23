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
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace SilverlightApplication3
{
    public class NumberParser
    {
        private const string BSPLINE_HEADER = "~bsplines~";
        private const string HSPLINE_HEADER = "~hsplines~";
        private const string CSPLINE_HEADER = "~csplines~";


        public double[] StringOfDecimalsSeparatedBySpaceToArray(string numbers)
        {
            //double[] d= numbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            //                .Select(s => double.Parse(s))
            //                .ToArray();
            //return d;
            return numbers.Split(' ').Select(double.Parse).ToArray();
        }


        public static void doubleAccepted(object sender, KeyEventArgs e)
        {

            const int KEYCODE_Hyphen_OnKeyboard = 189;
            const int KEYCODE_Dot_OnKeyboard = 190;
            const int KEYCODE_Dot_OnNumericKeyPad = 110;

            e.Handled = !(
                (!( //No modifier key must be pressed
                     (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                  || (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
                  || (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt
                )
              && ( //only these keys are supported
                    (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                  || e.Key == Key.Subtract || e.Key == Key.Add || e.Key == Key.Unknown
                  || e.Key == Key.Home || e.Key == Key.End || e.Key == Key.Delete
                  || e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.Back
                  || (e.Key == Key.Unknown && (
                             e.PlatformKeyCode == KEYCODE_Hyphen_OnKeyboard
                          || e.PlatformKeyCode == KEYCODE_Dot_OnKeyboard || e.PlatformKeyCode == KEYCODE_Dot_OnNumericKeyPad
                        )
                     )
                 )
              )
            );
        }

        public static void intAccepted(object sender, KeyEventArgs e)
        {

            //const int KEYCODE_Hyphen_OnKeyboard = 189;


            e.Handled = !(
                (!( //No modifier key must be pressed
                     (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                  || (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
                  || (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt
                )
              && ( //only these keys are supported
                    (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                  || e.Key == Key.Subtract || e.Key == Key.Add || e.Key == Key.Unknown
                  || e.Key == Key.Home || e.Key == Key.End || e.Key == Key.Delete
                  || e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.Back
                //|| (e.Key == Key.Unknown && (
                //           e.PlatformKeyCode == KEYCODE_Hyphen_OnKeyboard
                //        || e.PlatformKeyCode == KEYCODE_Dot_OnKeyboard || e.PlatformKeyCode == KEYCODE_Dot_OnNumericKeyPad
                //     )
                // )
                 )
              )
            );
        }

        public void SaveSplinesToFile(StreamWriter saveWriter, List<BSpline> bSplines, List<HSpline> hSplines,
            List<CSpline> cSplines) 
        {
            //var bSplinesString = JsonConvert.SerializeObject(bSplines, Formatting.Indented);
            //var hSplinesString = JsonConvert.SerializeObject(hSplines, Formatting.Indented);

            var bSplinesDefinitions = new List<Tuple<int, List<double>, List<double>>>(bSplines.Count);
            var hSplinesDefinitions = new List<Tuple<List<double>, List<double>, double>>(hSplines.Count);
            var cSplinesDefinitions = new List<Tuple<List<double>, List<double>, List<double>>>(cSplines.Count);
            for (int i = 0; i < bSplines.Count; i++)
            {
                bSplinesDefinitions.Add(Tuple.Create(bSplines[i].Degree, bSplines[i].Knots,  bSplines[i].ControlPoints));
            }
            for (int i = 0; i < hSplines.Count; i++)
            {
                hSplinesDefinitions.Add(Tuple.Create(hSplines[i].Knots, hSplines[i].ControlPoints, hSplines[i].Derivation));
            }
            for (int i = 0; i < cSplines.Count; i++)
            {
                cSplinesDefinitions.Add(Tuple.Create(cSplines[i].Knots, cSplines[i].ControlPoints, cSplines[i].Derivations));
            }
            var bSplinesString = JsonConvert.SerializeObject(bSplinesDefinitions, Formatting.Indented);
            var hSplinesString = JsonConvert.SerializeObject(hSplinesDefinitions, Formatting.Indented);
            var cSplinesString = JsonConvert.SerializeObject(cSplinesDefinitions, Formatting.Indented);
            saveWriter.WriteLine(BSPLINE_HEADER);
            saveWriter.WriteLine(bSplinesString);
           // saveWriter.WriteLine(BSPLINE_HEADER);
            saveWriter.WriteLine(HSPLINE_HEADER);
            saveWriter.WriteLine(hSplinesString);
           // saveWriter.WriteLine(HSPLINE_HEADER);
            saveWriter.WriteLine(CSPLINE_HEADER);
            saveWriter.WriteLine(cSplinesString);
        }

        public void LoadSplinesFromFile(StreamReader reader, List<BSpline> _bSplineList, List<HSpline> _hermiteSplineList,
            List<CSpline> _clampedSplineList, Engine engine)
        {
            var bSplinesStringBuilder = new StringBuilder();
            var hSplinesStringBuilder = new StringBuilder();
            var cSplinesStringBuilder = new StringBuilder();

            var line = reader.ReadLine();
            if (line.Equals(BSPLINE_HEADER))
            {
                line = reader.ReadLine();
                while (!line.Equals(HSPLINE_HEADER))
                {                    
                    bSplinesStringBuilder.Append(line);
                    line = reader.ReadLine();
                }
                line = reader.ReadLine();
                while (!line.Equals(CSPLINE_HEADER))
                {
                    hSplinesStringBuilder.Append(line);
                    line = reader.ReadLine();
                }
                line = reader.ReadLine();
                while (line!=null)
                {
                    cSplinesStringBuilder.Append(line);
                    line = reader.ReadLine();
                }
                //System.Diagnostics.Debug.WriteLine(bSplinesStringBuilder.ToString());
               // System.Diagnostics.Debug.WriteLine(hSplinesStringBuilder.ToString());
                var bSplinesDefinitions = JsonConvert.DeserializeObject<List<Tuple<int, List<double>, List<double>>>>(bSplinesStringBuilder.ToString());
                var hSplinesDefinitions = JsonConvert.DeserializeObject<List<Tuple<List<double>, List<double>, double>>>(hSplinesStringBuilder.ToString());
                var cSplinesDefinitions = JsonConvert.DeserializeObject<List<Tuple<List<double>, List<double>, List<double>>>>(cSplinesStringBuilder.ToString());
                BSpline bSpline;
                for (int i = 0; i < bSplinesDefinitions.Count; i++)
                {
                    bSpline = engine.InteractiveBSpline(bSplinesDefinitions[i].Item1, bSplinesDefinitions[i].Item2.ToArray(), bSplinesDefinitions[i].Item3.ToArray(), true);
                    _bSplineList.Add(bSpline);
                }
                HSpline hSpline;
                for (int i = 0; i < hSplinesDefinitions.Count; i++)
                {
                    hSpline = engine.InteractiveHermiteSpline(hSplinesDefinitions[i].Item1.ToArray(),hSplinesDefinitions[i].Item2.ToArray(), hSplinesDefinitions[i].Item3);
                    _hermiteSplineList.Add(hSpline);
                }
                CSpline cSpline;
                for (int i = 0; i < hSplinesDefinitions.Count; i++)
                {
                    cSpline = engine.InteractiveClampedSpline(cSplinesDefinitions[i].Item1.ToArray(), cSplinesDefinitions[i].Item2.ToArray(), cSplinesDefinitions[i].Item3[0], cSplinesDefinitions[i].Item3[cSplinesDefinitions[i].Item3.Count-1]);
                    _clampedSplineList.Add(cSpline);
                }
            }
            
        }
    }
}
