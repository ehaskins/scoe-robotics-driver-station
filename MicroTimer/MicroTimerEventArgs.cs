using System;

namespace MicroLibrary
{
    public class MicroTimerEventArgs : EventArgs
    {
        // Simple counter, number times timed event (callback function) executed
        public int TimerCount { get; private set; }
        // Time when timed event was called since timer started
        public long ElapsedMicroseconds { get; private set; }
        // How late the timer was compared to when it should have been called
        public long TimerLateBy { get; private set; }
        // The time it took to execute the previous
        // call to the callback function (OnTimedEvent)
        public long CallbackFunctionExecutionTime { get; private set; }

        public MicroTimerEventArgs(int nTimerCount, long lElapsedMicroseconds,
               long lTimerLateBy, long lCallbackFunctionExecutionTime)
        {
            TimerCount = nTimerCount;
            ElapsedMicroseconds = lElapsedMicroseconds;
            TimerLateBy = lTimerLateBy;
            CallbackFunctionExecutionTime = lCallbackFunctionExecutionTime;
        }
    }
}
