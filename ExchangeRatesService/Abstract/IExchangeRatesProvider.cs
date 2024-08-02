using ExchangeRatesService.DataProviders;

namespace ExchangeRatesService.Abstract
{
    public interface IExchangeRatesProvider
    {
        List<ExchangeRate> GetTimeSeries(DateOnly? startDate = null, DateOnly? endDate = null);

        ExchangeDeviation GetMaxDeviation(List<ExchangeRate> timeSeries);
    }

    public record ExchangeDeviation(DateOnly Date, decimal Value);
}
