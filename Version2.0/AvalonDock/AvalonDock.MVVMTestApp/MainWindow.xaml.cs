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
using System.Collections.ObjectModel;

namespace AvalonDock.MVVMTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            this.DataContext = this;
        }

        ObservableCollection<string> _viewModelItems = new ObservableCollection<string>();

        public ObservableCollection<string> ViewModelItems
        {
            get { return _viewModelItems; }
        }

        ReadOnlyObservableCollection<string> _viewModelReadonlyItems = null;
        public ReadOnlyObservableCollection<string> ViewModelReadonlyItems
        {
            get {
                if (_viewModelReadonlyItems == null)
                    _viewModelReadonlyItems = new ReadOnlyObservableCollection<string>(_viewModelItems);

                return _viewModelReadonlyItems;
            }
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            _viewModelItems.Add(string.Format("Document {0} content", _viewModelItems.Count + 1));
        }

        private void OnRemove(object sender, RoutedEventArgs e)
        {
            if (dockManager.Layout.LastFocusedDocument != null)
                _viewModelItems.Remove((string)dockManager.Layout.LastFocusedDocument.Content);
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            _viewModelItems.Clear();
        }



    }
}
