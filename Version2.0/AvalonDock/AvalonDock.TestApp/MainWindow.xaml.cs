using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;

namespace AvalonDock.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            Random rnd = new Random();
            timer.Interval = TimeSpan.FromSeconds(1.0);
            timer.Tick += (s, e) =>
                {
                    TestTimer++;

                    TestBackground = new SolidColorBrush(Color.FromRgb(
                        (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
                };
            timer.Start();

            this.DataContext = this;
        }

        #region TestTimer

        /// <summary>
        /// TestTimer Dependency Property
        /// </summary>
        public static readonly DependencyProperty TestTimerProperty =
            DependencyProperty.Register("TestTimer", typeof(int), typeof(MainWindow),
                new FrameworkPropertyMetadata((int)0));

        /// <summary>
        /// Gets or sets the TestTimer property.  This dependency property 
        /// indicates a test timer that elapses evry one second (just for binding test).
        /// </summary>
        public int TestTimer
        {
            get { return (int)GetValue(TestTimerProperty); }
            set { SetValue(TestTimerProperty, value); }
        }

        #endregion

        #region TestBackground

        /// <summary>
        /// TestBackground Dependency Property
        /// </summary>
        public static readonly DependencyProperty TestBackgroundProperty =
            DependencyProperty.Register("TestBackground", typeof(Brush), typeof(MainWindow),
                new FrameworkPropertyMetadata((Brush)null));

        /// <summary>
        /// Gets or sets the TestBackground property.  This dependency property 
        /// indicates a randomly changing brush (just for testing).
        /// </summary>
        public Brush TestBackground
        {
            get { return (Brush)GetValue(TestBackgroundProperty); }
            set { SetValue(TestBackgroundProperty, value); }
        }

        #endregion



    }
}
