using StarChampionship.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarChampionship.Services
{
    public class GeneratorService
    {
        private static readonly Random _rand = Random.Shared;

        private static List<T> Shuffle<T>(IEnumerable<T> items)
        {
            var list = items.ToList();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _rand.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }

        public List<Team> BuildBalancedTeams(List<Player> players, int numberOfTeams, Dictionary<int, int?> selectedCaptains)
        {
            // 1. Validação inicial de segurança
            if (numberOfTeams <= 0 || players == null) return new List<Team>();

            // 2. Inicializa os times
            var teams = new List<Team>();
            for (int i = 1; i <= numberOfTeams; i++)
            {
                teams.Add(new Team { Id = i, Name = $"Time {i}", Players = new List<Player>() });
            }

            // 3. Aloca os capitães pré-definidos (Se houver)
            var remainingPlayers = new List<Player>(players);

            // Verificação de nulidade para o dicionário vindo do Controller
            if (selectedCaptains != null)
            {
                foreach (var selection in selectedCaptains)
                {
                    int teamId = selection.Key;
                    int? playerId = selection.Value;

                    // CORREÇÃO: Só processa se houver um ID válido e maior que zero
                    // Isso ignora a opção "-- Sem Capitão --" vinda do HTML
                    if (playerId.HasValue && playerId.Value > 0)
                    {
                        var captain = remainingPlayers.FirstOrDefault(p => p.Id == playerId.Value);
                        if (captain != null)
                        {
                            var targetTeam = teams.FirstOrDefault(t => t.Id == teamId);
                            if (targetTeam != null)
                            {
                                targetTeam.Players.Add(captain);
                                remainingPlayers.Remove(captain);
                            }
                        }
                    }
                }
            }

            // 4. Ordena aleatoriedade e potes
            var pots = remainingPlayers
                .GroupBy(p => Math.Floor(p.Overall / 7.0))
                .OrderByDescending(g => g.Key) // pote mais forte primeiro
                .ToList();

            bool reverse = _rand.Next(2) == 0;

            // 5. Distribuição (Ajuste no critério de desempate)

            foreach (var pot in pots)
            {
                var shuffledPot = Shuffle(pot);

                // Ordena times por força atual
                var orderedTeams = teams
                    .OrderBy(t => t.Players.Sum(p => p.Overall))
                    .ThenBy(t => t.Players.Count)
                                        .ThenBy(_ => _rand.Next())
                    .ToList();

                if (reverse)
                    orderedTeams.Reverse();

                int index = _rand.Next(orderedTeams.Count);

                foreach (var player in shuffledPot)
                {
                    orderedTeams[index % orderedTeams.Count]
                        .Players.Add(player);

                    index++;
                }

                reverse = !reverse;
            }
            return teams;
        }
    }
}