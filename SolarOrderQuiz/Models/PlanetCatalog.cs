using System.Collections.Generic;

namespace SolarOrderQuiz.Models
{
    public static class PlanetCatalog
    {
        public static IReadOnlyList<PlanetDefinition> All { get; } = new List<PlanetDefinition>
        {
            new PlanetDefinition("mercury", "Merkür", 1, "MercuryImage"),
            new PlanetDefinition("venus", "Venüs", 2, "VenusImage"),
            new PlanetDefinition("earth", "Dünya", 3, "EarthImage"),
            new PlanetDefinition("mars", "Mars", 4, "MarsImage"),
            new PlanetDefinition("jupiter", "Jüpiter", 5, "JupiterImage"),
            new PlanetDefinition("saturn", "Satürn", 6, "SaturnImage"),
            new PlanetDefinition("uranus", "Uranüs", 7, "UranusImage"),
            new PlanetDefinition("neptune", "Neptün", 8, "NeptuneImage")
        };
    }
}
