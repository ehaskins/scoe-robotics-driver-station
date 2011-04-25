using System;
using System.Windows;
using System.Diagnostics;

namespace EHaskins.Frc.DSApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM _DataContext;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _DataContext = new MainVM();
                this.Closing += _DataContext.WindowClosing;
                this.DataContext = _DataContext;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
