namespace StarChampionship.Models.ViewModels
{
   public class GeneratorViewModel
{
    public List<Player> AvailablePlayers { get; set; }
    public int NumberOfTeams { get; set; } = 2; // Default
    public bool HasFixedCaptains { get; set; }
    
    // Mapeia o ID do Time para o ID do Jogador Capitão
    public Dictionary<int, int?> SelectedCaptains { get; set; } = new(); 
}
}
