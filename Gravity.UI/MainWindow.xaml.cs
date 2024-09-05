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
        private double pixelsPerMeter; // variable that represents the ratio of pixels to meters which is useful for rendering
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
            while (SimulationState == State.Running) // when the simulation is running
            {
                DateTime start = DateTime.Now; // get time before calculations and rendering
                Vector2 startPosition = Calculator.DynamicObject.CurrentPosition;
                Vector2 position = Calculator.CalculatePoint(Time, 4); // calculate the position of the object at the time
                // format a bunch of strings for the gui which represent their respective values
                Application.Current.Dispatcher.Invoke(() => RenderNext(startPosition));
                string timeString = FormatTime(Time);
                string positionString = $"({position.X:N4} m, {position.Y:N4} m)";
                string speedString = $"{Calculator.CurrentSpeed:N4} m/s";
                string accelerationString = $"{Calculator.CurrentAcceleration:N4} m/s/s";
                string distanceString = $"{Calculator.CurrentDistance:N2} m";
                DateTime end = DateTime.Now; // get time after calculations and rendering

                double totalSeconds = (end - start).TotalSeconds; // total time to do calculations in seconds
                double tickRate = 1 / totalSeconds; // tickrate aka. fps
                if (tickRate > 60) // cap the tickRate at 60
                {
                    Thread.Sleep((int)((1D / 60D - totalSeconds) * 1000));
                    tickRate = 60;
                }

                string fpsString = $"{tickRate:F0} fps"; // formatted fps string for display
                Time += 1 / tickRate;

                if (Time > Calculator.TimeOfCollision) // when the objects go past the collision point
                {
                    SimulationState = State.Collided;
                    // update the buttons in a logical way
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PauseButton.IsEnabled = false;
                        EndButton.IsEnabled = true;
                        EndButton.Content = "Reset";
                        RunButton.IsEnabled = false;
                        TimeSkipButton.IsEnabled = false;
                    });
                    Time = Calculator.TimeOfCollision;
                    // calculate and format strings
                    startPosition = position;
                    position = Calculator.CalculatePoint(Time, 4);
                    timeString = FormatTime(Time);
                    positionString = $"({position.X:N4} m, {position.Y:N4} m";
                    speedString = $"{Calculator.CurrentSpeed:N4} m/s";
                    accelerationString = $"{Calculator.CurrentAcceleration:N4} m/s/s";
                    distanceString = $"{Calculator.CurrentDistance:N2} m";
                    // update the GUI to reflect the changes
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PauseButton.IsEnabled = false;
                        EndButton.IsEnabled = true;
                        EndButton.Content = "Reset";
                        RunButton.IsEnabled = false;
                        TimeSkipButton.IsEnabled = false;
                        RenderNext(startPosition);
                    });
                }

                Application.Current.Dispatcher.Invoke(() => UpdateGUI(timeString, positionString, speedString, accelerationString, distanceString, fpsString)); // display new strings on gui

            }
        }
        private void UpdateGUI(string time, string position, string speed, string acceleration, string distance, string framerate)
        {
            //updates simulation status section of the gui
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
            //called whenever a property of a body gets changed

            //refresh the gui
            DirectionText.Text = $"{Calculator.Direction:F4}\u00B0";
            InitialDistanceText.Text = DistanceText.Text = $"{Calculator.InitialDistance:N2} m";
            CollisionTimeText.Text = FormatTime(Calculator.TimeOfCollision);
            PositionText.Text = $"({Calculator.DynamicObject.CurrentPosition.X:N4} m, {Calculator.DynamicObject.CurrentPosition.Y:N4} m)";
            AccelerationText.Text = Calculator.CurrentAcceleration.ToString("N4") + " m/s/s";
            RenderInitial();
        }
        public static string FormatTime(double time)
        {
            // formats a time value that is in seconds

            // calculations for time
            double years = Math.Truncate(time / 60D / 60D / 24D / 365D);
            double days = Math.Truncate((time / 60D / 60D / 24D - years * 365D));
            double hours = Math.Truncate(time / 60D / 60D - (years * 365D + days) * 24D);
            double minutes = Math.Truncate(time / 60D - (years * 24D * 365D + days * 24D + hours) * 60D);
            double seconds = Math.Truncate(time - (years * 60D * 24D * 365D + days * 60D * 24D + hours * 60D + minutes) * 60D);
            double milliseconds = Math.Truncate((time - years * 60D * 60D * 24D * 365D - days * 60D * 60D * 24D - hours * 60D * 60D - minutes * 60D - seconds) * 1000D);

            if (milliseconds < 0) // throw an exception whenever the accuracy of double makes milliseconds negative
            {
                throw new OverflowException("Time is too large.");
            }

            return $"{(years != 0 ? years.ToString("N0") + " years, " : "")}" +
                $"{(days != 0 ? days.ToString("N0") + " days, " : "") + (hours != 0 ? hours.ToString("N0") + " hrs, " : "")}" +
                $"{(minutes != 0 ? minutes.ToString("N0") + " min, " : "") + (seconds != 0 ? seconds.ToString("N0") + " sec, " : "")}" +
                $"{milliseconds.ToString("N0")} ms"; // return formatted string
        }
        private void On_TextChanged(object sender, TextChangedEventArgs e)
        {
            // initial value of the property we are changing
            double initialInput = 0;
            try
            {
                if (Calculator != null)
                {
                    var staticPosition = Calculator.StaticObject.InitialPosition;
                    var dynamicPosition = Calculator.DynamicObject.InitialPosition;
                    // get the textbox that sent the event as well as its text property
                    TextBox textBox = sender as TextBox;
                    var input = textBox.Text;

                    if (textBox.Text != "" && textBox.Text != "-")
                    {
                        // based on the name of the textbox, change different properties
                        switch (textBox.Name)
                        {
                            case "StaticXBox":
                                initialInput = staticPosition.X;
                                staticPosition.X = float.Parse(input);
                                break;
                            case "StaticYBox":
                                initialInput = staticPosition.Y;
                                staticPosition.Y = float.Parse(input);
                                break;
                            case "DynamicXBox":
                                initialInput = dynamicPosition.X;
                                dynamicPosition.X = float.Parse(input);
                                break;
                            case "DynamicYBox":
                                initialInput = dynamicPosition.Y;
                                dynamicPosition.Y = float.Parse(input);
                                break;
                            case "StaticMassBox":
                                initialInput = Calculator.StaticObject.Mass;
                                Calculator.StaticObject.Mass = double.Parse(input);
                                break;
                            case "DynamicMassBox":
                                initialInput = Calculator.DynamicObject.Mass;
                                Calculator.DynamicObject.Mass = double.Parse(input);
                                break;
                        }
                        Calculator.DynamicObject.InitialPosition = dynamicPosition;
                        Calculator.StaticObject.InitialPosition = staticPosition;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is OverflowException) // if the time of collision was too large
                {
                    var staticPosition = Calculator.StaticObject.InitialPosition;
                    var dynamicPosition = Calculator.DynamicObject.InitialPosition;

                    switch ((sender as TextBox).Name) // set the property to the value before this event was called
                    {
                        case "StaticXBox":
                            staticPosition.X = (float)initialInput;
                            break;
                        case "StaticYBox":
                            staticPosition.Y = (float)initialInput;
                            break;
                        case "DynamicXBox":
                            dynamicPosition.X = (float)initialInput;
                            break;
                        case "DynamicYBox":
                            dynamicPosition.Y = (float)initialInput;
                            break;
                        case "StaticMassBox":
                            Calculator.StaticObject.Mass = initialInput;
                            break;
                        case "DynamicMassBox":
                            Calculator.DynamicObject.Mass = initialInput;
                            break;
                    }
                    Calculator.DynamicObject.InitialPosition = dynamicPosition;
                    Calculator.StaticObject.InitialPosition = staticPosition;
                }

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void On_FocusLost(object sender, KeyboardFocusChangedEventArgs e)
        {
            // called to format the string after the user is done typing
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
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            SimulationState = State.Running;

            // enable/disable buttons logically
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

            SimulationTask = Task.Run(() => Run()); // cpu does calculations
        }
        private async void TimeSkipButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double addTime = 0; // time to be added to the current time
                double timeInput = double.Parse(TimeSkipTextBox.Text);

                if (timeInput < 0)
                {
                    throw new ArgumentException("Time input cannot be negative");
                }
                switch ((TimeUnitCombo.SelectedItem as ComboBoxItem).Name) // change the value of add time depending on what unit the user picked
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
                    // if the added time will bring the time over the time of collision, then go into the collided state and set the time to be the time of collision
                    SimulationState = State.Collided;
                    if (SimulationTask != null)
                    {
                        await SimulationTask; // wait for simulation to stop running
                    }
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
                // update the gui to reflect the changes
                Vector2 startPosition = Calculator.DynamicObject.CurrentPosition;
                Vector2 position = Calculator.CalculatePoint(Time, 4);
                string timeString = FormatTime(Time);
                string positionString = $"({position.X:N4} m, {position.Y:N4} m";
                string speedString = $"{Calculator.CurrentSpeed:N4} m/s";
                string accelerationString = $"{Calculator.CurrentAcceleration:N4} m/s/s";
                string distanceString = $"{Calculator.CurrentDistance:N2} m";
                UpdateGUI(timeString, positionString, speedString, accelerationString, distanceString, "0 fps");
                RenderNext(startPosition);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private async void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            // pause button has 2 functions: pausing when the simulation is running, and resuming when the simulation is paused
            if (SimulationState == State.Running)
            {
                SimulationState = State.Paused;
                PauseButton.Content = "Resume";
                await SimulationTask;
                FramerateText.Text = "0 fps";
            }
            else
            {
                SimulationState = State.Running;
                PauseButton.Content = "Pause";
                SimulationTask = Task.Run(() => Run()); // re run the simulation
            }
        }
        private async void EndButton_Click(object sender, RoutedEventArgs e)
        {
            EndButton.Content = "End";
            PauseButton.Content = "Pause";
            TimeSkipButton.IsEnabled = true;

            SimulationState = State.Stopped;
            if (SimulationTask != null) // when the simulation is running, wait for it to stop
            {
                await SimulationTask;
            }
            Time = 0; // reset the time
            // change gui based on the new time
            Vector2 position = Calculator.CalculatePoint(Time, 4);
            string timeString = FormatTime(Time);
            string positionString = $"({position.X:N4} m, {position.Y:N4} m)";
            string speedString = $"{Calculator.CurrentSpeed:N4} m/s";
            string accelerationString = $"{Calculator.CurrentAcceleration:N4} m/s/s";
            string distanceString = $"{Calculator.CurrentDistance:N2} m";
            string fpsString = $"0 fps";

            UpdateGUI(timeString, positionString, speedString, accelerationString, distanceString, fpsString);
            RenderInitial();

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
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // if the window closes, then make sure the simulation ends if it is running
            SimulationState = State.Stopped;
            if (SimulationTask != null)
            {
                await SimulationTask;
            }
        }
        private void RenderInitial()
        {
            // renders the initial frame of the simulation
            if (GravityCanvas.IsLoaded)
            {
                GravityCanvas.Children.Clear();
                double height = GravityCanvas.ActualHeight;
                double width = GravityCanvas.ActualWidth;
                double margin = 10; // the distance between the frame and the bodies
                double angle = -Calculator.Direction * Math.PI / 180D; // convert to radians
                // the bodies will be circles
                Ellipse dynamicBodyCircle = new Ellipse();
                Ellipse staticBodyCircle = new Ellipse();
                dynamicBodyCircle.Width = width * 0.05; // the circles will be 0.05x the width of the frame
                dynamicBodyCircle.Height = width * 0.05;
                dynamicBodyCircle.Fill = Brushes.Orange;
                dynamicBodyCircle.HorizontalAlignment = HorizontalAlignment.Left;
                dynamicBodyCircle.VerticalAlignment = VerticalAlignment.Bottom;
                staticBodyCircle.Width = width * 0.05;
                staticBodyCircle.Height = width * 0.05;
                staticBodyCircle.Fill = Brushes.SkyBlue;
                staticBodyCircle.HorizontalAlignment = HorizontalAlignment.Left;
                staticBodyCircle.VerticalAlignment = VerticalAlignment.Bottom;

                // adjusting the height and width to account for the body sizes and the margin
                height = height - width * 0.05 - 2D * margin;
                width = width - width * 0.05 - 2D * margin;

                // 2 terms that are repeatedly used
                double arcTan = Math.Atan(height / width);
                double sign = Math.Sign(WaveThingy(angle, height, width)) == 0 ? 1 : Math.Sign(WaveThingy(angle, height, width));

                // these do math stuff that would take a lot of explaining
                double y = Math.Sign(angle) * (sign == -1 ? height / 2D : width / 2D * Math.Pow(Math.Tan(2 * arcTan * Math.Acos(Math.Abs(WaveThingy(angle, height, width))) / Math.PI), sign));
                double x = Math.Sign(Math.Cos(angle)) * (sign == 1 ? width / 2D : height / 2D * Math.Pow(Math.Tan((Math.PI - 2 * arcTan) * Math.Acos(Math.Abs(WaveThingy(angle, height, width))) / Math.PI), -sign));

                // position vectors to represent the positions of both bodies in pixels (they account for the fact that they have sizes and the margin) 
                Vector2 staticBodyPos = new Vector2((float)(width / 2D - x + margin), (float)(height / 2D - y + margin));
                Vector2 dynamicBodyPos = new Vector2((float)(width / 2D + x + margin), (float)(height / 2D + y + margin));
                pixelsPerMeter = Vector2.Distance(staticBodyPos, dynamicBodyPos) / Calculator.InitialDistance; // pixels divided by meters gives pixels per meter

                // the transformations of the bodies
                staticBodyCircle.RenderTransform = new TranslateTransform(staticBodyPos.X, staticBodyPos.Y);
                dynamicBodyCircle.RenderTransform = new TranslateTransform(dynamicBodyPos.X, dynamicBodyPos.Y);
                
                // add the bodies to the canvas
                GravityCanvas.Children.Add(dynamicBodyCircle);
                GravityCanvas.Children.Add(staticBodyCircle);
            }
        }
        private void RenderNext(Vector2 startPosition)
        {
            // renders each subsequent frame of the simulation
            foreach (var element in GravityCanvas.Children)
            {
                Ellipse ellipse = element as Ellipse;
                if (ellipse.Fill == Brushes.Orange) // check if it is the dynamic body
                {
                    // transform the dynamic body visually on the screen
                    Vector2 newPosition = Calculator.DynamicObject.CurrentPosition;
                    Vector2 displacement = new Vector2(newPosition.X - startPosition.X, startPosition.Y - newPosition.Y); // had to switch the signs because OffsetY is aligned to the top
                    Matrix initialTransform = ellipse.RenderTransform.Value;
                    initialTransform.OffsetX += displacement.X * pixelsPerMeter;
                    initialTransform.OffsetY += displacement.Y * pixelsPerMeter;
                    ellipse.RenderTransform = new TranslateTransform(initialTransform.OffsetX, initialTransform.OffsetY);
                }
            }
        }
        private static double WaveThingy(double angle, double height, double width)
        {
            // this represents a mathematical piecewise function that is like a multipurpose for a rectangle, it is weird but it works
            double arcTan = Math.Atan(height / width);
            double absAngle = Math.Abs(angle);

            if (absAngle <= arcTan)
            {
                return Math.Cos(Math.PI * absAngle / (2D * arcTan));
            }
            else if (absAngle <= Math.PI - arcTan)
            {
                return Math.Cos(Math.PI * (absAngle - 3D * Math.PI / 2D + 2D * arcTan) / (Math.PI - 2D * arcTan));
            }
            else if (absAngle <= Math.PI + arcTan)
            {
                return Math.Cos(Math.PI * (absAngle - Math.PI) / (2D * arcTan));
            }
            else if (absAngle <= 2D * Math.PI - arcTan)
            {
                return Math.Cos(Math.PI * (absAngle - 3D * (3D * Math.PI / 2D - 2D * arcTan)) / (Math.PI - 2D * arcTan));
            }
            else
            {
                return Math.Cos(Math.PI * (absAngle - 2D * Math.PI) / (2D * arcTan));
            }
        }
        private void GravityCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // creates the first frame to exist
            RenderInitial();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(SimulationState == State.Stopped) // when the simulation is stopped, the simulation will resize when the window is resized
            {
                RenderInitial();
            }
        }
    }
}
