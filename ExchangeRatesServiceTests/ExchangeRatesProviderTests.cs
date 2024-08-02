using ExchangeRatesService.DataProviders;
using FluentAssertions;

namespace ExchangeRatesServiceTests
{
    [TestFixture]
    public class ExchangeRatesProviderTests
    {
        private readonly ExchangeRatesProvider _sut = new();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("2024-07-01", "2024-07-30")]
        [TestCase("2024-07-03", "2024-07-30")]
        public void GetTimeSeries_ListCount_IsEqualsToInputDaysCount(string startDate, string endDate)
        {
            var dates = (DateTime.Parse(startDate), DateTime.Parse(endDate));

            var result = _sut.GetTimeSeries(DateOnly.FromDateTime(dates.Item1), DateOnly.FromDateTime(dates.Item2));

            result.Count.Should().Be((dates.Item2 - dates.Item1).Days + 1);
        }

        [Test]
        public void GetTimeSeries_TwoOutputs_IsDifferent()
        {
            var dates = (DateTime.Parse("2024-07-01"), DateTime.Parse("2024-07-03"), DateTime.Parse("2024-07-30"));

            var result1 = _sut.GetTimeSeries(DateOnly.FromDateTime(dates.Item1), DateOnly.FromDateTime(dates.Item3));
            var result2 = _sut.GetTimeSeries(DateOnly.FromDateTime(dates.Item2), DateOnly.FromDateTime(dates.Item3));

            result1.First().Date.Should().NotBe(result2.First().Date);
            result1.First().Value.Should().NotBe(result2.First().Value);
        }

        [Test]
        public void GetMaxDeviation_Success()
        {
            var maxDeviation = _sut.GetMaxDeviation(new List<ExchangeRate>
            {
                new(new DateOnly(2024, 07, 01), 100),
                new(new DateOnly(2024, 07, 02), 200),
                new(new DateOnly(2024, 07, 03), 200),
                new(new DateOnly(2024, 07, 04), 100)
            });

            maxDeviation.Value.Should().Be(50);
            maxDeviation.Date.Should().Be(new DateOnly(2024, 07, 02));
        }
    }
}