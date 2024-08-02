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

        // Много математики, взято как есть из произвольного источника. Проверка показывает адекватные графики.
        public ExchangeDeviation GetMaxDeviation(List<ExchangeRate> timeSeries)
        {
            var exchangeRates = timeSeries.Select(x => x.Value).ToArray();

            int n = exchangeRates.Length;
            var days = Enumerable.Range(1, n).Select(i => (decimal)i).ToArray();

            // Вычисление коэффициентов линейной регрессии
            var sumX = days.Sum();
            var sumY = exchangeRates.Sum();
            var sumXY = days.Zip(exchangeRates, (x, y) => x * y).Sum();
            var sumX2 = days.Sum(x => x * x);

            var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            var intercept = (sumY - slope * sumX) / n;

            // Вычисление значений тренда и отклонений
            var trend = days.Select(x => slope * x + intercept).ToArray();
            var deviations = exchangeRates.Zip(trend, (y, t) => y - t).ToArray();

            // Вывод результатов
            Console.WriteLine("Day\tRate\tTrend\tDeviation");
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"{i + 1}\t{exchangeRates[i]:F2}\t{trend[i]:F2}\t{deviations[i]:F6}\t{timeSeries[i].Date}");
            }

            var minDeviation = deviations.Min();
            var maxDeviation = deviations.Max();
            var maxAbsDeviation = Math.Abs(minDeviation) > Math.Abs(maxDeviation) ? minDeviation : maxDeviation;

            var indexOfDeviation = Array.IndexOf(deviations, maxAbsDeviation);

            return new ExchangeDeviation(timeSeries[indexOfDeviation].Date, maxAbsDeviation);
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
