using Microsoft.EntityFrameworkCore;
using ProductService2.Domain;

namespace ProductService2.Abstract_
{
    public class EFRepo<T> : IRepo<T> where T : BaseEntity
    {
        private readonly DataContext _dataContext;

        public EFRepo(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dataContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await _dataContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dataContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                _dataContext.Remove(entity);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task AddAsync(T entity)
        {
            await _dataContext.Set<T>().AddAsync(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dataContext.Entry(entity).State = EntityState.Modified;
            await _dataContext.SaveChangesAsync();
        }
    }
}
