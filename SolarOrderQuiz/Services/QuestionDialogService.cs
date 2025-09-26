using System.Linq;
using System.Windows;
using SolarOrderQuiz.Models;
using SolarOrderQuiz.ViewModels;
using SolarOrderQuiz.Views;

namespace SolarOrderQuiz.Services
{
    public class QuestionDialogService : IQuestionDialogService
    {
        public QuestionResult ShowQuestion(string planetName, Question question)
        {
            var viewModel = new QuestionDialogViewModel(planetName, question);
            var dialog = new QuestionDialog
            {
                DataContext = viewModel,
                Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(window => window.IsActive)
                         ?? Application.Current?.MainWindow
            };

            QuestionResult? result = null;
            viewModel.Closed += (_, r) =>
            {
                result = r;
                dialog.DialogResult = true;
                dialog.Close();
            };

            dialog.ShowDialog();
            return result ?? new QuestionResult { IsCorrect = false };
        }
    }
}
