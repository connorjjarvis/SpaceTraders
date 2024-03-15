namespace SpaceTraders.Models
{
    public class Waypoint
    {
        public string Symbol { get; set; }
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<Orbital> Orbitals { get; set; }
        public string Orbits { get; set; }
    }
}
