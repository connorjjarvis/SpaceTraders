namespace SpaceTraders.Models
{
    public class LocalSystem
    {
        public string Symbol { get; set; }
        public string SectorSymbol { get; set; }
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<Waypoint> Waypoints { get; set; }
        public List<Faction> Factions { get; set; }
    }
}
