using System;
using SlimDX;
using SlimDX.DirectInput;
using System.Collections.Generic;

namespace EHaskins.Frc.Communication.DriverStation
{
    public class SlimDXDriverStation : DriverStation
    {
        SlimDX.DirectInput.Joystick[] _Joysticks;
        public SlimDXDriverStation(SlimDX.DirectInput.Joystick[] joysticks)
            : base()
        {
            Joysticks = joysticks;
            SendingData += SendingDataHandler;
        }

        private void SendingDataHandler(object sender, EventArgs e)
        {
            var dataSticks = ControlData.Joysticks;
            for (int i = 0; i < 4; i++)
            {

                if (Joysticks.Length > i)
                {
                    var stick = Joysticks[i];
                    var dataStick = dataSticks[i];
                    if (stick.Acquire().IsSuccess && stick.Poll().IsSuccess)
                    {
                        var state = stick.GetCurrentState();
                        if (Result.Last.IsSuccess)
                        {
                            dataStick.Axes[0] = state.X / 1000f;
                            dataStick.Axes[1] = state.Y / 1000f;
                            dataStick.Axes[2] = state.Z / 1000f;
                            dataStick.Axes[3] = state.RotationX / 1000f;
                            dataStick.Axes[4] = state.RotationY / 1000f;
                            dataStick.Axes[5] = state.RotationZ / 1000f;
                        }

                        var buttons = state.GetButtons();
                        for (int button = 0; button < buttons.Length; button++)
                        {
                            dataStick.Buttons[button] = buttons[button];
                        }
                    }
                }
            }

            //string axes = "";
            //foreach (var axis in dataSticks[0].Axes){
            //    axes += " " + axis.ToString("f2");
            //}
            //Console.WriteLine(axes);
        }
        public SlimDX.DirectInput.Joystick[] Joysticks
        {
            get
            {
                return _Joysticks;
            }
            set
            {
                if (_Joysticks == value)
                    return;
                _Joysticks = value;
                RaisePropertyChanged("Joysticks");
            }
        }

    
    }
}
