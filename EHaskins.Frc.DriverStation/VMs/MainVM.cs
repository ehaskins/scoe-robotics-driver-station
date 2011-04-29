using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EHaskins.Frc.Communication;
using System.Net;
using System.ComponentModel;
using EHaskins.Frc.Communication.DriverStation;
using SlimDX.DirectInput;
using System.Windows;
using EHaskins.Utilities.Wpf;
using System.Xml.Linq;
using EHaskins.Frc.Communication.SaveState;
using Microsoft.Win32;
using System.IO;
using System.Deployment.Application;
namespace EHaskins.Frc.DSApp
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private const string FILE_DIALOG_FILTER = "FRC DS State (.dssave)|*.dssave|All files|*";
        public DelegateCommand EnableAllCommand { get; set; }
        public DelegateCommand DisableAllCommand { get; set; }
        public DelegateCommand EStopAllCommand { get; set; }
        public DelegateCommand TeleopAllCommand { get; set; }
        public DelegateCommand AutonomousAllCommand { get; set; }

        public DelegateCommand SaveStateCommand { get; set; }
        public DelegateCommand LoadStateCommand { get; set; }

        public void EnableAll(object sender)
        {
            foreach (var dsvm in DriverStations)
            {
                var ds = dsvm.DriverStation;
                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsEnabled = true;
            }
        }

        public void DisableAll(object sender)
        {
            foreach (var dsvm in DriverStations)
            {
                var ds = dsvm.DriverStation;
                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsEnabled = false;
            }
        }

        public void EStopAll(object sender)
        {
            foreach (var dsvm in DriverStations)
            {
                var ds = dsvm.DriverStation;
                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsEStop = true;
            }
        }

        public void TeleopAll(object sender)
        {
            foreach (var dsvm in DriverStations)
            {
                var ds = dsvm.DriverStation;
                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsAutonomous = false;
            }
        }
        public void AutonomousAll(object sender)
        {
            foreach (var dsvm in DriverStations)
            {
                var ds = dsvm.DriverStation;
                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsAutonomous = true;
            }
        }

        public void SaveState(object sender)
        {
            var sfd = new SaveFileDialog();
            sfd.DefaultExt = ".dssave";
            sfd.Filter = FILE_DIALOG_FILTER;

            sfd.AddExtension = true;
            var result = sfd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var path = sfd.FileName;
                DSManager.SaveState(path);
            }
        }


        private void SaveState()
        {
            DSManager.SaveState(configLocation);
        }
        public void LoadState(object sender)
        {
            var ofd = new OpenFileDialog();
            ofd.DefaultExt = ".dssave";
            ofd.Filter = FILE_DIALOG_FILTER;
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                DSManager.LoadState(ofd.FileName);
            }
        }


            
        const string configLocation = @".\config.dssave";

        public void LoadState()
        {
            if (File.Exists(configLocation))
            {
                DSManager.LoadState(configLocation);
            }
            else
            {
                LoadDefaultState();
            }
        }
        public void LoadDefaultState()
        {
            //var ds = new Communication.DriverStation.DriverStation() { TeamNumber = 1692, Connection = new UdpTransmitter() { Network = 172, Host = 198 } };
            //var ds = new Communication.DriverStation.DriverStation() { TeamNumber = 0, Connection = new UdpTransmitter() { Network = 127, Host = 1 } };
            //var ds = new DriverStation() { TeamNumber = 6, Connection = new UdpTransmitter() { ReceivePort = 1150, TransmitPort = 1110 } };

            var ds = new DriverStation() { TeamNumber = 6, Connection = new SerialTranceiver() { PortName = "COM4" } };

            var Joysticks = DSManager.JoystickManager.Joysticks;

            for (int i = 0; i < 4; i++)
            {
                var stick = i < Joysticks.Length ? new SlimDXJoystick(DSManager.JoystickManager, Joysticks[i].Information.InstanceName) : new SlimDXJoystick(DSManager.JoystickManager, "");
                stick.JoystickNumber = i;
                ds.Joysticks[i] = stick;
            }

            ds.Start();

            var ds2 = new DriverStation { TeamNumber = 6, Connection = new UdpTransmitter() { Host=3, TransmitPort = 2110, ReceivePort = 2150 } };
            for (int i = 0; i < 4; i++)
            {
                var stick = i < Joysticks.Length ? new SlimDXJoystick(DSManager.JoystickManager, Joysticks[i].Information.InstanceName) : new SlimDXJoystick(DSManager.JoystickManager, "");
                stick.JoystickNumber = i;
                ds2.Joysticks[i] = stick;
            }

            DSManager.AddDriverStation(ds);
            DSManager.AddDriverStation(ds2);

        }

        public MainVM()
        {
            Version = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : "Local install";
            EnableAllCommand = new DelegateCommand(EnableAll);
            DisableAllCommand = new DelegateCommand(DisableAll);
            EStopAllCommand = new DelegateCommand(EStopAll);
            TeleopAllCommand = new DelegateCommand(TeleopAll);
            AutonomousAllCommand = new DelegateCommand(AutonomousAll);
            SaveStateCommand = new DelegateCommand(SaveState);
            LoadStateCommand = new DelegateCommand(LoadState);

            Configuration.UserControlDataSize = 4;
            Configuration.UserStatusDataSize = 52;
            DSManager = new DSManager();
            DSManager.Start();
            //LoadState();
            LoadDefaultState();

            DriverStations = new ObservableCollection<DriverStationVM>();
            JoystickManager = DSManager.JoystickManager;
            foreach (var ds in DSManager.DriverStations)
            {
                DriverStations.Add(new DriverStationVM() { DriverStation = ds, RobotConfigurator = new RobotConfiguratorVM() });
            }
        }

        private JoystickManager _JoystickManager;
        public JoystickManager JoystickManager
        {
            get
            {
                return _JoystickManager;
            }
            set
            {
                if (_JoystickManager == value)
                    return;
                _JoystickManager = value;
                RaisePropertyChanged("JoystickManager");
            }
        }
        public ObservableCollection<DriverStationVM> DriverStations { get; set; }
        public DSManager DSManager { get; set; }
        public string Version { get; set; }
        public void WindowClosing(object sender, CancelEventArgs e)
        {
            SaveState();
            foreach (var dsvm in DriverStations)
            {
                var ds = dsvm.DriverStation;
                if (ds != null)
                {
                    if (ds.IsEnabled)
                        ds.Stop();
                    ds.Dispose();
                }
            }
            DSManager.Dispose();
        }

    }
}
