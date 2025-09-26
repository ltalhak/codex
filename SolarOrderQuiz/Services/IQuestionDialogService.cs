using SolarOrderQuiz.Models;

namespace SolarOrderQuiz.Services
{
    public interface IQuestionDialogService
    {
        QuestionResult ShowQuestion(string planetName, Question question);
    }
}
