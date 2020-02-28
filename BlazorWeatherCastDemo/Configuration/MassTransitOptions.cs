namespace BlazorWeatherCastDemo.Configuration
{
	public class MassTransitOptions
	{
		public bool Enabled { get; set; }
		public string Endpoint { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string QueueNamePrefix { get; set; }
		public bool AutoDelete { get; set; }
	}
}
