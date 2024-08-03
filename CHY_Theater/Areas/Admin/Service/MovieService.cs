using CHY_Theater_Models.Models;

namespace CHY_Theater.Areas.Admin.Service
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7207/api/movies"; // Update with your API URL

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Movie>>(_apiBaseUrl);
        }

        public async Task<Movie> GetMovieAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Movie>($"{_apiBaseUrl}/{id}");
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, movie);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Movie>();
        }

        public async Task UpdateMovieAsync(int id, Movie movie)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", movie);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteMovieAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
