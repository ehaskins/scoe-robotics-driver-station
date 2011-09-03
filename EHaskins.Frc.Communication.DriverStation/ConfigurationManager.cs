using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EHaskins.Utilities;
using System.Collections.ObjectModel;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class ConfigurationInstanceMonager : NotificationObject
    {
        public ConfigurationInstanceMonager(ConfigurationManager parent)
        {

        }


    }
    public class ConfigurationManager : NotificationObject
    {
        public void LoadConfigurations(string path)
        {

        }

        public void SaveConfigurations(string path)
        {

        }

        private ObservableCollection<DSConfiguration> _Configurations;
        public ObservableCollection<DSConfiguration> Configurations
        {
            get { return _Configurations; }
            set
            {
                if (_Configurations == value)
                    return;
                _Configurations = value;
                RaisePropertyChanged("Configurations");
            }
        }

        public List<ConfigurationInstanceMonager> LastConfiguration{get; set;}
    }

    public class DSConfiguration : NotificationObject
    {
        private XElement _StateData;
        public XElement StateData
        {
            get { return _StateData; }
            set
            {
                if (_StateData == value)
                    return;
                _StateData = value;
                RaisePropertyChanged("StateData");
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value)
                    return;
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }
        

        private string _Notes;
        public string Notes
        {
            get { return _Notes; }
            set
            {
                if (_Notes == value)
                    return;
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        
        
    }
}
