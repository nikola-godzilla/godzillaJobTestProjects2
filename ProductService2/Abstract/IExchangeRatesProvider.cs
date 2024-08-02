using ExchangeRatesService.DataProviders;

namespace ExchangeRatesService.Abstract
{
    public interface IExchangeRatesProvider
    {
        List<ExchangeRate> GetTimeSeries(DateOnly? startDate = null, DateOnly? endDate = null);
    }
}
