using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHaskins.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;

namespace EHaskins.Frc.Communication.DriverStation
{
    public  class SlimDxJoystick : Joystick
    {
        Dispatcher dispatch;
        JoystickManager manager;
        public SlimDxJoystick(JoystickManager manager, String name) : base()
        {
            this.dispatch = Dispatcher.CurrentDispatcher;
            this.Manager = manager;
            Name = name;
        }
        public override void Update()
        {
            if (dispatch.CheckAccess())
            {
                if (Joystick != null)
                {
                    if (Joystick.Acquire().IsSuccess && Joystick.Poll().IsSuccess)
                    {
                        var state = Joystick.GetCurrentState();
                        if (SlimDX.Result.Last.IsSuccess)
                        {
                            Axes[0] = state.X / 1000f;
                            Axes[1] = state.Y / 1000f;
                            Axes[2] = state.Z / 1000f;
                            Axes[3] = state.RotationX / 1000f;
                            Axes[4] = state.RotationY / 1000f;
                            Axes[5] = state.RotationZ / 1000f;
                        }

                        var buttons = state.GetButtons();
                        for (int button = 0; button < buttons.Length; button++)
                        {
                            Buttons[button] = buttons[button];
                        }
                    }
                }
            }
            else
            {
                try
                {
                    dispatch.Invoke((ThreadStart)delegate { Update(); }, TimeSpan.FromMilliseconds(10), System.Windows.Threading.DispatcherPriority.SystemIdle);
                }
                catch(Exception ex){
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
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
                manager = value;
            }
        }
        public string Name
        {
            get {
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
