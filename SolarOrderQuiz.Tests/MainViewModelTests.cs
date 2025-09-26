using System.Linq;
using FluentAssertions;
using SolarOrderQuiz.Models;
using SolarOrderQuiz.Services;
using SolarOrderQuiz.ViewModels;
using Xunit;

namespace SolarOrderQuiz.Tests
{
    public class MainViewModelTests
    {
        [Fact]
        public void PlanetCatalog_ShouldBeInCorrectOrder()
        {
            var names = PlanetCatalog.All.Select(p => p.DisplayName).ToArray();
            names.Should().ContainInOrder("Merkür", "Venüs", "Dünya", "Mars", "Jüpiter", "Satürn", "Uranüs", "Neptün");
        }

        [Fact]
        public void DroppingCorrectPlanet_ShouldIncreaseScoreAndCorrectAnswers()
        {
            var vm = new MainViewModel(new StubQuestionDialogService(correct: true));
            var planet = vm.PalettePlanets.First(p => p.Definition.Id == "mercury");
            var slot = vm.OrbitSlots.First(s => s.OrbitIndex == planet.Definition.OrbitIndex);

            slot.DropCommand?.Execute(planet);

            vm.Score.Should().BeGreaterOrEqualTo(30);
            vm.CorrectAnswers.Should().Be(1);
            vm.RemainingPlanets.Should().Be(7);
        }

        [Fact]
        public void QuestionService_ShouldAvoidImmediateRepeats_WhenEnoughQuestions()
        {
            var service = new QuestionService();
            var first = service.GetRandomQuestion("venus", Enumerable.Empty<string>());
            var second = service.GetRandomQuestion("venus", new[] { first.Id });

            second.Id.Should().NotBe(first.Id);
        }

        private sealed class StubQuestionDialogService : IQuestionDialogService
        {
            private readonly bool correct;

            public StubQuestionDialogService(bool correct)
            {
                this.correct = correct;
            }

            public QuestionResult ShowQuestion(string planetName, Question question)
            {
                return new QuestionResult
                {
                    IsCorrect = correct,
                    SelectedKey = question.Choices.First().Key
                };
            }
        }
    }
}
