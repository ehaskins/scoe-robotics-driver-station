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
using System.Windows.Threading;

namespace EHaskins.Frc.DSApp
{
    public class RobotConfiguratorVM : ViewModelBase
    {
        private DelegateCommand _DiscoverCommand;
        public DelegateCommand DiscoverCommand
        {
            get
            {
                return _DiscoverCommand;
            }
            set
            {
                if (_DiscoverCommand == value)
                    return;
                _DiscoverCommand = value;
                RaisePropertyChanged("DiscoverCommand");
            }
        }
        private DelegateCommand _WriteConfigCommand;
        public DelegateCommand WriteConfigCommand
        {
            get
            {
                return _WriteConfigCommand;
            }
            set
            {
                if (_WriteConfigCommand == value)
                    return;
                _WriteConfigCommand = value;
                RaisePropertyChanged("WriteConfigCommand");
            }
        }
        private DelegateCommand _EraseConfigCommand;
        public DelegateCommand EraseConfigCommand
        {
            get
            {
                return _EraseConfigCommand;
            }
            set
            {
                if (_EraseConfigCommand == value)
                    return;
                _EraseConfigCommand = value;
                RaisePropertyChanged("EraseConfigCommand");
            }
        }
        private EHaskins.Frc.Communication.DriverStation.DriverStation _DriverStation;
        public EHaskins.Frc.Communication.DriverStation.DriverStation DriverStation
        {
            get
            {
                return _DriverStation;
            }
            set
            {
                if (_DriverStation == value)
                    return;
                _DriverStation = value;
                RaisePropertyChanged("DriverStation");
            }
        }
        private DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo> _Robots;
        public DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo> Robots
        {
            get
            {
                return _Robots;
            }
            set
            {
                if (_Robots == value)
                    return;
                _Robots = value;
                RaisePropertyChanged("Robots");
            }
        }
        private RobotInfo _SelectedRobot;
        public RobotInfo SelectedRobot
        {
            get
            {
                return _SelectedRobot;
            }
            set
            {
                if (_SelectedRobot == value)
                    return;
                _SelectedRobot = value;
                RaisePropertyChanged("SelectedRobot");
            }
        }
        private ObservableCollection<byte[]> _MacAddresses;
        public ObservableCollection<byte[]> MacAddresses
        {
            get
            {
                return _MacAddresses;
            }
            set
            {
                if (_MacAddresses == value)
                    return;
                _MacAddresses = value;
                RaisePropertyChanged("MacAddresses");
            }
        }
        private Byte[] _SelectedMac;
        public Byte[] SelectedMac
        {
            get
            {
                return _SelectedMac;
            }
            set
            {
                if (_SelectedMac == value)
                    return;
                _SelectedMac = value;
                RaisePropertyChanged("SelectedMac");
            }
        }
        private DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse> _Responses;
        public DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse> Responses
        {
            get
            {
                return _Responses;
            }
            set
            {
                _Responses = value;
                RaisePropertyChanged("Responses");
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
                if (_configurator == value)
                    return;
                _configurator = value;
                RaisePropertyChanged("Configurator");
            }
        }

        public RobotConfiguratorVM()
        {
            DiscoverCommand = new DelegateCommand(Discover);
            WriteConfigCommand = new DelegateCommand(WriteConfig);
            EraseConfigCommand = new DelegateCommand(EraseConfig);
            MacAddresses = new ObservableCollection<byte[]>();
            MacAddresses.Add(new byte[] { 0x90, 0xA2, 0xDA, 0x00, 0X30, 0x98 });
            MacAddresses.Add(new byte[] { 0x90, 0xA2, 0xDA, 0x00, 0X36, 0x2A });
            SelectedMac = MacAddresses[0];
            Configurator = new Communication.RobotConfig.RobotConfigurator();
            Robots = new DispatchingCollection<ObservableCollection<RobotInfo>, RobotInfo>(Configurator.Robots, Dispatcher.CurrentDispatcher);
            Responses = new DispatchingCollection<ObservableCollection<RobotResponse>, RobotResponse>(Configurator.Responses, Dispatcher.CurrentDispatcher);
            Application.Current.Exit += Application_Exit;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            CleanUp();
        }
        public void Unloaded(object sender, RoutedEventArgs e)
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
                    HostNumber = udp.Host,
                    Network = udp.Network,
                    SubnetMask = new byte[] { 255, 0, 0, 0 },
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
