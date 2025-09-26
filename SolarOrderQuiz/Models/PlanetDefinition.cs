namespace SolarOrderQuiz.Models
{
    public class PlanetDefinition
    {
        public PlanetDefinition(string id, string displayName, int orbitIndex, string imageKey)
        {
            Id = id;
            DisplayName = displayName;
            OrbitIndex = orbitIndex;
            ImageKey = imageKey;
        }

        public string Id { get; }

        public string DisplayName { get; }

        public int OrbitIndex { get; }

        public string ImageKey { get; }
    }
}
