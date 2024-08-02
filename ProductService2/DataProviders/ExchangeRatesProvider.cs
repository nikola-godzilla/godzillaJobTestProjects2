using ExchangeRatesService.Abstract;
using ProtoBuf;

namespace ExchangeRatesService.DataProviders
{
    public class ExchangeRatesProvider : IExchangeRatesProvider
    {
        public List<ExchangeRate> GetTimeSeries(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            // Hardcoded from data  https://fx-rate.net/historical/?c_input=USD&cp_input=RUB&date_to_input=&range_input=30
            var ratesProtoBuf = Path.Combine(AppContext.BaseDirectory, "Content", "ratesProtoBuf.bin");

            TimeSeries timeSeriesObj;
            using (var fileStream = File.Open(ratesProtoBuf, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var bufferedStream = new BufferedStream(fileStream))
            {
                timeSeriesObj = Serializer.Deserialize<TimeSeries>(bufferedStream);
            }

            List<ExchangeRate> ratesDict = timeSeriesObj.HistoricalRatesList
                .Where(x => (startDate == null || x.Date >= startDate) && (endDate == null || x.Date <= endDate))
                .OrderBy(x => x.Date)
                .Select(x => new ExchangeRate(x.Date, x.Rates.RUB))
                .ToList();

            return ratesDict;
        }

        [ProtoContract]
        public record TimeSeries
        {
            [ProtoMember(1)]
            public DateOnly StartDate { get; set; }

            [ProtoMember(2)]
            public DateOnly EndDate { get; set; }

            [ProtoMember(3)]
            public string Base { get; set; }

            [ProtoMember(4)]
            public IEnumerable<HistoricalRate> HistoricalRatesList { get; set; }
        }

        [ProtoContract]
        public record HistoricalRate
        {
            [ProtoMember(1)]
            public DateOnly Date { get; set; }

            [ProtoMember(2)]
            public Rates Rates { get; set; }
        }

        [ProtoContract]
        public record Rates
        {
            [ProtoMember(1)]
            public decimal RUB { get; set; }
        }
    }

    public record ExchangeRate(DateOnly Date, decimal Value);
}
