using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stackr_Api.Models;

namespace Stackr_Api.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users {get; set;}
        public DbSet<RankingList> RankingLists {get; set;}
        public DbSet<Item> Items {get; set;}
        public DbSet<Ranking> Rankings {get; set;}
        
    }
}