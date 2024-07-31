using ProductService2.Abstract_;
using System.Text.Json;

namespace ProductService2.DataProviders
{
    public class ExchangeRatesProvider : IExchangeRatesProvider
    {
        public List<ExchangeRate> GetTimeSeries(DateOnly? startDate = null, DateOnly? endDate = null)
        {
            // Hardcoded from data  https://fx-rate.net/historical/?c_input=USD&cp_input=RUB&date_to_input=&range_input=30
            // TODO parse CSV:      https://fx-rate.net/historical/?c_input=USD&cp_input=RUB&date_to_input=&range_input=30&csv=true
            var ratesJsonPath = Path.Combine(AppContext.BaseDirectory, "Content", "rates.json");
            using var streamReader = new StreamReader(ratesJsonPath);
            var ratesJson = streamReader.ReadToEnd();

            var timeSeriesObj = JsonSerializer.Deserialize<TimeSeries>(ratesJson, new JsonSerializerOptions
            {
                Converters = {
                    new DateOnlyJsonConverter() ,
                    new StringToDecimalConverter()
                }
            });

            List<ExchangeRate> ratesDict = timeSeriesObj.historicalRatesList
                .Where(x => (startDate == null || x.date >= startDate) && (endDate == null || x.date <= endDate))
                .OrderBy(x => x.date)
                .Select(x => new ExchangeRate(x.date, x.rates.RUB))
                .ToList();

            return ratesDict;
        }

        public record TimeSeries(DateOnly startDate, DateOnly endDate, string @base, IEnumerable<HistoricalRate> historicalRatesList);

        public record HistoricalRate(DateOnly date, Rates rates);

        public record Rates(decimal RUB);
    }

    public record ExchangeRate(DateOnly Date, decimal Value);
}
