namespace ExchangeRatesService.Abstract
{
	public interface IRepo<T> where T : BaseEntity
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetAsync(Guid id);
		Task DeleteAsync(Guid id);
		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
	}
}
