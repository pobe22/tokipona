using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TokiPonaQuiz.Services
{
    public class PerplexityService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.perplexity.ai";

        public PerplexityService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["PerplexityAPI:ApiKey"];

            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GenerateTokiPonaSentence(string difficulty = "beginner")
        {
            var requestBody = new
            {
                model = "sonar-pro",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a Toki Pona language expert. Generate only valid Toki Pona sentences."
                    },
                    new
                    {
                        role = "user",
                        content = $"Generate one simple {difficulty}-level Toki Pona sentence. Return only the sentence, no explanations."
                    }
                },
                temperature = 0.7,
                max_tokens = 50
            };

            var response = await _httpClient.PostAsJsonAsync("/chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PerplexityResponse>();
            return result?.Choices[0]?.Message?.Content?.Trim() ?? string.Empty;
        }

        public async Task<TranslationResult> TranslateSentence(string tokiPonaSentence)
        {
            var requestBody = new
            {
                model = "sonar-pro",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a Toki Pona translator. Translate accurately to German."
                    },
                    new
                    {
                        role = "user",
                        content = $"Translate this Toki Pona sentence to German: '{tokiPonaSentence}'. Return only the German translation."
                    }
                },
                temperature = 0.3,
                max_tokens = 100
            };

            var response = await _httpClient.PostAsJsonAsync("/chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PerplexityResponse>();
            return new TranslationResult
            {
                TokiPonaSentence = tokiPonaSentence,
                GermanTranslation = result?.Choices[0]?.Message?.Content?.Trim() ?? string.Empty
            };
        }

        public async Task<ValidationResult> ValidateSentence(string userAnswer, string correctAnswer)
        {
            var requestBody = new
            {
                model = "sonar-reasoning-pro",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a Toki Pona language validator. Compare two sentences and determine if they are semantically equivalent."
                    },
                    new
                    {
                        role = "user",
                        content = $"Compare these sentences:\nUser: '{userAnswer}'\nCorrect: '{correctAnswer}'\n\nRespond with only 'CORRECT' or 'INCORRECT' followed by a brief explanation."
                    }
                },
                temperature = 0.1,
                max_tokens = 150
            };

            var response = await _httpClient.PostAsJsonAsync("/chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PerplexityResponse>();
            var content = result?.Choices[0]?.Message?.Content ?? string.Empty;

            return new ValidationResult
            {
                IsCorrect = content.StartsWith("CORRECT", StringComparison.OrdinalIgnoreCase),
                Feedback = content
            };
        }

        public async Task<List<string>> GenerateWordPool(string tokiPonaSentence, int poolSize = 10)
        {
            var requestBody = new
            {
                model = "sonar-pro",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = $"Given this Toki Pona sentence: '{tokiPonaSentence}', generate {poolSize} Toki Pona words including the words from the sentence plus some distractors. Return as comma-separated list."
                    }
                },
                temperature = 0.5,
                max_tokens = 100
            };

            var response = await _httpClient.PostAsJsonAsync("/chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PerplexityResponse>();
            var words = result?.Choices[0]?.Message?.Content?.Split(',')
                .Select(w => w.Trim())
                .ToList() ?? new List<string>();

            return words;
        }
    }

    // Response Models
    public class PerplexityResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class TranslationResult
    {
        public string TokiPonaSentence { get; set; }
        public string GermanTranslation { get; set; }
    }

    public class ValidationResult
    {
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; }
    }
}
