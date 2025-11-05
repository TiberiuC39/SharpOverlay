using Core.Events;
using Core.Services;

namespace Tests
{
    [TestFixture]
    public class TelemetryReadTests
    {
        private ISimReader _reader;

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
            _reader.Dispose();
        }

        [Test]
        public void Test1()
        {
            const string filePath = "../../../../../mx5 mx52016_nurburgring gpnochicane 2025-10-18 14-42-02.ibt";
            _reader = new SimReader(filePath);
            // var service = new FuelCalculatorService(_reader);
            // service.FuelUpdated += TestUpdate;

            // while (_reader.ReadNextFrame())
            // {
            //     var sessionInfo = _reader.GetSessionInfo();
            // }

            Assert.Pass();
        }

        private void TestUpdate(object? sender, FuelEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
