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
namespace EHaskins.Frc.DriverStation
{
    public class DriverStationVM : INotifyPropertyChanged
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

        public JoystickManager JoystickManager { get; set; }

        public void EnableAll(object sender)
        {
            foreach (Communication.DriverStation.DriverStation ds in DriverStations)
            {
                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsEnabled = true;
            }
        }

        public void DisableAll(object sender)
        {
            foreach (Communication.DriverStation.DriverStation ds in DriverStations)
            {

                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsEnabled = false;
            }
        }

        public void EStopAll(object sender)
        {
            foreach (Communication.DriverStation.DriverStation ds in DriverStations)
            {

                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsEStop = true;
            }
        }

        public void TeleopAll(object sender)
        {
            foreach (Communication.DriverStation.DriverStation ds in DriverStations)
            {

                if (ds.ControlData != null)
                    ds.ControlData.Mode.IsAutonomous = false;
            }
        }
        public void AutonomousAll(object sender)
        {
            foreach (Communication.DriverStation.DriverStation ds in DriverStations)
            {

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
                try
                {
                    var data = new XElement("DriverStation");
                    data.Add((from ds in DriverStations select ds.GetState()));
                    var doc = new XDocument();
                    doc.Add(data);
                    File.WriteAllText(sfd.FileName, doc.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving state: " + ex.Message);
                }
            }
        }

        public void LoadState(object sender)
        {
            var ofd = new OpenFileDialog();
            ofd.DefaultExt = ".dssave";
            ofd.Filter = FILE_DIALOG_FILTER;
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    var data = XDocument.Parse(File.ReadAllText(ofd.FileName));
                    var i = 0;
                    var ds = data.Element("DriverStation");
                    var dsstates = ds.Elements("DSState").ToArray();

                    foreach (var dsdata in dsstates)
                    {
                        if (DriverStations.Count > i)
                        {
                            DriverStations[i].LoadState(dsdata, JoystickManager);
                            i++;
                        }
                    }
                    //DriverStations[0].LoadState(dsstates[0], JoystickManager);
                    //RaisePropertyChanged("DriverStations");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading state: " + ex.Message);
                }
            }
        }
        public DriverStationVM()
        {
            EnableAllCommand = new DelegateCommand(EnableAll);
            DisableAllCommand = new DelegateCommand(DisableAll);
            EStopAllCommand = new DelegateCommand(EStopAll);
            TeleopAllCommand = new DelegateCommand(TeleopAll);
            AutonomousAllCommand = new DelegateCommand(AutonomousAll);
            SaveStateCommand = new DelegateCommand(SaveState);
            LoadStateCommand = new DelegateCommand(LoadState);

            JoystickManager = new JoystickManager();
            Configuration.UserControlDataSize = 101;
            Configuration.UserStatusDataSize = 152;
            DriverStations = new ObservableCollection<Communication.DriverStation.DriverStation>();

            //var ds = new Communication.DriverStation.DriverStation() { TeamNumber = 1692, Connection = new UdpTransmitter() { Network = 172, Host = 198 } };
            //var ds = new Communication.DriverStation.DriverStation() { TeamNumber = 0, Connection = new UdpTransmitter() { Network = 127, Host = 1 } };
            var ds = new Communication.DriverStation.DriverStation() { TeamNumber = 1103, Connection = new UdpTransmitter() { Network = 10, Host = 12, ReceivePort = 1150, TransmitPort = 1110 } };
            var Joysticks = JoystickManager.Joysticks;

            for (int i = 0; i < 4; i++)
            {
                var stick = i < Joysticks.Length ? new SlimDXJoystick(JoystickManager, Joysticks[i].Information.InstanceName) : new SlimDXJoystick(JoystickManager, "");
                stick.JoystickNumber = i;
                ds.Joysticks[i] = stick;
            }

            ds.Started += new EventHandler(DriverStationStarted);
            ds.Start();
            DriverStations.Add(ds);

            var ds2 = new Communication.DriverStation.DriverStation { TeamNumber = 1103, Connection = new UdpTransmitter() { TransmitPort = 2110, ReceivePort = 2150 } };
            for (int i = 0; i < 4; i++)
            {
                var stick = i < Joysticks.Length ? new SlimDXJoystick(JoystickManager, Joysticks[i].Information.InstanceName) : new SlimDXJoystick(JoystickManager, "");
                stick.JoystickNumber = i;
                ds2.Joysticks[i] = stick;
            }
            //ds2.Start();
            DriverStations.Add(ds2);
        }

        public ObservableCollection<Communication.DriverStation.DriverStation> DriverStations { get; set; }

        private void DriverStationStarted(object sender, EventArgs e)
        {
            var ds = sender as Communication.DriverStation.DriverStation;
            ds.ControlData.Mode.IsAutonomous = false;
        }
        public void WindowClosing(object sender, CancelEventArgs e)
        {
            foreach (var ds in DriverStations)
            {
                if (ds != null)
                {
                    if (ds.IsEnabled)
                        ds.Stop();
                    ds.Dispose();
                }
            }
            JoystickManager.Dispose();
        }

    }
}
