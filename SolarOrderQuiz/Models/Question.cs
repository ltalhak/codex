using System.Collections.Generic;

namespace SolarOrderQuiz.Models
{
    public class Question
    {
        public string Id { get; set; } = string.Empty;

        public string PlanetId { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        public IList<QuestionChoice> Choices { get; set; } = new List<QuestionChoice>();
    }

    public class QuestionChoice
    {
        public string Key { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
