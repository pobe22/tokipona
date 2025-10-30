using Microsoft.EntityFrameworkCore;
using TokiPonaQuiz.Models;

namespace TokiPonaQuiz.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Sentence> Sentences { get; set; }
        public DbSet<UserStats> UserStats { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sentence>(entity =>
            {
                entity.ToTable("sentences"); 

                entity.Property(e => e.TokiPonaSentence)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.GermanSentence)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Difficulty)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.WordPoolJson)
                    .HasColumnType("jsonb");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                entity.HasIndex(e => e.Difficulty)
                    .HasDatabaseName("idx_sentences_difficulty");

                entity.HasIndex(e => e.UsageCount)
                    .HasDatabaseName("idx_sentences_usage");

                entity.HasIndex(e => new { e.Difficulty, e.UsageCount })
                    .HasDatabaseName("idx_sentences_difficulty_usage");
            });

            // ========== USERSTATS KONFIGURATION ==========
            modelBuilder.Entity<UserStats>(entity =>
            {
                entity.ToTable("user_stats");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Accuracy)
                    .HasPrecision(5, 2); 

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                entity.HasIndex(e => e.Username)
                    .IsUnique()
                    .HasDatabaseName("idx_user_stats_username");

                entity.HasIndex(e => e.Accuracy)
                    .IsDescending()
                    .HasDatabaseName("idx_user_stats_accuracy");
            });

            // ========== VOCABULARY KONFIGURATION ==========
            modelBuilder.Entity<Vocabulary>(entity =>
            {
                entity.ToTable("vocabularies");

                entity.Property(e => e.TokiPonaWord)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.GermanTranslation)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.OptionsArray)
                    .HasColumnName("options")
                    .HasColumnType("text[]"); 

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                entity.HasIndex(e => e.TokiPonaWord)
                    .IsUnique()
                    .HasDatabaseName("idx_vocabularies_toki_pona_word");

                entity.HasIndex(e => e.OptionsArray)
                    .HasMethod("gin")
                    .HasDatabaseName("idx_vocabularies_options_gin");
            });

            // ========== SEED DATA (OPTIONAL) ==========
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Beispiel-Vokabeln
            modelBuilder.Entity<Vocabulary>().HasData(
                new Vocabulary
                {
                    Id = 1,
                    TokiPonaWord = "toki",
                    GermanTranslation = "sprechen, Sprache",
                    OptionsArray = new[] { "sprechen", "essen", "gehen", "sehen" }
                },
                new Vocabulary
                {
                    Id = 2,
                    TokiPonaWord = "pona",
                    GermanTranslation = "gut, einfach",
                    OptionsArray = new[] { "gut", "schlecht", "groß", "klein" }
                },
                new Vocabulary
                {
                    Id = 3,
                    TokiPonaWord = "jan",
                    GermanTranslation = "Person, Mensch",
                    OptionsArray = new[] { "Person", "Tier", "Ding", "Ort" }
                }
            );
        }
    }
}
