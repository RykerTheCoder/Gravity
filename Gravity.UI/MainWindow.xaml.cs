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
        }
        private void Update()
        {

        }
        private void CreateCalculator()
        {
            try
            {
                Vector2 dynamicPosition = new Vector2(float.Parse(DynamicXBox.Text), float.Parse(DynamicYBox.Text));
                Vector2 staticPosition = new Vector2(float.Parse(StaticXBox.Text), float.Parse(StaticYBox.Text));
                DynamicBody dynamicBody = new DynamicBody(double.Parse(DynamicMassBox.Text), dynamicPosition);
                StaticBody staticBody = new StaticBody(double.Parse(StaticMassBox.Text), staticPosition);
                Calculator = new GravityCalculator(dynamicBody, staticBody);

                DirectionText.Text = $"{Calculator.Direction:F4}";
                InitialDistanceText.Text = $"{Calculator.InitialDistance:F4}";
                CollisionTimeText.Text = $"{Calculator.TimeOfCollision:F4}";

            }
            catch
            {
                MessageBox.Show("Something went wrong. Maybe you used an invalid input?", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
