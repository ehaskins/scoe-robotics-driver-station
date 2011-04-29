using System;
using System.IO.Ports;
using System.Threading;

namespace EHaskins.Frc.Communication
{
    public class SerialTranceiver : Transceiver
    {
        byte[] packetStart = { 255, 254, 253, 252 };
        SerialPort _port;
        Thread _receieveThread;
        bool _isStopped = true;
        public SerialTranceiver()
        {
            
        }
        public override void Start()
        {
            _port = new SerialPort(PortName, 115200);
            _port.Open();
            _receieveThread = new Thread((ThreadStart)this.ReceiveDataSync) { Priority = ThreadPriority.AboveNormal };
            _receieveThread.Start();
        }
        public override void Stop()
        {
            if (_port != null && _port.IsOpen)
                _port.Close();
            _port = null;
        }
        protected override void InvalidateConnection()
        {
            base.InvalidateConnection();
        }
        public override void Transmit(byte[] data)
        {
            var fullData = new byte[data.Length + packetStart.Length];
            packetStart.CopyTo(fullData, 0);
            data.CopyTo(fullData, packetStart.Length);

            _port.Write(fullData, 0, fullData.Length);
        }

        private void ReceiveDataSync()
        {
            _isStopped = false;
            int headerPosition = 0;
            while (IsEnabled)
            {
                try
                {
                    var b = _port.ReadByte();
                    if (b == packetStart[headerPosition])
                        headerPosition++;
                    else
                        headerPosition = 0;
                    if (headerPosition == packetStart.Length)
                    {

                        headerPosition = 0;
                        var buffer = new byte[PacketSize];
                        _port.Read(buffer, 0, PacketSize);

                        RaiseDataReceived(buffer);
                    }

                }
                catch (Exception ex)
                {
                    if (IsEnabled)
                        Stop();
                }
            }
            _isStopped = true;
        }
        private string _PortName;
        public string PortName
        {
            get
            {
                return _PortName;
            }
            set
            {
                if (_PortName == value)
                    return;
                _PortName = value;
                RaisePropertyChanged("PortName");
            }
        }
    }
}
