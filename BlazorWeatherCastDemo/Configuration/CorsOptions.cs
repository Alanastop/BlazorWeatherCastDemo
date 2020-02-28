using System.Collections.Generic;

namespace BlazorWeatherCastDemo.Configuration
{
	public class CorsOptions
	{
		public BasicCors BasicCors { get; set; }

		public WebSocketCors WebSocketCors { get; set; }
	}

	public class BasicCors
	{
		public ICollection<string> Origins { get; set; }

		public ICollection<string> AllowedMethods { get; set; } = new HashSet<string>();
	}

	public class WebSocketCors
	{
		public ICollection<string> AllowedOrigins { get; set; } = new HashSet<string>();
	}
}
