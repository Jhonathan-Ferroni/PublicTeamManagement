namespace StarChampionship.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; } = new();
        public double TotalOverall => Players.Sum(p => p.Overall);
        public double AverageOverall => Players.Any() ? Players.Average(p => p.Overall) : 0;
    }
}
