using StarChampionship.Data;
using StarChampionship.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StarChampionship.Services.Exceptions;

namespace StarChampionship.Services
{
    public class PlayerService
    {
        private readonly StarChampionshipContext _context;

        public PlayerService(StarChampionshipContext context)
        {
            _context = context;
        }

        public async Task<List<Player>> FindAllAsync()
        {
            // Removido qualquer referência a Department
            return await _context.Player.ToListAsync();
        }

        public async Task InsertAsync(Player obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<Player> FindByIdAsync(int id)
        {
            // Removido o .Include(obj => obj.Department) pois a propriedade não existe mais
            return await _context.Player.FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Player.FindAsync(id);
                if (obj != null)
                {
                    _context.Player.Remove(obj);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException(e.Message);
            }
        }

        public async Task UpdateAsync(Player obj)
        {
            bool hasAny = await _context.Player.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
            {
                throw new NotFoundException("ID not found");
            }
            try
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}