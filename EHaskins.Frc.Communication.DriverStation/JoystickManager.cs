using System;
using System.Collections.ObjectModel;
using SlimDX.DirectInput;
using System.Linq;
namespace EHaskins.Frc.Communication.DriverStation
{
    public class JoystickManager : IDisposable
    {
        private DirectInput _directInput;
        public String[] JoystickNames
        {
            get
            {
                String[] output = new String[Joysticks.Length + 1];
                for (int i = 0; i < Joysticks.Length; i++)
                {
                    output[i] = Joysticks[i].Information.InstanceName;
                }
                output[output.Length - 1] = "Not connected";
                return output;
            }
        }
        public SlimDX.DirectInput.Joystick[] Joysticks { get; set; }

        public JoystickManager()
        {
            _directInput = new DirectInput();
            Joysticks = GetJoysticks().ToArray();
        }
        
        public SlimDX.DirectInput.Joystick GetJoystick(string instanceName){
            return (from j in Joysticks where j.Information.InstanceName == instanceName select j).SingleOrDefault();
        }

        private ObservableCollection<SlimDX.DirectInput.Joystick> GetJoysticks()
        {
            
            var sticks = new ObservableCollection<SlimDX.DirectInput.Joystick>();
            foreach (DeviceInstance device in _directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // create the device
                try
                {
                    var stick = new SlimDX.DirectInput.Joystick(_directInput, device.InstanceGuid);
                    stick.Acquire();

                    foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                    {
                        if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                            stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                    }

                    sticks.Add(stick);
                }
                catch (DirectInputException)
                {
                }
            }

            foreach (var stick in sticks)
            {
                Console.WriteLine(stick.Information.InstanceName);
                /*foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                {
                    if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                        stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                }

                // acquire the device
                stick.Acquire();*/
            }
            Console.WriteLine("Found " + sticks.Count + " sticks.");

            return sticks;
        }

        #region "IDisposable Support"
        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    foreach (SlimDX.DirectInput.Joystick joystick in Joysticks)
                    {
                        joystick.Dispose();
                    }
                    _directInput.Dispose();
                }

            }
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
