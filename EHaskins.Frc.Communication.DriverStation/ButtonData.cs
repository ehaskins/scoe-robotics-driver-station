using System;
using System.ComponentModel;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class ButtonData : INotifyPropertyChanged
    {
        private int _Index;
        public int Index
        {
            get { return _Index; }
            protected set
            {
                if (_Index == value)
                    return;
                _Index = value;
                RaisePropertyChanged("Index");
            }
        }

        public ButtonData(int index)
        {
            Index = index;
            PhysicalButton = index;
        }


        public void Update(bool[] physicalValues)
        {
            bool value;
            if (PhysicalButton >= physicalValues.Length)
                value = false;
            else
                value = physicalValues[PhysicalButton];
            if (Invert)
                value = !value;

            Value = value;
        }

        private bool _Value;
        public bool Value
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

        private int _PhysicalButton;
        public int PhysicalButton
        {
            get
            {
                return _PhysicalButton;
            }
            set
            {
                if (_PhysicalButton == value)
                    return;
                _PhysicalButton = value;
                RaisePropertyChanged("PhysicalButton");
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
