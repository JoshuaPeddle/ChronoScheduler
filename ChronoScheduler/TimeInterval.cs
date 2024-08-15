using System;

namespace ChronoScheduler
{
    public class TimeInterval
    {
        public int Hours { get; private set; }
        public int Minutes { get; private set; }

        public TimeInterval(int hours, int minutes)
        {
            if (hours < 0 || hours > 23)
                throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be between 0 and 23.");
            if (minutes < 0 || minutes > 59)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be between 0 and 59.");
            if (hours == 0 && minutes == 0)
                throw new ArgumentException("Interval cannot be zero.");

            Hours = hours;
            Minutes = minutes;
        }

        internal TimeSpan ToTimeSpan()
        {
            return new TimeSpan(Hours, Minutes, 0);
        }
    }
}
