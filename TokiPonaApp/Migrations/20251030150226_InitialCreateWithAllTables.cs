using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TokiPonaApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithAllTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sentences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokiPonaSentence = table.Column<string>(type: "text", nullable: false),
                    GermanSentence = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WordPoolJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UsageCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sentences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_stats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Accuracy = table.Column<double>(type: "double precision", precision: 5, scale: 2, nullable: false),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vocabularies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokiPonaWord = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GermanTranslation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    options = table.Column<string[]>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabularies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "vocabularies",
                columns: new[] { "Id", "CreatedAt", "GermanTranslation", "options", "TokiPonaWord" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 30, 15, 2, 26, 422, DateTimeKind.Utc).AddTicks(5253), "sprechen, Sprache", new[] { "sprechen", "essen", "gehen", "sehen" }, "toki" },
                    { 2, new DateTime(2025, 10, 30, 15, 2, 26, 422, DateTimeKind.Utc).AddTicks(6755), "gut, einfach", new[] { "gut", "schlecht", "groß", "klein" }, "pona" },
                    { 3, new DateTime(2025, 10, 30, 15, 2, 26, 422, DateTimeKind.Utc).AddTicks(6757), "Person, Mensch", new[] { "Person", "Tier", "Ding", "Ort" }, "jan" }
                });

            migrationBuilder.CreateIndex(
                name: "idx_sentences_difficulty",
                table: "sentences",
                column: "Difficulty");

            migrationBuilder.CreateIndex(
                name: "idx_sentences_difficulty_usage",
                table: "sentences",
                columns: new[] { "Difficulty", "UsageCount" });

            migrationBuilder.CreateIndex(
                name: "idx_sentences_usage",
                table: "sentences",
                column: "UsageCount");

            migrationBuilder.CreateIndex(
                name: "idx_user_stats_accuracy",
                table: "user_stats",
                column: "Accuracy",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "idx_user_stats_username",
                table: "user_stats",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_vocabularies_options_gin",
                table: "vocabularies",
                column: "options")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "idx_vocabularies_toki_pona_word",
                table: "vocabularies",
                column: "TokiPonaWord",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sentences");

            migrationBuilder.DropTable(
                name: "user_stats");

            migrationBuilder.DropTable(
                name: "vocabularies");
        }
    }
}
