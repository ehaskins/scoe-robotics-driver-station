using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace EHaskins.Frc.DSApp
{
    public class DriverStationVM : ViewModelBase
    {
        private EHaskins.Frc.Communication.DriverStation.DriverStation _DriverStation;
        public EHaskins.Frc.Communication.DriverStation.DriverStation DriverStation
        {
            get { return _DriverStation; }
            set
            {
                if (_DriverStation == value)
                    return;
                _DriverStation = value;
                if (_RobotConfigurator != null)
                    _RobotConfigurator.DriverStation = value;
                RaisePropertyChanged("DriverStation");
            }
        }


        private RobotConfiguratorVM _RobotConfigurator;
        public RobotConfiguratorVM RobotConfigurator
        {
            get { return _RobotConfigurator; }
            set
            {
                if (_RobotConfigurator == value)
                    return;
                _RobotConfigurator = value;
                _RobotConfigurator.DriverStation = DriverStation;
                RaisePropertyChanged("RobotConfigurator");
            }
        }
       

    }
}
