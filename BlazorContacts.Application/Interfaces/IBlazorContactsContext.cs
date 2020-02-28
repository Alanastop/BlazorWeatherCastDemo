using BlazorContacts.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BlazorContacts.Application
{
	public interface IBlazorContactsDBContext
	{
		DbSet<Contact> Contacts { get; set; }
	}
}
