using QuickGrid.Crud;

namespace QuickGrid.Examples.Data
{
    public class WeatherForecast
    {
        [GridVisivel(true)]
        public DateOnly Date { get; set; }
        [GridVisivel(true)]
        [GridPodeFiltrar(true)]
        public int TemperatureC { get; set; }
        [GridVisivel(true)]
        [GridPodeFiltrar(true)]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        [GridVisivel(true)]
        [GridPodeFiltrar(true)]
        public string? Summary { get; set; }
    }
}