using System;
using System.Diagnostics;

namespace FlyApp.Core.Utils
{
    /// <summary>
    ///     Measures elapsed time from Start to End
    ///     Includes method to log output in Debug mode
    ///     Can chain calls like: timer.End().Log() etc
    /// </summary>
    public class ElapsedTimer
    {
        private TimeSpan _elapsedTime;

        private DateTime _endTime;

        // Different times
        private DateTime _startTime;

        // Start the timer
        public ElapsedTimer Start()
        {
            // Start the timer
            _startTime = DateTime.Now;
            return this;
        }

        // End the timer
        public ElapsedTimer End()
        {
            // End timer
            _endTime = DateTime.Now;
            // Elapsed time
            _elapsedTime = _endTime - _startTime;
            return this;
        }

        public DateTime GetStartTime()
        {
            return _startTime;
        }

        public DateTime GetEndTime()
        {
            return _endTime;
        }

        public TimeSpan GetElapsedTime()
        {
            return _elapsedTime;
        }

        // Get the elapsed time as string with Start/End Times
        public string GetElapsedTimeString(string header = null)
        {
            var s = $"ElapsedTime:{_elapsedTime}; Started:{_startTime}; Ended:{_endTime}";
            if (header != null) s = $"{header}: {s}";

            return s;
        }

        // Log the elapsedTime in Debug mode
        public ElapsedTimer LogElapsedTime(string header)
        {
            var s = GetElapsedTimeString(header);
            Debug.WriteLine(s);
            return this;
        }
    }
}