using AutoMapper;

namespace BlazorContacts.Common.Automapper.Interfaces
{
	public interface IHaveCustomMapping
	{
		void CreateMappings(Profile configuration);
	}
}
