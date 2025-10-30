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

        // Token-Tracking
        private int _totalTokensUsed = 0;
        private readonly ILogger<PerplexityService> _logger;

        public PerplexityService(IConfiguration configuration, ILogger<PerplexityService> logger)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["PerplexityAPI:ApiKey"];
            _logger = logger;

            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GenerateTokiPonaSentence(string difficulty = "beginner", bool requireConfirmation = true)
        {
            int estimatedTokens = 35; 

            if (requireConfirmation && !await ConfirmTokenUsage("GenerateSentence", estimatedTokens))
                return string.Empty;

            var requestBody = new
            {
                model = "sonar", 
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = $"Toki Pona {difficulty} sentence:" 
                    }
                },
                temperature = 0.7,
                max_tokens = 20 
            };

            return await ExecuteRequest(requestBody, "generate");
        }

        public async Task<TranslationResult> TranslateSentence(string tokiPonaSentence, bool requireConfirmation = true)
        {
            int estimatedTokens = 50;

            if (requireConfirmation && !await ConfirmTokenUsage("Translate", estimatedTokens))
                return new TranslationResult { TokiPonaSentence = tokiPonaSentence, GermanTranslation = "" };

            var requestBody = new
            {
                model = "sonar", 
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = $"'{tokiPonaSentence}' -> German:" 
                    }
                },
                temperature = 0.2, 
                max_tokens = 30 
            };

            var translation = await ExecuteRequest(requestBody, "translate");
            return new TranslationResult
            {
                TokiPonaSentence = tokiPonaSentence,
                GermanTranslation = translation
            };
        }

        // OPTIMIERT: Einfaches Ja/Nein statt Erklärungen
        public async Task<ValidationResult> ValidateSentence(string userAnswer, string correctAnswer, bool requireConfirmation = true)
        {
            int estimatedTokens = 60;

            if (requireConfirmation && !await ConfirmTokenUsage("Validate", estimatedTokens))
                return new ValidationResult { IsCorrect = false, Feedback = "Cancelled" };

            var requestBody = new
            {
                model = "sonar", 
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = $"Same meaning?\n1: '{userAnswer}'\n2: '{correctAnswer}'\nAnswer: YES/NO"
                    }
                },
                temperature = 0.1,
                max_tokens = 10
            };

            var result = await ExecuteRequest(requestBody, "validate");

            return new ValidationResult
            {
                IsCorrect = result.Contains("YES", StringComparison.OrdinalIgnoreCase),
                Feedback = result.Trim()
            };
        }

        public async Task<List<string>> GenerateWordPool(string tokiPonaSentence, int poolSize = 10, bool requireConfirmation = true)
        {
            int estimatedTokens = 45;

            if (requireConfirmation && !await ConfirmTokenUsage("WordPool", estimatedTokens))
                return new List<string>();

            var requestBody = new
            {
                model = "sonar",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = $"{poolSize} Toki Pona words for: '{tokiPonaSentence}'\nFormat: word1,word2,word3"
                    }
                },
                temperature = 0.5,
                max_tokens = 40 
            };

            var response = await ExecuteRequest(requestBody, "wordpool");
            return response?.Split(',')
                .Select(w => w.Trim())
                .Where(w => !string.IsNullOrEmpty(w))
                .Take(poolSize)
                .ToList() ?? new List<string>();
        }

        // NEU: Bestätigungsmechanismus
        private async Task<bool> ConfirmTokenUsage(string operation, int estimatedTokens)
        {
            _logger.LogWarning($"⚠️ Operation '{operation}' will use ~{estimatedTokens} tokens. Total used: {_totalTokensUsed}");

            // In Produktion: Hier UI-Bestätigung einbauen oder Budget-Check
            // Für jetzt: Automatisch erlauben, aber loggen
            return true;
        }

        // NEU: Zentrale Request-Ausführung mit Token-Tracking
        private async Task<string> ExecuteRequest(object requestBody, string operation)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/chat/completions", requestBody);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PerplexityResponse>();
                var content = result?.Choices[0]?.Message?.Content?.Trim() ?? string.Empty;

                // Token-Usage aus Response-Headers extrahieren (falls verfügbar)
                if (response.Headers.TryGetValues("x-tokens-used", out var tokenValues))
                {
                    if (int.TryParse(tokenValues.FirstOrDefault(), out int tokensUsed))
                    {
                        _totalTokensUsed += tokensUsed;
                        _logger.LogInformation($"✓ {operation}: {tokensUsed} tokens used. Total: {_totalTokensUsed}");
                    }
                }

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ {operation} failed: {ex.Message}");
                throw;
            }
        }

        // NEU: Token-Statistik abrufen
        public int GetTotalTokensUsed() => _totalTokensUsed;
    }

    // Response Models (unverändert)
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
