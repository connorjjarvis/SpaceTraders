namespace SpaceTraders.Models
{
    public class Location
    {
        public string SystemSymbol { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<Orbital> Orbitals { get; set; }
        public List<Trait> Traits { get; set; }
        public List<object> Modifiers { get; set; }
        public Chart Chart { get; set; }
        public Faction Faction { get; set; }
        public bool IsUnderConstruction { get; set; }
    }
    public class Orbital
    {
        public string Symbol { get; set; }
    }

    public class Trait
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Chart
    {
        public string SubmittedBy { get; set; }
        public string SubmittedOn { get; set; }
    }

}
