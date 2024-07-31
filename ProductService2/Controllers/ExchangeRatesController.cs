using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService2.Abstract_;
using ProductService2.DataProviders;

namespace ProductService2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IExchangeRatesProvider _exchangeRatesProvider;

        public ExchangeRatesController(IExchangeRatesProvider exchangeRatesProvider)
        {
            _exchangeRatesProvider = exchangeRatesProvider;
        }

        [HttpGet()]
        public List<ExchangeRate> Get()
        {
            var timeSeries = _exchangeRatesProvider.GetTimeSeries();
            return timeSeries;
        }

        [HttpGet("MaxDeviation")]
        public ExchangeDeviation GetMaxDeviation(string startDate = "2024-07-01", string endDate = "2024-07-30")
        {
            var timeSeries = _exchangeRatesProvider.GetTimeSeries(DateOnly.Parse(startDate), DateOnly.Parse(endDate));
            var maxDeviation = GetMaxDeviation(timeSeries);
            return maxDeviation;
        }


        // Много математики, взято как есть из произвольного источника. Проверка показывает адекватные графики.
        private ExchangeDeviation GetMaxDeviation(List<ExchangeRate> timeSeries)
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

        public record ExchangeDeviation(DateOnly Date, decimal Value);
    }
}
