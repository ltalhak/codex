using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using SolarOrderQuiz.Models;

namespace SolarOrderQuiz.Services
{
    public class QuestionService
    {
        private readonly Dictionary<string, List<Question>> questionsByPlanet = new(StringComparer.OrdinalIgnoreCase);
        private readonly Random random = new();

        public QuestionService()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SolarOrderQuiz.Resources.Questions.json");
            if (stream is null)
            {
                throw new InvalidDataException("Questions resource cannot be found.");
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var questionList = JsonSerializer.Deserialize<List<Question>>(json, options) ?? new List<Question>();
            foreach (var question in questionList)
            {
                if (!questionsByPlanet.TryGetValue(question.PlanetId, out var list))
                {
                    list = new List<Question>();
                    questionsByPlanet[question.PlanetId] = list;
                }

                list.Add(question);
            }
        }

        public Question GetRandomQuestion(string planetId, IEnumerable<string>? excludedQuestionIds = null)
        {
            if (!questionsByPlanet.TryGetValue(planetId, out var list) || list.Count == 0)
            {
                throw new InvalidOperationException($"No question available for planet {planetId}.");
            }

            var available = list;
            if (excludedQuestionIds is not null)
            {
                var excludedSet = new HashSet<string>(excludedQuestionIds, StringComparer.OrdinalIgnoreCase);
                var filtered = list.Where(q => !excludedSet.Contains(q.Id)).ToList();
                if (filtered.Count > 0)
                {
                    available = filtered;
                }
            }

            var selected = available[random.Next(available.Count)];
            return CloneQuestion(selected);
        }

        private static Question CloneQuestion(Question question)
        {
            return new Question
            {
                Id = question.Id,
                PlanetId = question.PlanetId,
                Text = question.Text,
                Explanation = question.Explanation,
                Choices = question.Choices.Select(choice => new QuestionChoice
                {
                    Key = choice.Key,
                    Text = choice.Text,
                    IsCorrect = choice.IsCorrect
                }).ToList()
            };
        }
    }
}
