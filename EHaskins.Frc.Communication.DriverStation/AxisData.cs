using System;
using System.ComponentModel;
using EHaskins.Utilities.NumericExtensions;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class AxisData : INotifyPropertyChanged
    {


        public AxisData(int physicalAxis)
        {
            PhysicalAxis = physicalAxis;
        }

        public void Update(double[] physicalValues)
        {
            double value;
            if (PhysicalAxis >= physicalValues.Length)
                value = 0;
            else
                value = physicalValues[PhysicalAxis];

            value = value.Deadband(0.0, 1.0, Deadband);

            if (Invert)
                value = -value;
            Value = value;
        }

        private double _Value;
        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value == value)
                    return;
                _Value = value;
                RaisePropertyChanged("Value");
            }
        }

        private double _DeadBand;
        public double Deadband
        {
            get { return _DeadBand; }
            set
            {
                if (_DeadBand == value)
                    return;
                _DeadBand = value;
                RaisePropertyChanged("Deadband");
            }
        }

        private bool _Invert;
        public bool Invert
        {
            get
            {
                return _Invert;
            }
            set
            {
                if (_Invert == value)
                    return;
                _Invert = value;
                RaisePropertyChanged("Invert");
            }
        }

        private int _PhysicalAxis;
        public int PhysicalAxis
        {
            get
            {
                return _PhysicalAxis;
            }
            set
            {
                if (_PhysicalAxis == value)
                    return;
                _PhysicalAxis = value;
                RaisePropertyChanged("PhysicalAxis");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
