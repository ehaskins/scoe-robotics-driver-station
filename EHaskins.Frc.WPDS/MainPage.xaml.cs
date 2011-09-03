using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using EHaskins.Frc.Communication.DriverStation;
using EHaskins.Frc.Communication;

namespace EHaskins.Frc.WPDS
{
    public partial class MainPage : PhoneApplicationPage
    {
        DriverStation ds;
        DSTimer timer;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }
        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Communication.Configuration.UserControlDataSize = 64;
            Communication.Configuration.UserStatusDataSize = 64;
            timer = new DSThreadingTimer(20);

            ds = new DriverStation();
            ds.Connection = new UdpPhoneTransmitter { Network = 172, Host = 14, TransmitPort = 1110, ReceivePort = 1150 };
            ds.TeamNumber = 1692;
            ds.Interval = 1000;

            timer.AddDriverStation(ds);

            ds.Start();

            this.DataContext = ds;
        }
    }
}