using Microsoft.EntityFrameworkCore;

namespace ProductService2.Domain
{
	public class DataContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public DataContext()
		{

		}

		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
	}
}
