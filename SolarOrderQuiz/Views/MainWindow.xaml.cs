using System.Windows;
using SolarOrderQuiz.Services;
using SolarOrderQuiz.ViewModels;

namespace SolarOrderQuiz.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(new QuestionDialogService());
        }
    }
}
