using System.Windows.Media;
using SolarOrderQuiz.Models;

namespace SolarOrderQuiz.ViewModels
{
    public class PlanetItemViewModel : ViewModelBase
    {
        private bool isPlaced;
        private ImageSource? imageSource;

        public PlanetItemViewModel(PlanetDefinition definition)
        {
            Definition = definition;
            DisplayName = definition.DisplayName;
        }

        public PlanetDefinition Definition { get; }

        public string DisplayName { get; }

        public bool IsPlaced
        {
            get => isPlaced;
            set => SetProperty(ref isPlaced, value);
        }

        public ImageSource? Image
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }
    }
}
