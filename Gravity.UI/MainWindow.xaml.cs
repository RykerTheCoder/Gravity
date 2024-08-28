using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Gravity.Logic.Models;

namespace Gravity.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GravityCalculator Calculator { get; set; }
        private double _time = 0;
        private Task SimulationTask { get; set; }
        public double Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Time cannot be negative");
                }
                else
                {
                    _time = value;
                }
            }
        }
        private enum State
        {
            Running, Paused, Stopped, Collided
        }
        private State SimulationState { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            CreateCalculator();
            Calculator.OnChange += On_BodyChanged;
        }
        private void Run()
        {
            while (SimulationState == State.Running)
            {
                DateTime start = DateTime.Now;
                Vector2 position = Calculator.CalculatePoint(Time, 4);
                string timeString = FormatTime(Time);
                string positionString = $"({position.X:N4} m, {position.Y:N4} m";
                string speedString = $"{Calculator.CurrentSpeed:N4} m/s";
                string accelerationString = $"{Calculator.CurrentAcceleration:N4} m/s/s";
                string distanceString = $"{Calculator.CurrentDistance:N2} m";
                DateTime end = DateTime.Now;

                double totalSeconds = (end - start).TotalSeconds;
                double tickRate = 1 / totalSeconds;
                if (tickRate > 60)
                {
                    Thread.Sleep((int)((1D / 60D - totalSeconds) * 1000));
                    tickRate = 60;
                }
                string fpsString = $"{tickRate:F0} fps";
                Time += 1 / tickRate;
                if (Time > Calculator.TimeOfCollision)
                {
                    SimulationState = State.Collided;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PauseButton.IsEnabled = false;
                        EndButton.IsEnabled = true;
                        EndButton.Content = "Reset";
                        RunButton.IsEnabled = false;
                        TimeSkipButton.IsEnabled = false;
                    });
                    Time = Calculator.TimeOfCollision;
                }

                Application.Current.Dispatcher.Invoke(() => UpdateGUI(timeString, positionString, speedString, accelerationString, distanceString, fpsString));

            }
        }
        private void UpdateGUI(string time, string position, string speed, string acceleration, string distance, string framerate)
        {
            TimeText.Text = time;
            PositionText.Text = position;
            SpeedText.Text = speed;
            AccelerationText.Text = acceleration;
            DistanceText.Text = distance;
            FramerateText.Text = framerate;
            TimeText.Text = time;
            PositionText.Text = position;
            SpeedText.Text = speed;
            AccelerationText.Text = acceleration;
            DistanceText.Text = distance;
            FramerateText.Text = framerate;
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
            InitialDistanceText.Text = DistanceText.Text = $"{Calculator.InitialDistance:N2} m";
            CollisionTimeText.Text = FormatTime(Calculator.TimeOfCollision);
            PositionText.Text = $"({Calculator.DynamicObject.CurrentPosition.X:N4} m, {Calculator.DynamicObject.CurrentPosition.Y:N4} m)";
            AccelerationText.Text = Calculator.CurrentAcceleration.ToString("N4") + " m/s/s";
        }
        public static string FormatTime(double seconds)
        {
            int mSeconds = (int)Math.Truncate(seconds * 1000); // convert seconds to milliseconds
            var span = new TimeSpan(0, 0, 0, 0, mSeconds); // create timespan object

            return $"{(span.Days != 0 ? span.Days.ToString("N0") + " days, " : "") + (span.Hours != 0 ? span.Hours.ToString("N0") + " hrs, " : "")}" +
                $"{(span.Minutes != 0 ? span.Minutes.ToString("N0") + " min, " : "") + (span.Seconds != 0 ? span.Seconds.ToString("N0") + " sec, " : "")}" +
                $"{span.Milliseconds.ToString("N0")} ms"; // return formatted string
        }
        private void On_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Calculator != null)
                {
                    var staticPosition = Calculator.StaticObject.InitialPosition;
                    var dynamicPosition = Calculator.DynamicObject.InitialPosition;
                    TextBox textBox = sender as TextBox;
                    var input = textBox.Text;

                    if (textBox.Text != "")
                    {
                        switch (textBox.Name)
                        {
                            case "StaticXBox":
                                staticPosition.X = float.Parse(input);
                                break;
                            case "StaticYBox":
                                staticPosition.Y = float.Parse(input);
                                break;
                            case "DynamicXBox":
                                dynamicPosition.X = float.Parse(input);
                                break;
                            case "DynamicYBox":
                                dynamicPosition.Y = float.Parse(input);
                                break;
                            case "StaticMassBox":
                                Calculator.StaticObject.Mass = double.Parse(input);
                                break;
                            case "DynamicMassBox":
                                Calculator.DynamicObject.Mass = double.Parse(input);
                                break;
                        }
                        Calculator.DynamicObject.InitialPosition = dynamicPosition;
                        Calculator.StaticObject.InitialPosition = staticPosition;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Invalid input in one or more fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void On_FocusLost(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            switch (textBox.Name)
            {
                case "StaticXBox":
                    StaticXBox.Text = Calculator.StaticObject.InitialPosition.X.ToString();
                    break;
                case "StaticYBox":
                    StaticYBox.Text = Calculator.StaticObject.InitialPosition.Y.ToString();
                    break;
                case "DynamicXBox":
                    DynamicXBox.Text = Calculator.DynamicObject.InitialPosition.X.ToString();
                    break;
                case "DynamicYBox":
                    DynamicYBox.Text = Calculator.DynamicObject.InitialPosition.Y.ToString();
                    break;
                case "StaticMassBox":
                    StaticMassBox.Text = Calculator.StaticObject.Mass.ToString();
                    break;
                case "DynamicMassBox":
                    DynamicMassBox.Text = Calculator.DynamicObject.Mass.ToString();
                    break;
                case "TimeSkipTextBox":
                    if (TimeSkipTextBox.Text == "")
                    {
                        TimeSkipTextBox.Text = "0";
                    }
                    break;
            }
        }
        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            SimulationState = State.Running;
            StaticXBox.IsEnabled = false;
            StaticYBox.IsEnabled = false;
            DynamicXBox.IsEnabled = false;
            DynamicYBox.IsEnabled = false;
            StaticMassBox.IsEnabled = false;
            DynamicMassBox.IsEnabled = false;
            PauseButton.IsEnabled = true;
            EndButton.IsEnabled = true;
            TimeSkipButton.IsEnabled = true;
            RunButton.IsEnabled = false;

            SimulationTask = Task.Run(() => Run());
            await SimulationTask;
        }
        private async void TimeSkipButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double addTime = 0;
                double timeInput = double.Parse(TimeSkipTextBox.Text);
                if (timeInput < 0)
                {
                    throw new ArgumentException("Time input cannot be negative");
                }
                switch ((TimeUnitCombo.SelectedItem as ComboBoxItem).Name)
                {
                    case "Milliseconds":
                        addTime = double.Parse(TimeSkipTextBox.Text) / 1000D;
                        break;
                    case "Seconds":
                        addTime = double.Parse(TimeSkipTextBox.Text);
                        break;
                    case "Minutes":
                        addTime = double.Parse(TimeSkipTextBox.Text) * 60;
                        break;
                    case "Hours":
                        addTime = double.Parse(TimeSkipTextBox.Text) * 60 * 60;
                        break;
                    case "Days":
                        addTime = double.Parse(TimeSkipTextBox.Text) * 24 * 60 * 60;
                        break;
                    case "Years":
                        addTime = double.Parse(TimeSkipTextBox.Text) * 365 * 24 * 60 * 60;
                        break;
                }
                if (addTime > Calculator.TimeOfCollision - Time)
                {
                    SimulationState = State.Collided;
                    await SimulationTask;
                    Time = Calculator.TimeOfCollision;
                    PauseButton.IsEnabled = false;
                    PauseButton.Content = "Pause";
                    RunButton.IsEnabled = false;
                    EndButton.IsEnabled = true;
                    EndButton.Content = "Reset";
                    TimeSkipButton.IsEnabled = false;
                }
                else
                {
                    Time += addTime;
                }
                Vector2 position = Calculator.CalculatePoint(Time, 4);
                string timeString = FormatTime(Time);
                string positionString = $"({position.X:N4} m, {position.Y:N4} m";
                string speedString = $"{Calculator.CurrentSpeed:N4} m/s";
                string accelerationString = $"{Calculator.CurrentAcceleration:N4} m/s/s";
                string distanceString = $"{Calculator.CurrentDistance:N2} m";
                UpdateGUI(timeString, positionString, speedString, accelerationString, distanceString, "0 fps");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private async void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SimulationState == State.Running)
            {
                SimulationState = State.Paused;
                PauseButton.Content = "Resume";
                FramerateText.Text = "0 fps";
            }
            else
            {
                SimulationState = State.Running;
                PauseButton.Content = "Pause";
                await SimulationTask;
                SimulationTask = Task.Run(() => Run());
            }
        }
        private async void EndButton_Click(object sender, RoutedEventArgs e)
        {
            EndButton.Content = "End";
            PauseButton.Content = "Pause";
            TimeSkipButton.IsEnabled = true;

            SimulationState = State.Stopped;
            if (SimulationTask != null)
            {
                await SimulationTask;
            }
            Time = 0;
            Vector2 position = Calculator.CalculatePoint(Time, 4);
            string timeString = FormatTime(Time);
            string positionString = $"({position.X:N4} m, {position.Y:N4} m";
            string speedString = $"{Calculator.CurrentSpeed:N4} m/s";
            string accelerationString = $"{Calculator.CurrentAcceleration:N4} m/s/s";
            string distanceString = $"{Calculator.CurrentDistance:N2} m";
            string fpsString = $"0 fps";

            UpdateGUI(timeString, positionString, speedString, accelerationString, distanceString, fpsString);

            StaticXBox.IsEnabled = true;
            StaticYBox.IsEnabled = true;
            DynamicXBox.IsEnabled = true;
            DynamicYBox.IsEnabled = true;
            StaticMassBox.IsEnabled = true;
            DynamicMassBox.IsEnabled = true;
            RunButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            EndButton.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EndButton_Click(this, new RoutedEventArgs());
        }
    }
}
