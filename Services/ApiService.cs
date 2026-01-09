using praC3.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace praC3.Services
{
    public class ApiService
    {
        private readonly HttpClient client;

        public ApiService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:8000/api/");
        }

        public async Task<List<Match>> GetMatches()
        {
            return await client.GetFromJsonAsync<List<Match>>("matches");
        }

        public async Task<List<MatchResult>> GetResults()
        {
            var response = await client.GetAsync("results");
            if (!response.IsSuccessStatusCode)
                return new List<MatchResult>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MatchResult>>(json);
        }
    }
}
