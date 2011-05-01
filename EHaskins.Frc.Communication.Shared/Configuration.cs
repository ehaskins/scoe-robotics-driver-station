using System;

namespace EHaskins.Frc.Communication
{
    public static class Configuration
    {
        const int DEFAULT_USER_CONTROL_DATA_SIZE = 936;
        const int DEFAULT_USER_STATUS_DATA_SIZE = 984;
        const int DEFAULT_INVALID_PACKET_SAFETY_LEVEL = 500;
        const int DS_TO_ROBOT_REMOTE_PORT = 1110;
        const int ROBOT_TO_DS_LOCAL_PORT = 1150;
        static int _userControlDataSize = DEFAULT_USER_CONTROL_DATA_SIZE;
        private static int _userStatusDataSize = DEFAULT_USER_STATUS_DATA_SIZE;
        private static int _invalidPacketCountSafety = DEFAULT_INVALID_PACKET_SAFETY_LEVEL;
        private static int _dsToRobotRemotePortNumber = DS_TO_ROBOT_REMOTE_PORT;
        private static int _dsToRobotLocalPortNumber = DS_TO_ROBOT_REMOTE_PORT;
        private static int _robotToDsRemotePortNumber = ROBOT_TO_DS_LOCAL_PORT;

        private static int _robotToDsLocalPortNumber = ROBOT_TO_DS_LOCAL_PORT;

        //public static int DsToRobotLocalPortNumber
        //{
        //    get { return _dsToRobotLocalPortNumber; }
        //    set { _dsToRobotLocalPortNumber = value; }
        //}
        public static int DsToRobotDestinationPortNumber
        {
            get { return _dsToRobotRemotePortNumber; }
            set { _dsToRobotRemotePortNumber = value; }
        }
        //public static int RobotToDsLocalPortNumber
        //{
        //    get { return _robotToDsLocalPortNumber; }
        //    set { _robotToDsLocalPortNumber = value; }
        //}
        public static int RobotToDsDestinationPortNumber
        {
            get { return _robotToDsRemotePortNumber; }
            set { _robotToDsRemotePortNumber = value; }
        }

        public static int InvalidPacketCountSafety
        {
            get { return _invalidPacketCountSafety; }
        }

        public static int UserControlDataSize
        {
            get { return _userControlDataSize; }
            set { _userControlDataSize = value; }
        }
        public static int UserStatusDataSize
        {
            get { return _userStatusDataSize; }
            set
            {
                _userStatusDataSize = value;
            }
        }
        

    }
}
