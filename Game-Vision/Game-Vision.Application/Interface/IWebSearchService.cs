using System.Text.RegularExpressions;

namespace Game_Vision.Application.Interface
{
    public interface IWebSearchService
    {
        Task<int?> GetBenchmarkScore(string model, string type);
    }

    public class WebSearchService : IWebSearchService
    {
        private readonly HttpClient _httpClient;

        public WebSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int?> GetBenchmarkScore(string model, string type)
        {
            if (string.IsNullOrWhiteSpace(model))
                return null;

       
            var query = $"\"{model}\" {type} PassMark score site:passmark.com";

            try
            {
               
                var searchUrl = type.ToLower() == "cpu"
                    ? "https://www.cpubenchmark.net/search.php?search=" + Uri.EscapeDataString(model)
                    : "https://www.videocardbenchmark.net/search.php?search=" + Uri.EscapeDataString(model);

                var response = await _httpClient.GetAsync(searchUrl);
                if (!response.IsSuccessStatusCode)
                    return null;

                var html = await response.Content.ReadAsStringAsync();


                var scorePattern = new Regex(@"(\d{1,3}(,\d{3})*(\.\d+)?)\s*PassMark", RegexOptions.IgnoreCase);
                var match = scorePattern.Match(html);

                if (match.Success)
                {
                    var scoreStr = match.Groups[1].Value.Replace(",", "");
                    if (int.TryParse(scoreStr, out int score))
                    {
                        return score;
                    }
                }

            
                var fallbackPattern = new Regex(@"Score\D*(\d{4,6})", RegexOptions.IgnoreCase);
                match = fallbackPattern.Match(html);

                if (match.Success && int.TryParse(match.Groups[1].Value, out int fallbackScore))
                {
                    return fallbackScore;
                }

                return null;
            }
            catch
            {
       
                return null;
            }
        }
    }
}
