using AutoMapper;
using BlazorContacts.Common.Automapper;
using System;
using System.Linq;
using System.Reflection;

namespace BlazorContacts.Automapper.Common
{
	public class AutoMapperProfile : Profile
	{

		private Assembly[] _supportedAssemblies;

		public AutoMapperProfile()
		{
			_supportedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.FullName.Contains("BlueBrown")).ToArray();

			LoadStandardMappings();
			LoadCustomMappings();
			LoadConverters();
		}

		private void LoadConverters()
		{

		}

		private void LoadCustomMappings()
		{
			var mapsFrom = MapperProfileHelper.LoadCustomMappings(_supportedAssemblies);

			foreach (var map in mapsFrom)
			{
				map.CreateMappings(this);
			}
		}

		private void LoadStandardMappings()
		{
			var mapsFrom = MapperProfileHelper.LoadSourceMappings(_supportedAssemblies);

			foreach (var map in mapsFrom)
			{
				CreateMap(map.Source, map.Destination).ReverseMap();
			}

			var mapsTo = MapperProfileHelper.LoadDestinationMappings(_supportedAssemblies);

			foreach (var map in mapsTo)
			{
				CreateMap(map.Source, map.Destination).ReverseMap();
			}
		}
	}
}
