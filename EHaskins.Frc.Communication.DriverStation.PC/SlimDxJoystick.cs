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
    public class SlimDXJoystick : Joystick
    {
        Dispatcher dispatch;
        JoystickManager manager;
        public SlimDXJoystick(JoystickManager manager, String name)
            : base()
        {
            //this.dispatch = syncronize ? Dispatcher.CurrentDispatcher : null;
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
            //if (dispatch == null || dispatch.CheckAccess())
            //{
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
                    //Joystick = null;
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

            PhysicalAxes = physicalAxes.Length;
            PhysicalButtons = physicalButtons.Length;

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
            //}
            //else
            //{
            //    try
            //    {
            //        dispatch.Invoke((ThreadStart)delegate { Update(); }, TimeSpan.FromMilliseconds(5), System.Windows.Threading.DispatcherPriority.SystemIdle);
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine(ex.Message);
            //    }
            //}
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

        private int _PhysicalAxes;
        public int PhysicalAxes
        {
            get
            {
                return _PhysicalAxes;
            }
            set
            {
                if (_PhysicalAxes == value)
                    return;
                _PhysicalAxes = value;
                RaisePropertyChanged("PhysicalAxes");
            }
        }
        private int _PhysicalButtons;
        public int PhysicalButtons
        {
            get
            {
                return _PhysicalButtons;
            }
            set
            {
                if (_PhysicalButtons == value)
                    return;
                _PhysicalButtons = value;
                RaisePropertyChanged("PhysicalButtons");
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
