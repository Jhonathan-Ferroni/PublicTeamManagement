using Microsoft.AspNetCore.Mvc;
using PublicTeamManagement.Models;
using PublicTeamManagement.Services;
using System.Collections.Generic;
using System.Linq;

namespace PublicTeamManagement.Controllers
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

        // Removido async/await pois o PlayerService agora é síncrono
        public IActionResult Index()
        {
            var players = _playerService.GetAll();
            return View(players);
        }

        [HttpPost]
        public IActionResult Generate(int[] selectedIds, int numberOfTeams, bool hasFixedCaptains, Dictionary<string, string>? selectedCaptains, double margin = 10)
        {
            var selectedPlayersIds = selectedIds?.Distinct().ToHashSet() ?? new HashSet<int>();

            // 1. Tratamento de capitães
            var captainsToProcess = new Dictionary<int, int?>();
            var usedCaptains = new HashSet<int>();

            if (hasFixedCaptains && selectedCaptains != null)
            {
                foreach (var entry in selectedCaptains)
                {
                    if (int.TryParse(entry.Key, out int teamIndex) && int.TryParse(entry.Value, out int playerId))
                    {
                        if (playerId == 0) continue;

                        if (!selectedPlayersIds.Contains(playerId) || !usedCaptains.Add(playerId))
                        {
                            TempData["Error"] = "Os capitães devem ser atletas selecionados e não podem se repetir.";
                            return RedirectToAction(nameof(Index));
                        }
                        captainsToProcess[teamIndex] = playerId;
                    }
                }
            }

            // 2. Validação de segurança
            if (selectedIds == null || selectedIds.Length < numberOfTeams || numberOfTeams < 2)
            {
                TempData["Error"] = "Selecione atletas suficientes para a quantidade de times.";
                return RedirectToAction(nameof(Index));
            }

            // Chamada síncrona para o Service de CSV
            var allPlayers = _playerService.GetAll();
            var selectedPlayers = allPlayers.Where(p => selectedIds.Contains(p.Id)).ToList();

            if (selectedPlayers.Count < numberOfTeams)
            {
                TempData["Error"] = "Selecione atletas suficientes.";
                return RedirectToAction(nameof(Index));
            }

            // 3. Lógica de Geração (Otimização de Equilíbrio)
            var candidateGenerations = new List<(List<Team> Teams, double Score)>();
            var allGenerations = new List<(List<Team> Teams, double Score)>();

            for (int i = 0; i < 200; i++)
            {
                var currentTeams = _generatorService.BuildBalancedTeams(selectedPlayers, numberOfTeams, captainsToProcess);

                if (currentTeams == null || !currentTeams.Any()) continue;

                var totals = currentTeams.Select(t => t.TotalOverall).ToList();
                var avg = totals.Average();
                var variance = totals.Select(x => Math.Pow(x - avg, 2)).Average();
                double score = Math.Sqrt(variance);
                double diff = totals.Max() - totals.Min();

                allGenerations.Add((currentTeams, score));

                if (diff <= margin)
                {
                    candidateGenerations.Add((currentTeams, score));
                }
            }

            var pool = candidateGenerations.Any() ? candidateGenerations : allGenerations;

            var topCandidates = pool
                .OrderBy(x => x.Score)
                .Take(Math.Min(10, pool.Count))
                .ToList();

            var selected = topCandidates[Random.Shared.Next(topCandidates.Count)];

            // 4. Preparação da View de Resultado
            ViewBag.Teams = selected.Teams;
            ViewBag.Difference = selected.Score;
            ViewBag.SelectedIds = selectedIds;
            ViewBag.NumberOfTeams = numberOfTeams;
            ViewBag.Margin = margin;

            return View("Result", allPlayers);
        }
    }
}