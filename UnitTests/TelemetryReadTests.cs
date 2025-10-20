using System.Diagnostics;
using Core.Services;
using Core.Services.FuelCalculator;

namespace Tests;

[TestFixture]
public class TelemetryReadTests
{
    private SimReader _reader;

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        string filePath = "../../../../../mx5 mx52016_nurburgring gpnochicane 2025-10-18 14-42-02.ibt";
        // _reader = new SimReader(filePath);
        // var service = new FuelCalculatorService(_reader);
        // service.FuelUpdated += TestUpdate;
        //
        // while (_reader.ReadNextFrame()) {}


        Assert.Pass();
    }
}
