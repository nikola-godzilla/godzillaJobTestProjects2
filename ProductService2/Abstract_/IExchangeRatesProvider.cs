using ProductService2.DataProviders;

namespace ProductService2.Abstract_
{
    public interface IExchangeRatesProvider
    {
        List<ExchangeRate> GetTimeSeries(DateOnly? startDate = null, DateOnly? endDate = null);
    }
}
