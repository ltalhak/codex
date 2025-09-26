using System.Windows.Input;

namespace SolarOrderQuiz.ViewModels
{
    public class OrbitSlotViewModel : ViewModelBase
    {
        private PlanetItemViewModel? planet;
        private bool showError;
        private bool showSuccess;

        public OrbitSlotViewModel(int orbitIndex, string displayName)
        {
            OrbitIndex = orbitIndex;
            DisplayName = displayName;
        }

        public int OrbitIndex { get; }

        public string DisplayName { get; }

        public double OrbitSize => 140 + (OrbitIndex - 1) * 50;

        public double CanvasOffset => (600 - OrbitSize) / 2;

        public PlanetItemViewModel? Planet
        {
            get => planet;
            set => SetProperty(ref planet, value);
        }

        public bool ShowError
        {
            get => showError;
            set => SetProperty(ref showError, value);
        }

        public bool ShowSuccess
        {
            get => showSuccess;
            set => SetProperty(ref showSuccess, value);
        }

        public ICommand? DropCommand { get; set; }
    }
}
