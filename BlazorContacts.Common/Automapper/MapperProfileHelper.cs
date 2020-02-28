using BlazorContacts.Common.Automapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlazorContacts.Common.Automapper
{
	public sealed class Map
	{
		public Type Source { get; set; }
		public Type Destination { get; set; }
	}

	public static class MapperProfileHelper
	{
		public static IList<Map> LoadSourceMappings(Assembly[] assemblies)
		{
			var types = assemblies.SelectMany(a => a.GetExportedTypes());

			var mapsFrom = (
					from type in types
					from instance in type.GetInterfaces()
					where
						instance.IsGenericType && instance.GetGenericTypeDefinition() == typeof(IMapFrom<>)
						&& !type.IsAbstract
						&& !type.IsInterface
					select new Map
					{
						Source = type.GetInterfaces().First().GetGenericArguments().First(),
						Destination = type
					}).ToList();

			return mapsFrom;
		}

		public static IList<Map> LoadDestinationMappings(Assembly[] assemblies)
		{
			var types = assemblies.SelectMany(a => a.GetExportedTypes());

			var mapsTo = (from type in types
						  from instance in type.GetInterfaces()
						  where instance.IsGenericType && instance.GetGenericTypeDefinition() == typeof(IMapTo<>) &&
								  !type.IsAbstract &&
								  !type.IsInterface
						  select new Map
						  {
							  Source = type,
							  Destination = type.GetInterfaces().First().GetGenericArguments().First()
						  }).ToList();

			return mapsTo;
		}

		public static IList<IHaveCustomMapping> LoadCustomMappings(Assembly[] assemblies)
		{
			var types = assemblies.SelectMany(a => a.GetExportedTypes());

			var mapsFrom = (
					from type in types
					from instance in type.GetInterfaces()
					where
						typeof(IHaveCustomMapping).IsAssignableFrom(type)
						&& !type.IsAbstract
						&& !type.IsInterface
					select (IHaveCustomMapping)Activator.CreateInstance(type)).ToList();

			return mapsFrom;
		}
	}
}
