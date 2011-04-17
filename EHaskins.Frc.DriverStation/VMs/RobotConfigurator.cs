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

namespace EHaskins.Frc.DriverStation
{
    class RobotConfigurator : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public EHaskins.Frc.Communication.DriverStation.DriverStation DriverStation { get; set; }

        public DelegateCommand DiscoverCommand { get; set; }
        public DelegateCommand WriteConfigCommand { get; set; }
        public DelegateCommand EraseConfigCommand { get; set; }
        private ObservableCollection<RobotInfo> _Robots;
        public ObservableCollection<RobotInfo> Robots
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
        private byte[] _SelectedMac;
        public byte[] SelectedMac
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

        private EHaskins.Frc.Communication.RobotConfig.RobotConfigurator _configurator;
        public RobotConfigurator()
        {
            DiscoverCommand = new DelegateCommand(Discover);
            WriteConfigCommand = new DelegateCommand(WriteConfig);
            EraseConfigCommand = new DelegateCommand(EraseConfig);
            MacAddresses = new ObservableCollection<byte[]>();
            MacAddresses.Add(new byte[] { 0x90, 0xA2, 0xDA, 0x00, 0X30, 0x98 });
            MacAddresses.Add(new byte[] { 0x90, 0xA2, 0xDA, 0x00, 0X36, 0x2A });
            SelectedMac = MacAddresses[0];

            _configurator = new Communication.RobotConfig.RobotConfigurator();
            Robots = _configurator.Robots;
        }

        public void Discover(object sender)
        {
            _configurator.BeginDiscovery();
        }

        public void WriteConfig(object sender)
        {
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
             if (SelectedRobot != null)
                 _configurator.BeginErase(SelectedRobot.DeviceId);
        }
    }
}
