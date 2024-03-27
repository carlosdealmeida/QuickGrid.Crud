using System.Linq.Expressions;

namespace QuickGrid.Examples.Data
{
    public class WeatherForecastService
    {
        public ServiceTest service;
        public WeatherForecastService(ServiceTest service)
        {
            this.service = service;
        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<IQueryable<WeatherForecast>> GetAll(Expression<Func<WeatherForecast, bool>> pred)
        {
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                Link = "<a class = 'btn btn-primary' href = 'https://www.google.com'>TESTE</a>"
            }).AsQueryable());
        }

        public Task<IQueryable<WeatherForecast>> GetAllV2()
        {
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                Link = "<a class = 'btn btn-primary' href = 'https://www.google.com'>TESTE</a>"
            }).AsQueryable());
        }
    }
}