using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExchangeRatesService.Abstract;
using ExchangeRatesService.DataProviders;

namespace ExchangeRatesService.Controllers
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
            var maxDeviation = _exchangeRatesProvider.GetMaxDeviation(timeSeries);
            return maxDeviation;
        }        
    }
}
