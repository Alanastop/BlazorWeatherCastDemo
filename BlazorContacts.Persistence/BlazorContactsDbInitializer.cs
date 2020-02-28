using BlazorContacts.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorContacts.Persistence
{
	public class BlazorContactsDbInitializer
	{
		/// <summary>
		/// This method is made to emulate the old database initialization and migration strategies from EF6
		/// </summary>
		/// <param name="context">The database context</param>
		public static void CreateAndMigrate(BlazorContactsDBContext context)
		{
			var initializer = new BlazorContactsDbInitializer();

			//TODO :Need to think this through along with CI/CD & Devops
			try
			{
				context.Database.Migrate();

				BlazorContactsDbInitializer.Seed(context);
			}
			catch (AggregateException ex)
			{
				System.Diagnostics.Debugger.Break();
				throw;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debugger.Break();
				throw;
			}
		}

		public static void Seed(BlazorContactsDBContext context)
		{
			if (!context.Contacts.Any())
			{
				context.Contacts.AddRange(new[] {
					new Contact() { FirstName = "Alex", LastName = "Koutsomhtros", PhoneNumber = "6984567341" },
					new Contact() { FirstName = "Novi", LastName = "Panagiotou", PhoneNumber = "6984567342" },
					new Contact() { FirstName = "NoviFI", LastName = "Papamixalhs", PhoneNumber = "6984567343" },
					new Contact() { FirstName = "NoviDE", LastName = "Karadhmos", PhoneNumber = "6984567344" },
					new Contact() { FirstName = "Bet666", LastName = "Kaloghrou", PhoneNumber = "6984567345"},
					new Contact() { FirstName = "SpinBet365", LastName = "Karagiwrghs", PhoneNumber = "6984567346" },
					new Contact() { FirstName = "CasinoRio" , LastName = "Papakwstopoulos", PhoneNumber = "6984567347" },
					new Contact() { FirstName = "Assos", LastName = "Papas", PhoneNumber = "6984567348" },
					new Contact() { FirstName = "MonacoAces", LastName = "Papakwnstantinou", PhoneNumber = "6984567349" },
					new Contact() { FirstName = "KingSolomon", LastName = "Papadopoulos", PhoneNumber = "6984567350" },
					new Contact() { FirstName = "NoviCasino", LastName = "Tsourapas", PhoneNumber = "6984567352" }
				});
			}

			context.SaveChanges();
		}

	}
}
