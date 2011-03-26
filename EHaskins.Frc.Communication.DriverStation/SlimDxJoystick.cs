using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using EHaskins.Utilities.NumericExtensions;
using System.Diagnostics;
using SlimDX.DirectInput;

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
                value = !Value;

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

    public class AxisData : INotifyPropertyChanged
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

        public AxisData(int index)
        {
            Index = index;
            PhysicalAxis = index;
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

    public class SlimDxJoystick : Joystick
    {
        Dispatcher dispatch;
        JoystickManager manager;
        public SlimDxJoystick(JoystickManager manager, String name, bool syncronize)
            : base()
        {
            this.dispatch = syncronize ? Dispatcher.CurrentDispatcher : null;
            this.Manager = manager;
            Name = name;

            AxisData = new AxisData[6];
            for (int i = 0; i < AxisData.Length; i++)
            {
                AxisData[i] = new AxisData(i);
            }

            ButtonData = new ButtonData[16];
            for (int i = 0; i < ButtonData.Length; i++)
            {
                ButtonData[i] = new ButtonData(i);
            }
        }
        public override void Update()
        {
            if (dispatch == null || dispatch.CheckAccess())
            {
                double[] physicalAxes = null;
                bool[] physicalButtons = null;
                if (Joystick != null)
                {
                    JoystickState state = null;
                    try
                    {
                        bool isAttached = Joystick.Acquire().IsSuccess && Joystick.Poll().IsSuccess;
                        if (isAttached && SlimDX.Result.Last.IsSuccess)
                        {
                            state = Joystick.GetCurrentState();
                            physicalButtons = state.GetButtons();
                        }
                    }   
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        //Joystick disconnected disable it.
                        Joystick = null;
                    }

                    if (state != null)
                    {
                        var sliders = state.GetSliders();
                        physicalAxes = new double[6 + sliders.Length];

                        physicalAxes[0] = state.X / 1000f;
                        physicalAxes[1] = state.Y / 1000f;
                        physicalAxes[2] = state.Z / 1000f;
                        physicalAxes[3] = state.RotationX / 1000f;
                        physicalAxes[4] = state.RotationY / 1000f;
                        physicalAxes[5] = state.RotationZ / 1000f;
                        for (int i = 0; i < sliders.Length; i++)
                        {
                            physicalAxes[6 + i] = sliders[i] / 1000f;
                        }
                    }
                }
                if (physicalAxes == null)
                    physicalAxes = new double[0];
                if (physicalButtons == null)
                    physicalButtons = new bool[0];

                foreach (ButtonData button in ButtonData)
                {
                    button.Update(physicalButtons);
                }

                foreach (AxisData axis in AxisData)
                {
                    axis.Update(physicalAxes);
                }

                for (int i = 0; i < AxisData.Length; i++)
                {
                    Axes[i] = AxisData[i].Value;
                }
                for (int button = 0; button < ButtonData.Length; button++)
                {
                    Buttons[button] = ButtonData[button].Value;
                }
            }
            else
            {
                try
                {
                    dispatch.Invoke((ThreadStart)delegate { Update(); }, TimeSpan.FromMilliseconds(5), System.Windows.Threading.DispatcherPriority.SystemIdle);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private ButtonData[] _ButtonData;
        public ButtonData[] ButtonData
        {
            get
            {
                return _ButtonData;
            }
            set
            {
                if (_ButtonData == value)
                    return;
                _ButtonData = value;
                RaisePropertyChanged("ButtonData");
            }
        }

        private AxisData[] _AxisData;
        public AxisData[] AxisData
        {
            get
            {
                return _AxisData;
            }
            set
            {
                if (_AxisData == value)
                    return;
                _AxisData = value;
                RaisePropertyChanged("AxisData");
            }
        }

        protected JoystickManager Manager
        {
            get
            {
                return manager;
            }
            set
            {
                if (manager == value)
                    return;
                manager = value;
                RaisePropertyChanged("Manager");
            }
        }
        public string Name
        {
            get
            {
                if (Joystick != null)
                    return Joystick.Information.InstanceName;
                else
                    return "Not connected";
            }
            set
            {
                Joystick = Manager.GetJoystick(value);
                RaisePropertyChanged("Name");
            }
        }


        private SlimDX.DirectInput.Joystick _Joystick;
        public SlimDX.DirectInput.Joystick Joystick
        {
            get
            {
                return _Joystick;
            }
            set
            {
                if (_Joystick == value)
                    return;
                _Joystick = value;
                RaisePropertyChanged("Joystick");
                RaisePropertyChanged("Name");
            }
        }
    }
}
