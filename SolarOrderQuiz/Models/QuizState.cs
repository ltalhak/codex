using System.Collections.Generic;

namespace SolarOrderQuiz.Models
{
    public class QuizState
    {
        public Dictionary<string, bool> PlacedPlanets { get; set; } = new();

        public Dictionary<string, bool> AnsweredQuestions { get; set; } = new();

        public int Score { get; set; }

        public int CorrectAnswers { get; set; }

        public int IncorrectAnswers { get; set; }

        public int ElapsedSeconds { get; set; }

        public int BonusScore { get; set; }

        public bool IsCompleted { get; set; }
    }
}
