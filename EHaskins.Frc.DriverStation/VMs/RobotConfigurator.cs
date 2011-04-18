using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Utilities.Wpf;
using System.ComponentModel;
using System.Collections.ObjectModel;
using EHaskins.Frc.Communication.RobotConfig;
using System.Windows;
using EHaskins.Frc.Communication;
using System.Windows.Controls;
using System.Diagnostics;

namespace EHaskins.Frc.DriverStation
{
    class RobotConfigurator : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }


        public static readonly DependencyProperty DiscoverCommandProperty = DependencyProperty.Register("DiscoverCommand", typeof(DelegateCommand), typeof(RobotConfigurator), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDiscoverCommandChanged), new CoerceValueCallback(OnCoerceDiscoverCommand)));
        private static object OnCoerceDiscoverCommand(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceDiscoverCommand((DelegateCommand)value);
            else
                return value;
        }
        private static void OnDiscoverCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnDiscoverCommandChanged((DelegateCommand)e.OldValue, (DelegateCommand)e.NewValue);
        }
        protected virtual DelegateCommand OnCoerceDiscoverCommand(DelegateCommand value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnDiscoverCommandChanged(DelegateCommand oldValue, DelegateCommand newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public DelegateCommand DiscoverCommand
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DelegateCommand)GetValue(DiscoverCommandProperty);
            }
            set
            {
                SetValue(DiscoverCommandProperty, value);
            }
        }

        public static readonly DependencyProperty WriteConfigCommandProperty = DependencyProperty.Register("WriteConfigCommand", typeof(DelegateCommand), typeof(RobotConfigurator), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWriteConfigCommandChanged), new CoerceValueCallback(OnCoerceWriteConfigCommand)));
        private static object OnCoerceWriteConfigCommand(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceWriteConfigCommand((DelegateCommand)value);
            else
                return value;
        }
        private static void OnWriteConfigCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnWriteConfigCommandChanged((DelegateCommand)e.OldValue, (DelegateCommand)e.NewValue);
        }
        protected virtual DelegateCommand OnCoerceWriteConfigCommand(DelegateCommand value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnWriteConfigCommandChanged(DelegateCommand oldValue, DelegateCommand newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public DelegateCommand WriteConfigCommand
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DelegateCommand)GetValue(WriteConfigCommandProperty);
            }
            set
            {
                SetValue(WriteConfigCommandProperty, value);
            }
        }

        public static readonly DependencyProperty EraseConfigCommandProperty = DependencyProperty.Register("EraseConfigCommand", typeof(DelegateCommand), typeof(RobotConfigurator), new UIPropertyMetadata(null, new PropertyChangedCallback(OnEraseConfigCommandChanged), new CoerceValueCallback(OnCoerceEraseConfigCommand)));
        private static object OnCoerceEraseConfigCommand(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceEraseConfigCommand((DelegateCommand)value);
            else
                return value;
        }
        private static void OnEraseConfigCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnEraseConfigCommandChanged((DelegateCommand)e.OldValue, (DelegateCommand)e.NewValue);
        }
        protected virtual DelegateCommand OnCoerceEraseConfigCommand(DelegateCommand value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnEraseConfigCommandChanged(DelegateCommand oldValue, DelegateCommand newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public DelegateCommand EraseConfigCommand
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DelegateCommand)GetValue(EraseConfigCommandProperty);
            }
            set
            {
                SetValue(EraseConfigCommandProperty, value);
            }
        }
        

        public static readonly DependencyProperty DriverStationProperty = DependencyProperty.Register("DriverStation", typeof(EHaskins.Frc.Communication.DriverStation.DriverStation), typeof(RobotConfigurator), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDriverStationChanged), new CoerceValueCallback(OnCoerceDriverStation)));
        private static object OnCoerceDriverStation(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceDriverStation((EHaskins.Frc.Communication.DriverStation.DriverStation)value);
            else
                return value;
        }
        private static void OnDriverStationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnDriverStationChanged((EHaskins.Frc.Communication.DriverStation.DriverStation)e.OldValue, (EHaskins.Frc.Communication.DriverStation.DriverStation)e.NewValue);
        }
        protected virtual EHaskins.Frc.Communication.DriverStation.DriverStation OnCoerceDriverStation(EHaskins.Frc.Communication.DriverStation.DriverStation value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnDriverStationChanged(EHaskins.Frc.Communication.DriverStation.DriverStation oldValue, EHaskins.Frc.Communication.DriverStation.DriverStation newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public EHaskins.Frc.Communication.DriverStation.DriverStation DriverStation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (EHaskins.Frc.Communication.DriverStation.DriverStation)GetValue(DriverStationProperty);
            }
            set
            {
                SetValue(DriverStationProperty, value);
            }
        }

        public static readonly DependencyProperty RobotsProperty = DependencyProperty.Register("Robots", typeof(DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo>), typeof(RobotConfigurator), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRobotsChanged), new CoerceValueCallback(OnCoerceRobots)));
        private static object OnCoerceRobots(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceRobots((DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo>)value);
            else
                return value;
        }
        private static void OnRobotsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnRobotsChanged((DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo>)e.OldValue, (DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo>)e.NewValue);
        }
        protected virtual DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo> OnCoerceRobots(DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo> value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnRobotsChanged(DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo> oldValue, DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo> newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo> Robots
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo>)GetValue(RobotsProperty);
            }
            set
            {
                SetValue(RobotsProperty, value);
            }
        }


        public static readonly DependencyProperty SelectedRobotProperty = DependencyProperty.Register("SelectedRobot", typeof(RobotInfo), typeof(RobotConfigurator), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSelectedRobotChanged), new CoerceValueCallback(OnCoerceSelectedRobot)));
        private static object OnCoerceSelectedRobot(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceSelectedRobot((RobotInfo)value);
            else
                return value;
        }
        private static void OnSelectedRobotChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnSelectedRobotChanged((RobotInfo)e.OldValue, (RobotInfo)e.NewValue);
        }
        protected virtual RobotInfo OnCoerceSelectedRobot(RobotInfo value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnSelectedRobotChanged(RobotInfo oldValue, RobotInfo newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public RobotInfo SelectedRobot
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (RobotInfo)GetValue(SelectedRobotProperty);
            }
            set
            {
                SetValue(SelectedRobotProperty, value);
            }
        }

        public static readonly DependencyProperty MacAddressesProperty = DependencyProperty.Register("MacAddresses", typeof(ObservableCollection<byte[]>), typeof(RobotConfigurator), new UIPropertyMetadata(new ObservableCollection<byte[]>(), new PropertyChangedCallback(OnMacAddressesChanged), new CoerceValueCallback(OnCoerceMacAddresses)));
        private static object OnCoerceMacAddresses(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceMacAddresses((ObservableCollection<byte[]>)value);
            else
                return value;
        }
        private static void OnMacAddressesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnMacAddressesChanged((ObservableCollection<byte[]>)e.OldValue, (ObservableCollection<byte[]>)e.NewValue);
        }
        protected virtual ObservableCollection<byte[]> OnCoerceMacAddresses(ObservableCollection<byte[]> value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnMacAddressesChanged(ObservableCollection<byte[]> oldValue, ObservableCollection<byte[]> newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public ObservableCollection<byte[]> MacAddresses
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ObservableCollection<byte[]>)GetValue(MacAddressesProperty);
            }
            set
            {
                SetValue(MacAddressesProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedMacProperty = DependencyProperty.Register("SelectedMac", typeof(Byte[]), typeof(RobotConfigurator), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSelectedMacChanged), new CoerceValueCallback(OnCoerceSelectedMac)));
        private static object OnCoerceSelectedMac(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceSelectedMac((Byte[])value);
            else
                return value;
        }
        private static void OnSelectedMacChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnSelectedMacChanged((Byte[])e.OldValue, (Byte[])e.NewValue);
        }
        protected virtual Byte[] OnCoerceSelectedMac(Byte[] value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnSelectedMacChanged(Byte[] oldValue, Byte[] newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public Byte[] SelectedMac
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Byte[])GetValue(SelectedMacProperty);
            }
            set
            {
                SetValue(SelectedMacProperty, value);
            }
        }

        public static readonly DependencyProperty ResponsesProperty = DependencyProperty.Register("Responses", typeof(DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse>), typeof(RobotConfigurator), new UIPropertyMetadata(null, new PropertyChangedCallback(OnResponsesChanged), new CoerceValueCallback(OnCoerceResponses)));
        private static object OnCoerceResponses(DependencyObject o, object value)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                return robotConfigurator.OnCoerceResponses((DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse>)value);
            else
                return value;
        }
        private static void OnResponsesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RobotConfigurator robotConfigurator = o as RobotConfigurator;
            if (robotConfigurator != null)
                robotConfigurator.OnResponsesChanged((DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse>)e.OldValue, (DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse>)e.NewValue);
        }
        protected virtual DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse> OnCoerceResponses(DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse> value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        protected virtual void OnResponsesChanged(DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse> oldValue, DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse> newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        public DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse> Responses
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse>)GetValue(ResponsesProperty);
            }
            set
            {
                SetValue(ResponsesProperty, value);
            }
        }

        private EHaskins.Frc.Communication.RobotConfig.RobotConfigurator _configurator;
        public EHaskins.Frc.Communication.RobotConfig.RobotConfigurator Configurator
        {
            get
            {
                return _configurator;
            }
            set
            {
                _configurator = value;
            }
        }

        public RobotConfigurator()
        {
            DiscoverCommand = new DelegateCommand(Discover);
            WriteConfigCommand = new DelegateCommand(WriteConfig);
            EraseConfigCommand = new DelegateCommand(EraseConfig);
            MacAddresses.Add(new byte[] { 0x90, 0xA2, 0xDA, 0x00, 0X30, 0x98 });
            MacAddresses.Add(new byte[] { 0x90, 0xA2, 0xDA, 0x00, 0X36, 0x2A });
            SelectedMac = MacAddresses[0];
            Configurator = new Communication.RobotConfig.RobotConfigurator();
            Robots = new DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo>(Configurator.Robots, Dispatcher);
            Responses = new DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse>(Configurator.Responses, Dispatcher);
            this.Unloaded += This_Unloaded;
            Application.Current.Exit += Application_Exit;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            CleanUp();
        }
        private void This_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp();
        }
        private void CleanUp()
        {
            if (Configurator.IsOpen)
                Configurator.Close();
            Debug.WriteLine("Closed configurator");
        }
        public void Discover(object sender)
        {
            if (!Configurator.IsOpen)
                Configurator.Open();
            Configurator.BeginDiscovery();
        }

        public void WriteConfig(object sender)
        {
            if (!Configurator.IsOpen)
                Configurator.Open();
            if (SelectedRobot != null)
            {
                var udp = (UdpTransmitter)DriverStation.Connection;
                _configurator.BeginWrite(SelectedRobot.DeviceId, new RobotConfiguration()
                {
                    Team = DriverStation.TeamNumber,
                    ControlReceivePort = (ushort)udp.TransmitPort,
                    StatusTransmitPort = (ushort)udp.ReceivePort,
                    HostNumber=udp.Host,
                    Network = udp.Network,
                    SubnetMask = new byte[]{255, 0,0,0},
                    Mac = SelectedMac 
                });
            }
        }

        public void EraseConfig(object sender)
        {
            if (!Configurator.IsOpen)
                Configurator.Open();
             if (SelectedRobot != null)
                 _configurator.BeginErase(SelectedRobot.DeviceId);
        }
    }
}
