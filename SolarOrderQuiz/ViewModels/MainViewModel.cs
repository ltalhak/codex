using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SolarOrderQuiz.Models;
using SolarOrderQuiz.Resources;
using SolarOrderQuiz.Services;

namespace SolarOrderQuiz.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly List<PlanetDefinition> planetDefinitions = new();
        private readonly QuestionService questionService;
        private readonly IQuestionDialogService questionDialogService;
        private readonly SaveLoadService saveLoadService;
        private readonly DispatcherTimer timer;
        private readonly Dictionary<string, bool> answeredQuestionResults = new(StringComparer.OrdinalIgnoreCase);

        private int score;
        private int correctAnswers;
        private int incorrectAnswers;
        private int elapsedSeconds;
        private int bonusScore;
        private bool isSummaryVisible;
        private string statusMessage = string.Empty;

        public MainViewModel(IQuestionDialogService questionDialogService)
        {
            this.questionDialogService = questionDialogService;
            questionService = new QuestionService();
            saveLoadService = new SaveLoadService();

            PalettePlanets = new ObservableCollection<PlanetItemViewModel>();
            OrbitSlots = new ObservableCollection<OrbitSlotViewModel>();
            PalettePlanets.CollectionChanged += (_, __) => OnPropertyChanged(nameof(RemainingPlanets));

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (_, __) =>
            {
                elapsedSeconds++;
                OnPropertyChanged(nameof(ElapsedTime));
            };

            InitializePlanets();
            InitializeOrbits();
            StartNewGame();

            ResetCommand = new RelayCommand(_ => StartNewGame());
            RetryCommand = new RelayCommand(_ => StartNewGame());
            SaveCommand = new RelayCommand(_ => SaveState());
            LoadCommand = new RelayCommand(_ => LoadState());
        }

        public ObservableCollection<PlanetItemViewModel> PalettePlanets { get; }

        public ObservableCollection<OrbitSlotViewModel> OrbitSlots { get; }

        public int RemainingPlanets => PalettePlanets.Count;

        public int Score
        {
            get => score;
            private set
            {
                if (SetProperty(ref score, value))
                {
                    OnPropertyChanged(nameof(OverallScore));
                }
            }
        }

        public int BonusScore
        {
            get => bonusScore;
            private set => SetProperty(ref bonusScore, value);
        }

        public int CorrectAnswers
        {
            get => correctAnswers;
            private set
            {
                if (SetProperty(ref correctAnswers, value))
                {
                    OnPropertyChanged(nameof(Accuracy));
                }
            }
        }

        public int IncorrectAnswers
        {
            get => incorrectAnswers;
            private set
            {
                if (SetProperty(ref incorrectAnswers, value))
                {
                    OnPropertyChanged(nameof(Accuracy));
                }
            }
        }

        public string ElapsedTime => TimeSpan.FromSeconds(elapsedSeconds).ToString("mm\\:ss");

        public double Accuracy
        {
            get
            {
                var total = CorrectAnswers + IncorrectAnswers;
                return total == 0 ? 0 : Math.Round((double)CorrectAnswers / total * 100, 2);
            }
        }

        public int OverallScore => Score;

        public bool IsSummaryVisible
        {
            get => isSummaryVisible;
            private set => SetProperty(ref isSummaryVisible, value);
        }

        public string StatusMessage
        {
            get => statusMessage;
            private set => SetProperty(ref statusMessage, value);
        }

        public ICommand ResetCommand { get; }

        public ICommand RetryCommand { get; }

        public ICommand SaveCommand { get; }

        public ICommand LoadCommand { get; }

        private void InitializePlanets()
        {
            planetDefinitions.Clear();
            planetDefinitions.AddRange(PlanetCatalog.All);
        }

        private void InitializeOrbits()
        {
            OrbitSlots.Clear();
            foreach (var definition in planetDefinitions)
            {
                var slot = new OrbitSlotViewModel(definition.OrbitIndex, definition.DisplayName)
                {
                    DropCommand = new RelayCommand(data => HandleDrop(slot, data), data => data is PlanetItemViewModel)
                };
                OrbitSlots.Add(slot);
            }
        }

        private void StartNewGame()
        {
            timer.Stop();
            PalettePlanets.Clear();
            answeredQuestionResults.Clear();
            foreach (var slot in OrbitSlots)
            {
                slot.Planet = null;
                slot.ShowError = false;
                slot.ShowSuccess = false;
            }

            foreach (var definition in planetDefinitions.OrderBy(p => p.OrbitIndex))
            {
                var item = new PlanetItemViewModel(definition)
                {
                    Image = LoadImage(definition.ImageKey)
                };
                PalettePlanets.Add(item);
            }

            Score = 0;
            BonusScore = 0;
            CorrectAnswers = 0;
            IncorrectAnswers = 0;
            elapsedSeconds = 0;
            OnPropertyChanged(nameof(ElapsedTime));
            IsSummaryVisible = false;
            StatusMessage = string.Empty;
            timer.Start();
        }

        private static ImageSource? LoadImage(string key)
        {
            return Application.Current?.TryFindResource(key) as ImageSource;
        }

        private void HandleDrop(OrbitSlotViewModel slot, object? data)
        {
            if (data is not PlanetItemViewModel planet)
            {
                return;
            }

            if (slot.Planet is not null)
            {
                TriggerError(slot, string.Format(Strings.OrbitOccupiedMessage, slot.DisplayName));
                return;
            }

            if (planet.Definition.OrbitIndex != slot.OrbitIndex)
            {
                TriggerError(slot, string.Format(Strings.OrbitMismatchMessage, slot.DisplayName));
                return;
            }

            PlacePlanet(slot, planet);
        }

        private void PlacePlanet(OrbitSlotViewModel slot, PlanetItemViewModel planet)
        {
            slot.Planet = planet;
            planet.IsPlaced = true;
            PalettePlanets.Remove(planet);
            Score += 10;
            StatusMessage = $"{planet.DisplayName} yerleştirildi.";
            slot.ShowSuccess = true;
            _ = ClearSuccessAsync(slot);

            AskQuestion(planet);
            CheckCompletion();
        }

        private async Task ClearSuccessAsync(OrbitSlotViewModel slot)
        {
            await Task.Delay(800);
            slot.ShowSuccess = false;
        }

        private void AskQuestion(PlanetItemViewModel planet)
        {
            try
            {
                var question = questionService.GetRandomQuestion(planet.Definition.Id, answeredQuestionResults.Keys);
                var result = questionDialogService.ShowQuestion(planet.DisplayName, question);
                answeredQuestionResults[question.Id] = result.IsCorrect;
                if (result.IsCorrect)
                {
                    Score += 20;
                    CorrectAnswers++;
                    StatusMessage = $"{planet.DisplayName}: {Strings.CorrectAnswerFeedback}";
                }
                else
                {
                    IncorrectAnswers++;
                    StatusMessage = question.Explanation;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void CheckCompletion()
        {
            if (OrbitSlots.All(slot => slot.Planet is not null))
            {
                CompleteGame();
            }
        }

        private void CompleteGame()
        {
            timer.Stop();
            BonusScore = CalculateBonus(elapsedSeconds);
            Score += BonusScore;
            StatusMessage = Strings.SummaryDescription;
            IsSummaryVisible = true;
        }

        private int CalculateBonus(int seconds)
        {
            if (seconds <= 300)
            {
                return 50;
            }

            if (seconds <= 420)
            {
                return 30;
            }

            if (seconds <= 600)
            {
                return 10;
            }

            return 0;
        }

        private void TriggerError(OrbitSlotViewModel slot, string message)
        {
            slot.ShowError = true;
            StatusMessage = message;
            _ = ClearErrorAsync(slot);
        }

        private async Task ClearErrorAsync(OrbitSlotViewModel slot)
        {
            await Task.Delay(600);
            slot.ShowError = false;
        }

        private void SaveState()
        {
            try
            {
                var state = new QuizState
                {
                    Score = Score,
                    CorrectAnswers = CorrectAnswers,
                    IncorrectAnswers = IncorrectAnswers,
                    ElapsedSeconds = elapsedSeconds,
                    BonusScore = BonusScore,
                    IsCompleted = IsSummaryVisible,
                    AnsweredQuestions = new Dictionary<string, bool>(answeredQuestionResults, StringComparer.OrdinalIgnoreCase)
                };

                foreach (var slot in OrbitSlots)
                {
                    if (slot.Planet is not null)
                    {
                        state.PlacedPlanets[slot.Planet.Definition.Id] = true;
                    }
                }

                saveLoadService.SaveAsync(state).GetAwaiter().GetResult();
                StatusMessage = Strings.SaveSuccess;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("{0}: {1}", Strings.SaveFailed, ex.Message);
            }
        }

        private void LoadState()
        {
            try
            {
                var state = saveLoadService.LoadAsync().GetAwaiter().GetResult();
                if (state is null)
                {
                    StatusMessage = Strings.LoadFailed;
                    return;
                }

                StartNewGame();
                Score = state.Score;
                BonusScore = state.BonusScore;
                CorrectAnswers = state.CorrectAnswers;
                IncorrectAnswers = state.IncorrectAnswers;
                elapsedSeconds = state.ElapsedSeconds;
                OnPropertyChanged(nameof(ElapsedTime));
                answeredQuestionResults.Clear();
                foreach (var entry in state.AnsweredQuestions)
                {
                    answeredQuestionResults[entry.Key] = entry.Value;
                }

                foreach (var planetId in state.PlacedPlanets.Keys)
                {
                    var planet = PalettePlanets.FirstOrDefault(p => p.Definition.Id.Equals(planetId, StringComparison.OrdinalIgnoreCase));
                    if (planet is null)
                    {
                        continue;
                    }

                    PalettePlanets.Remove(planet);
                    planet.IsPlaced = true;
                    var slot = OrbitSlots.First(s => s.OrbitIndex == planet.Definition.OrbitIndex);
                    slot.Planet = planet;
                }

                IsSummaryVisible = state.IsCompleted;
                if (IsSummaryVisible)
                {
                    timer.Stop();
                }

                StatusMessage = Strings.LoadSuccess;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("{0}: {1}", Strings.LoadFailed, ex.Message);
            }
        }
    }
}
