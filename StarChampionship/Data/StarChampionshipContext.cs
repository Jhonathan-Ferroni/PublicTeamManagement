using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StarChampionship.Models;

namespace StarChampionship.Data
{
    public class StarChampionshipContext : DbContext
    {
        public StarChampionshipContext (DbContextOptions<StarChampionshipContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Player { get; set; }
    }
}
