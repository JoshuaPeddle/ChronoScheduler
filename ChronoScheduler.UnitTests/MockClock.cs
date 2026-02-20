using System;

namespace ChronoScheduler.UnitTests
{
    public class MockClock : ISystemClock
    {
        private DateTimeOffset _now;

        public MockClock(DateTimeOffset startTime)
        {
            _now = startTime;
        }

        public DateTimeOffset UtcNow => _now;

        public void Advance(TimeSpan duration)
        {
            _now = _now.Add(duration);
        }
    }
}

