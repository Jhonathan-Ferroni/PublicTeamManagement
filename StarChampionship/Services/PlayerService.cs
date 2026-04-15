using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using PublicTeamManagement.Models;

namespace PublicTeamManagement.Services
{
    public class PlayerService
    {
        private readonly string _filePath;
        private readonly object _fileLock = new();

        public PlayerService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            // Gerar/obter um identificador por usuário (armazenado em cookie).
            // Esse id será usado para um arquivo CSV separado por usuário.
            var httpContext = httpContextAccessor.HttpContext;
            string userId = "default";

            try
            {
                if (httpContext != null)
                {
                    var request = httpContext.Request;
                    var response = httpContext.Response;

                    if (!request.Cookies.TryGetValue("PTM_UserId", out userId) || string.IsNullOrWhiteSpace(userId))
                    {
                        userId = Guid.NewGuid().ToString("N");
                        // Cookie durará 1 ano
                        response.Cookies.Append("PTM_UserId", userId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true });
                    }
                }
            }
            catch
            {
                // Em alguns cenários (ex: execução em background) HttpContext pode ser nulo
                userId = "default";
            }

            // Usar ContentRootPath para não expor os CSVs via static files
            var dataDir = Path.Combine(env.ContentRootPath, "data");
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);

            _filePath = Path.Combine(dataDir, $"players_{userId}.csv");

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

            lock (_fileLock)
            {
                using var reader = new StreamReader(_filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                return csv.GetRecords<Player>().ToList();
            }
        }

        public void SaveAll(List<Player> players)
        {
            lock (_fileLock)
            {
                var temp = _filePath + ".tmp";
                using (var writer = new StreamWriter(temp, false))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(players);
                    writer.Flush();
                }

                // Substituição atômica do arquivo
                File.Move(temp, _filePath, true);
            }
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