using System;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace EHaskins.Frc.Communication.DriverStation
{
		public class SerialTransceiver : Transceiver
		    {
                SerialPort _port;
		        Thread _receieveThread;
		        bool _isStopped;

                private string _PortName;
                public string PortName
                {
                    get { return _PortName; }
                    set
                    {
                        if (_PortName == value)
                            return;
                        _PortName = value;
                        RaisePropertyChanged("PortName");
                    }
                }

                

                private int _MaxLength;
                public int 
                    MaxLength
                {
                    get
                    {
                        return _MaxLength;
                    }
                    set
                    {
                        _MaxLength = value;
                    }
                }
                public SerialTransceiver() : base()
		        {

		        }

		        public override void Transmit(byte[] data)
		        {
		            if (_port == null)
		                return;
                    _port.Write(data, 0, data.Length);
		        }

		        private void ReceiveDataSync()
		        {
		            _isStopped = false;
		            while (IsEnabled)
		            {
		                try
		                {
                            var data = new byte[MaxLength];


		                    RaiseDataReceived(data);
		                }
		                catch (Exception ex)
		                {
		                    if (IsEnabled)
		                        Stop();
		                }
		            }
		            _isStopped = true;
		        }

		        public override void Start()
		        {
		            _IsEnabled = true;
                    _port = new SerialPort();

		            //_client.BeginReceive(this.ReceiveData, null);

		        }
		        public override void Stop()
		        {
		            _IsEnabled = false;
		            if (_port != null)
		                _port.Close();
		            _port = null;
		            SpinWait.SpinUntil(() => _isStopped, 100);
		        }
		        protected override void InvalidateConnection()
		        {
		            try
		            {
		                if (IsEnabled)
		                {
		                    Stop();
		                    //SpinWait.SpinUntil(() => _isStopped == true);
		                    Start();
		                }
		                base.InvalidateConnection();
		            }
		            catch (Exception ex)
		            {
		                Debug.WriteLine(ex.Message);
		                throw;
		            }
		        }
		    }
}
