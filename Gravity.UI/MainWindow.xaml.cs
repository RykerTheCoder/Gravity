using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gravity.Logic.Models;

namespace Gravity.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GravityCalculator Calculator { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            CreateCalculator();
            Calculator.OnChange += On_BodyChanged;
        }
        private void Update()
        {

        }
        private void CreateCalculator()
        {
            try
            {
                // create a calculator object
                Vector2 dynamicPosition = new Vector2(float.Parse(DynamicXBox.Text), float.Parse(DynamicYBox.Text));
                Vector2 staticPosition = new Vector2(float.Parse(StaticXBox.Text), float.Parse(StaticYBox.Text));
                DynamicBody dynamicBody = new DynamicBody(double.Parse(DynamicMassBox.Text), dynamicPosition);
                StaticBody staticBody = new StaticBody(double.Parse(StaticMassBox.Text), staticPosition);
                Calculator = new GravityCalculator(dynamicBody, staticBody);

                On_BodyChanged(this, EventArgs.Empty); // set text blocks to the properties

            }
            catch
            {
                MessageBox.Show("Something went wrong. Maybe you used an invalid input?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void On_BodyChanged(object sender, EventArgs e)
        {

            DirectionText.Text = $"{Calculator.Direction:F4}\u00B0";
            InitialDistanceText.Text = $"{Calculator.InitialDistance:F4} m";
            CollisionTimeText.Text = FormatTime(Calculator.TimeOfCollision);
        }
        public static string FormatTime(double seconds)
        {
            int mSeconds = (int)Math.Truncate(seconds * 1000); // convert seconds to milliseconds
            var span = new TimeSpan(0, 0, 0, 0, mSeconds); // create timespan object

            return $"{(span.Days != 0 ? span.Days + " days, " : "") + (span.Hours != 0 ? span.Hours + " hrs, " : "")}" +
                $"{(span.Minutes != 0 ? span.Minutes + " min, " : "") + (span.Seconds != 0 ? span.Seconds + " sec, " : "")}" +
                $"{span.Milliseconds} ms"; // return formatted string
        }

        private void On_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Calculator != null)
            {
                var staticPosition = Calculator.StaticObject.InitialPosition;
                var dynamicPosition = Calculator.DynamicObject.InitialPosition;
                TextBox textBox = sender as TextBox;

                switch (textBox.Name)
                {
                    case "StaticXBox":
                        staticPosition.X = float.Parse(textBox.Text);
                        break;
                    case "StaticYBox":
                        staticPosition.Y = float.Parse(textBox.Text);
                        break;
                    case "DynamicXBox":
                        dynamicPosition.X = float.Parse(textBox.Text);
                        break;
                    case "DynamicYBox":
                        dynamicPosition.Y = float.Parse(textBox.Text);
                        break;
                    case "StaticMassBox":
                        Calculator.StaticObject.Mass = double.Parse(textBox.Text);
                        break;
                    case "DynamicMassBox":
                        Calculator.DynamicObject.Mass = double.Parse(textBox.Text);
                        break;
                }
                Calculator.DynamicObject.InitialPosition = dynamicPosition;
                Calculator.StaticObject.InitialPosition = staticPosition;
            }
        }
    }
}
