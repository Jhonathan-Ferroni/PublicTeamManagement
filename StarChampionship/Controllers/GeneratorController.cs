using Microsoft.AspNetCore.Mvc;
using StarChampionship.Models;
using StarChampionship.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace StarChampionship.Controllers
{
    public class GeneratorController : Controller
    {
        private readonly PlayerService _playerService;
        private readonly GeneratorService _generatorService;

        public GeneratorController(PlayerService playerService, GeneratorService generatorService)
        {
            _playerService = playerService;
            _generatorService = generatorService;
        }

        public async Task<IActionResult> Index()
        {
            var players = await _playerService.FindAllAsync();
            return View(players);
        }

        [HttpPost]
        public async Task<IActionResult> Generate(int[] selectedIds, int numberOfTeams, bool hasFixedCaptains, Dictionary<string, string>? selectedCaptains, double margin = 10)
        {
            var selectedPlayersIds = selectedIds?.Distinct().ToHashSet() ?? new HashSet<int>();

            // 1. Tratamento manual do dicionário para evitar erro de Binding do ASP.NET
            // Recebemos como string e convertemos internamente para int?
            var captainsToProcess = new Dictionary<int, int?>();
            var usedCaptains = new HashSet<int>();

            if (hasFixedCaptains && selectedCaptains != null)
            {
                foreach (var entry in selectedCaptains)
                {
                    // Tenta converter a chave (número do time) e o valor (id do jogador)
                    if (int.TryParse(entry.Key, out int teamIndex) && int.TryParse(entry.Value, out int playerId))
                    {
                        // Se o playerId for 0, o Service já está pronto para ignorar
                        // Ignora opção "Sem Capitão"
                        if (playerId == 0)
                        {
                            continue;
                        }

                        // Garante que o capitão esteja entre os atletas selecionados e não seja repetido
                        if (!selectedPlayersIds.Contains(playerId) || !usedCaptains.Add(playerId))
                        {
                            TempData["Error"] = "Os capitães devem ser atletas selecionados e não podem se repetir entre os times.";
                            return RedirectToAction(nameof(Index));
                        }

                        captainsToProcess[teamIndex] = playerId;
                    }
                }
            }

            // 2. Validação de segurança de dados
            if (selectedIds == null || selectedIds.Length < numberOfTeams || numberOfTeams < 2)
            {
                TempData["Error"] = "Selecione atletas suficientes para a quantidade de times.";
                return RedirectToAction(nameof(Index));
            }

            var allPlayers = await _playerService.FindAllAsync();
            var selectedPlayers = allPlayers.Where(p => selectedIds.Contains(p.Id)).ToList();

            if (selectedPlayers.Count < numberOfTeams)
            {
                TempData["Error"] = "Selecione atletas suficientes para a quantidade de times.";
                return RedirectToAction(nameof(Index));
            }

            var candidateGenerations = new List<(List<Team> Teams, double Score)>();
            var allGenerations = new List<(List<Team> Teams, double Score)>();

            for (int i = 0; i < 200; i++)
            {
                var currentTeams = _generatorService
                    .BuildBalancedTeams(selectedPlayers, numberOfTeams, captainsToProcess);

                if (currentTeams == null || !currentTeams.Any())
                    continue;

                var totals = currentTeams
                    .Select(t => t.TotalOverall)
                    .ToList();

                var avg = totals.Average();

                var variance = totals
                    .Select(x => Math.Pow(x - avg, 2))
                    .Average();

                double score = Math.Sqrt(variance);

                double diff = totals.Max() - totals.Min();

                // 🔥 Guarda TODAS
                allGenerations.Add((currentTeams, score));

                // 🔥 Guarda apenas as que respeitam margem
                if (diff <= margin)
                {
                    candidateGenerations.Add((currentTeams, score));
                }
            }

            var pool = candidateGenerations.Any()
                ? candidateGenerations
                : allGenerations;

            var topCandidates = pool
    .OrderBy(x => x.Score)
    .Take(Math.Min(10, pool.Count))
    .ToList();

            var selected = topCandidates[Random.Shared.Next(topCandidates.Count)];

            var bestGeneration = selected.Teams;
            var bestScore = selected.Score;
            ViewBag.Teams = bestGeneration;
            ViewBag.Difference = bestScore;
            ViewBag.SelectedIds = selectedIds;
            ViewBag.NumberOfTeams = numberOfTeams;
            ViewBag.Margin = margin;

            return View("Result", allPlayers);
        }
    }
}
