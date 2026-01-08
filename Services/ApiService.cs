using praC3.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace praC3.Services
{
    public class ApiService
    {
        private readonly HttpClient client;

        public ApiService()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://127.0.0.1:8000/api/");
        }

        public async Task<User> Login(string username)
        {
            var response = await client.PostAsJsonAsync("login", new { username });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            return null;
        }

        public async Task<List<Match>> GetMatches()
        {
            return await client.GetFromJsonAsync<List<Match>>("matches");
        }

        public async Task<List<Match>> GetResults()
        {
            return await client.GetFromJsonAsync<List<Match>>("results");
        }

        public async Task<bool> PlaceBet(int matchId, int teamId, int amount)
        {
            var response = await client.PostAsJsonAsync("bets", new { match_id = matchId, team_id = teamId, amount });
            return response.IsSuccessStatusCode;
        }
    }
}
