using System;
using System.ComponentModel;
using EHaskins.Utilities.NumericExtensions;
namespace EHaskins.Frc.Communication.RobotConfig
{
    public class RobotInfo : INotifyPropertyChanged
    {

        public RobotInfo(RobotResponse response)
        {
            this.Update(response);
        }
        public override string ToString()
        {
            return String.Format("{0} : {1}", DeviceId, MacAddress.ToMacString());
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private ushort _DeviceId;
        public ushort DeviceId
        {
            get { return _DeviceId; }
            private set
            {
                if (_DeviceId == value)
                    return;
                _DeviceId = value;
                RaisePropertyChanged("DeviceId");
            }
        }

        private byte[] _MacAddress;
        public byte[] MacAddress
        {
            get { return _MacAddress; }
            set
            {
                if (_MacAddress == value)
                    return;
                _MacAddress = value;
                RaisePropertyChanged("MacAddress");
            }
        }

        private void Update(RobotResponse response)
        {
            DeviceId = response.DeviceID;
            MacAddress = response.Payload;
        }

    }
}
