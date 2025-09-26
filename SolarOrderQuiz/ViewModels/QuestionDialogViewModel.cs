using System;
using System.Collections.ObjectModel;
using SolarOrderQuiz.Models;
using SolarOrderQuiz.Resources;

namespace SolarOrderQuiz.ViewModels
{
    public class QuestionDialogViewModel : ViewModelBase
    {
        private QuestionChoice? selectedChoice;
        private bool showFeedback;
        private string feedbackMessage = string.Empty;
        private bool isCorrect;

        public QuestionDialogViewModel(string planetName, Question question)
        {
            Question = question;
            Title = string.Format(Strings.QuestionTitleFormat, planetName);
            QuestionText = question.Text;
            Explanation = question.Explanation;
            Choices = new ObservableCollection<QuestionChoice>(question.Choices);

            ConfirmCommand = new RelayCommand(_ => Confirm(), _ => SelectedChoice is not null);
            ContinueCommand = new RelayCommand(_ => Continue(), _ => ShowFeedback);
        }

        public event EventHandler<QuestionResult>? Closed;

        public string Title { get; }

        public string QuestionText { get; }

        public ObservableCollection<QuestionChoice> Choices { get; }

        public Question Question { get; }

        public QuestionChoice? SelectedChoice
        {
            get => selectedChoice;
            set
            {
                if (SetProperty(ref selectedChoice, value))
                {
                    (ConfirmCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool ShowFeedback
        {
            get => showFeedback;
            private set
            {
                if (SetProperty(ref showFeedback, value))
                {
                    (ContinueCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string FeedbackMessage
        {
            get => feedbackMessage;
            private set => SetProperty(ref feedbackMessage, value);
        }

        public string Explanation { get; }

        public bool IsCorrect
        {
            get => isCorrect;
            private set => SetProperty(ref isCorrect, value);
        }

        public RelayCommand ConfirmCommand { get; }

        public RelayCommand ContinueCommand { get; }

        private void Confirm()
        {
            if (SelectedChoice is null)
            {
                return;
            }

            IsCorrect = SelectedChoice.IsCorrect;
            FeedbackMessage = SelectedChoice.IsCorrect ? Strings.CorrectAnswerFeedback : Strings.IncorrectAnswerFeedback;
            ShowFeedback = true;
        }

        private void Continue()
        {
            if (!ShowFeedback)
            {
                return;
            }

            Closed?.Invoke(this, new QuestionResult
            {
                IsCorrect = IsCorrect,
                SelectedKey = SelectedChoice?.Key
            });
        }
    }
}
