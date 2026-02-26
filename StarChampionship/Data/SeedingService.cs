
using StarChampionship.Models;

namespace StarChampionship.Data
{
    public class SeedingService
    {
        private StarChampionshipContext _context;

        public SeedingService(StarChampionshipContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (
                _context.Player.Any())
            {
                return; // DB has been seeded
            }

            _context.SaveChanges();
        }
    }
}
