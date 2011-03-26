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
        public SlimDxJoystick(JoystickManager manager, String name, bool syncronize) : base()
        {
            this.dispatch = syncronize ? Dispatcher.CurrentDispatcher : null;
            this.Manager = manager;
            Name = name;
        }
        public override void Update()
        {
            if (dispatch == null || dispatch.CheckAccess())
            {
                if (Joystick != null)
                {
                    if (Joystick.Acquire().IsSuccess && Joystick.Poll().IsSuccess)
                    {
                        var state = Joystick.GetCurrentState();
                        if (SlimDX.Result.Last.IsSuccess)
                        {

                            var sliders = state.GetSliders();
                            var physicalAxes = new double[6 + sliders.Length];
                            physicalAxes[0] = state.X / 1000f;
                            physicalAxes[1] = state.Y / 1000f;
                            physicalAxes[2] = state.Z / 1000f;
                            physicalAxes[3] = state.RotationX / 1000f;
                            physicalAxes[4] = state.RotationY / 1000f;
                            physicalAxes[5] = state.RotationZ / 1000f;
                            for (int i = 0; i < sliders.Length; i++)
                            {
                            	physicalAxes[6+i] = sliders[i] / 1000f;
                            }

                            for (int i = 0; i < 6; i++)
                            {
                                Axes[i] = physicalAxes[i];
                            }
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
                    dispatch.Invoke((ThreadStart)delegate { Update(); }, TimeSpan.FromMilliseconds(5), System.Windows.Threading.DispatcherPriority.SystemIdle);
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
