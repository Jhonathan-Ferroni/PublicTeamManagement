using System.Globalization;
using CsvHelper;
using PublicTeamManagement.Models;

namespace PublicTeamManagement.Services
{
    public class PlayerService
    {
        private readonly string _filePath;

        public PlayerService(IWebHostEnvironment env)
        {
            // Define o caminho na pasta wwwroot/data/players.csv
            _filePath = Path.Combine(env.WebRootPath, "data", "players.csv");

            // Cria a pasta e o arquivo caso não existam
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);

            // Se o arquivo não existir, cria um vazio com o cabeçalho para evitar erros de leitura
            if (!File.Exists(_filePath))
            {
                SaveAll(new List<Player>());
            }
        }

        // Retorna o caminho para o Controller usar no Download
        public string GetFilePath() => _filePath;

        public List<Player> GetAll()
        {
            if (!File.Exists(_filePath)) return new List<Player>();

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Player>().ToList();
        }

        public void SaveAll(List<Player> players)
        {
            using var writer = new StreamWriter(_filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(players);
        }

        public void AddPlayer(Player player)
        {
            var players = GetAll();
            // Lógica simples para gerar ID incremental
            player.Id = players.Any() ? players.Max(p => p.Id) + 1 : 1;
            players.Add(player);
            SaveAll(players);
        }

        public Player? FindById(int id)
        {
            return GetAll().FirstOrDefault(p => p.Id == id);
        }

        public void Update(Player obj)
        {
            var players = GetAll();
            var index = players.FindIndex(p => p.Id == obj.Id);

            if (index != -1)
            {
                players[index] = obj;
                SaveAll(players);
            }
        }

        public void Remove(int id)
        {
            var players = GetAll();
            var player = players.FirstOrDefault(p => p.Id == id);

            if (player != null)
            {
                players.Remove(player);
                SaveAll(players);
            }
        }
    }
}