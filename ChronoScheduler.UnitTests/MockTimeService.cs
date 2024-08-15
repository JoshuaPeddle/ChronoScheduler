namespace ChronoScheduler.UnitTests
{
    public class MockTimeService(DateTime currentTime) : ITimeService
    {
        private DateTime _currentTime = currentTime;

        public DateTime Now => _currentTime;

        public void AdvanceTime(TimeSpan timeSpan)
        {
            _currentTime = _currentTime.Add(timeSpan);
        }
    }
}