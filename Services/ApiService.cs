using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace praC3.Services
{
    public class ApiService
    {
        private HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://127.0.0.1:8000/api/")
        };

        public async Task<List<MatchResult>> GetResults()
        {
            var response = await client.GetAsync("results");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<MatchResult>>(json, options) ?? new List<MatchResult>();
        }

        public async Task<List<MatchInfo>> GetMatches()
        {
            var response = await client.GetAsync("matches");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<MatchInfo>>(json, options) ?? new List<MatchInfo>();
        }
    }

    public class MatchResult
    {
        public int Match_Id { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int Winner_Team_Id { get; set; }
    }

    public class MatchInfo
    {
        public int Match_Id { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public string Start_Time { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}