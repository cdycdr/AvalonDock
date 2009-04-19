using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

namespace AvalonDockTest
{
    /// <summary>
    /// Interaction logic for Window5.xaml
    /// </summary>
    public partial class Window5 : Window
    {
        public Window5()
        {
            InitializeComponent();
        }

        private void SaveLayout_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
     + @"\AvalonDockTest.Layout.xml";

            //_dockingManager.SaveLayout(path);

        }
    }
}
