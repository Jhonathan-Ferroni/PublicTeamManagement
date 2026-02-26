using Microsoft.AspNetCore.Mvc;
using PublicTeamManagement.Services;
using PublicTeamManagement.Models;
using PublicTeamManagement.Models.ViewModels;
using System.Diagnostics;
using System.Globalization;
using CsvHelper;

namespace PublicTeamManagement.Controllers
{
    public class PlayersController : Controller
    {
        private readonly PlayerService _playerService;

        public PlayersController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        // Listagem principal
        public IActionResult Index()
        {
            var list = _playerService.GetAll();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Player player)
        {
            if (!ModelState.IsValid) return View(player);

            _playerService.AddPlayer(player);
            return RedirectToAction(nameof(Index));
        }

        // Carregar arquivo CSV do computador do usuário
        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                // Lê os registros do arquivo enviado
                var players = csv.GetRecords<Player>().ToList();

                // Salva no nosso "banco" local (players.csv na wwwroot)
                _playerService.SaveAll(players);
            }
            return RedirectToAction(nameof(Index));
        }

        // Baixar o arquivo CSV atual
        public IActionResult Download()
        {
            var filePath = _playerService.GetFilePath();
            if (!System.IO.File.Exists(filePath))
            {
                return RedirectToAction(nameof(Error), new { message = "Arquivo não encontrado para download." });
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/csv", "PublicTeamManagement_Players.csv");
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Error), new { message = "Id not provided" });

            var obj = _playerService.FindById(id.Value);
            if (obj == null) return RedirectToAction(nameof(Error), new { message = "Id not found" });

            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Error), new { message = "Id not provided" });

            var obj = _playerService.FindById(id.Value);
            if (obj == null) return RedirectToAction(nameof(Error), new { message = "Id not found" });

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Player player)
        {
            if (!ModelState.IsValid) return View(player);
            if (id != player.Id) return RedirectToAction(nameof(Error), new { message = "Id mismatch" });

            _playerService.Update(player);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Error), new { message = "Id not provided" });

            var obj = _playerService.FindById(id.Value);
            if (obj == null) return RedirectToAction(nameof(Error), new { message = "Id not found" });

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _playerService.Remove(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}