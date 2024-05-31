using FlashQuiz.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashQuiz.Services
{
    internal class FlashQuizzContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }

        public FlashQuizzContext() { 
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // rajoute le fichier de la db
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "flashquizz.sqlite");
            options.UseSqlite($"Filename={dbPath}");
        }
    }
}
