using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SolarOrderQuiz.Models;

namespace SolarOrderQuiz.Services
{
    public class SaveLoadService
    {
        private readonly string folderPath;
        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        public SaveLoadService()
        {
            folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SolarOrderQuiz");
        }

        public async Task SaveAsync(QuizState state, string fileName = "save.json")
        {
            Directory.CreateDirectory(folderPath);
            var path = Path.Combine(folderPath, fileName);
            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, state, options).ConfigureAwait(false);
        }

        public async Task<QuizState?> LoadAsync(string fileName = "save.json")
        {
            var path = Path.Combine(folderPath, fileName);
            if (!File.Exists(path))
            {
                return null;
            }

            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<QuizState>(stream).ConfigureAwait(false);
        }
    }
}
