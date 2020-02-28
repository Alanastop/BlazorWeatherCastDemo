using BlazorContacts.Persistence.Infrastracture;
using Microsoft.EntityFrameworkCore;

namespace BlazorContacts.Persistence
{
	public class BlazorContactDBContextFactory : DesignTimeDbContextFactoryBase<BlazorContactsDBContext>
	{
		protected override BlazorContactsDBContext CreateNewInstance(DbContextOptions<BlazorContactsDBContext> options)
		{
			return new BlazorContactsDBContext(options);
		}
	}
}
