using CHY_Theater_Models.Models;
using System.Net.Http.Headers;

namespace CHY_Theater.Areas.Admin.Service
{
    public class MovieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7207/api/movies"; // Update with your API URL

        public MovieService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AdminJwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("No JWT token found");           
            }
            // Set the authorization header with the Bearer token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Make the HTTP GET request and return the result
            var response = await _httpClient.GetAsync(_apiBaseUrl);
            switch ((int)response.StatusCode)
            {
                case 200:
                    return await response.Content.ReadFromJsonAsync<IEnumerable<Movie>>();
                case 401:
                    throw new UnauthorizedAccessException("Authentication failed. Please log in again.");
                case 403:
                    throw new UnauthorizedAccessException("You do not have permission to access this resource.");
                default:
                    throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }
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
