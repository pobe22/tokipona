using Microsoft.EntityFrameworkCore;
using TokiPonaQuiz.Data;
using TokiPonaQuiz.Models;

namespace TokiPonaQuiz.Services
{
    public interface ISentenceRepository
    {
        Task<Sentence> GetRandomSentenceAsync(string difficulty);
        Task<Sentence> CreateSentenceAsync(Sentence sentence);
        Task IncrementUsageAsync(int sentenceId);
        Task<int> GetCountByDifficultyAsync(string difficulty);
    }

    public class SentenceRepository : ISentenceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SentenceRepository> _logger;

        public SentenceRepository(ApplicationDbContext context, ILogger<SentenceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Sentence> GetRandomSentenceAsync(string difficulty)
        {
            var sentences = await _context.Sentences
                .Where(s => s.Difficulty == difficulty)
                .ToListAsync();

            if (!sentences.Any())
                return null;

            // Bevorzuge weniger oft verwendete Sätze
            var leastUsed = sentences.OrderBy(s => s.UsageCount).Take(10).ToList();
            var random = new Random();
            var selected = leastUsed[random.Next(leastUsed.Count)];

            _logger.LogInformation($"📖 Reusing sentence ID {selected.Id} (used {selected.UsageCount} times)");
            return selected;
        }

        public async Task<Sentence> CreateSentenceAsync(Sentence sentence)
        {
            _context.Sentences.Add(sentence);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"💾 Saved new sentence ID {sentence.Id} to database");
            return sentence;
        }

        public async Task IncrementUsageAsync(int sentenceId)
        {
            var sentence = await _context.Sentences.FindAsync(sentenceId);
            if (sentence != null)
            {
                sentence.UsageCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountByDifficultyAsync(string difficulty)
        {
            return await _context.Sentences.CountAsync(s => s.Difficulty == difficulty);
        }
    }
}
