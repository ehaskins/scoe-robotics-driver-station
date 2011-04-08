#if !NETMF
using System;

namespace MicroLibrary
{
    public class MicroTimer
    {
        public delegate void MicroTimerElapsedEventHandler(object sender,
                             MicroTimerEventArgs timerEventArgs);
        public event MicroTimerElapsedEventHandler Elapsed;

        System.Threading.Thread m_threadTimer = null;
        long m_lIgnoreEventIfLateBy = long.MaxValue;
        long m_lTimerIntervalInMicroSec = 0;
        bool m_bStopTimer = true;

        public MicroTimer()
        {
        }

        public MicroTimer(long lTimerIntervalInMicroseconds)
        {
            Interval = lTimerIntervalInMicroseconds;
        }

        public long Interval
        {
            get { return m_lTimerIntervalInMicroSec; }
            set { m_lTimerIntervalInMicroSec = value; }
        }

        public long IgnoreEventIfLateBy
        {
            get
            {
                return m_lIgnoreEventIfLateBy;
            }
            set
            {
                if (value == 0)
                    m_lIgnoreEventIfLateBy = long.MaxValue;
                else
                    m_lIgnoreEventIfLateBy = value;
            }
        }

        public bool Enabled
        {
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
            get
            {
                return (m_threadTimer != null && m_threadTimer.IsAlive);
            }
        }

        public void Start()
        {
            if ((m_threadTimer == null || !m_threadTimer.IsAlive) && Interval > 0)
            {
                m_bStopTimer = false;
                System.Threading.ThreadStart threadStart =
                  delegate()
                  {
                      NotificationTimer(Interval,
                          IgnoreEventIfLateBy, ref m_bStopTimer);
                  };
                m_threadTimer = new System.Threading.Thread(threadStart);
                m_threadTimer.Priority = System.Threading.ThreadPriority.Highest;
                m_threadTimer.Start();
            }
        }

        public void Stop()
        {
            m_bStopTimer = true;

            while (Enabled)
            {
            }
        }

        void NotificationTimer(long lTimerInterval,
             long lIgnoreEventIfLateBy, ref bool bStopTimer)
        {
            int nTimerCount = 0;
            long lNextNotification = 0;
            long lCallbackFunctionExecutionTime = 0;

            MicroStopwatch microStopwatch = new MicroStopwatch();
            microStopwatch.Start();

            while (!bStopTimer)
            {
                lCallbackFunctionExecutionTime =
                  microStopwatch.ElapsedMicroseconds - lNextNotification;
                lNextNotification += lTimerInterval;
                nTimerCount++;
                long lElapsedMicroseconds = 0;

                while ((lElapsedMicroseconds =
                        microStopwatch.ElapsedMicroseconds) < lNextNotification)
                {
                }

                long lTimerLateBy = lElapsedMicroseconds - (nTimerCount * lTimerInterval);

                if (lTimerLateBy < lIgnoreEventIfLateBy)
                {
                    MicroTimerEventArgs microTimerEventArgs =
                      new MicroTimerEventArgs(nTimerCount, lElapsedMicroseconds,
                      lTimerLateBy, lCallbackFunctionExecutionTime);
                    Elapsed(this, microTimerEventArgs);
                }
            }

            microStopwatch.Stop();
        }
    }
}
#endif