using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Data {
    public class ColorContext : DbContext {
        private readonly string _dbFile;
        public DbSet<ColorSwatch> ColorSwatches { get; set; }
        public DbSet<ColorNumber> ColorNumbers { get; set; }

        public ColorContext(string dbFile) {
            _dbFile = dbFile;
        }

        public bool TryAddNewColorSwatch(ColorSwatch newColorSwatch) {
            bool wasAdded = false;
            IQueryable<ColorSwatch> colorSwatchResults = ColorSwatches.Where(entity => entity.Brand == newColorSwatch.Brand && entity.Name == newColorSwatch.Name);
            if (colorSwatchResults.Count() == 0) {
                ColorSwatches.Add(newColorSwatch);
                SaveChanges();
                wasAdded = true;
                Console.WriteLine("Added " + newColorSwatch.Name);
            }
            else {
                ColorSwatch existingColorSwatch = colorSwatchResults.First();
                ColorNumber newColorNumber = newColorSwatch.ColorNumbers[0];
                IQueryable<ColorNumber> colorNumberResults = ColorNumbers.Where(entity => entity.Number == newColorNumber.Number && entity.ColorSwatchId == existingColorSwatch.Id);
                if (colorNumberResults.Count() == 0) {
                    newColorNumber.ColorSwatchId = existingColorSwatch.Id;
                    ColorNumbers.Add(newColorNumber);
                    SaveChanges();
                    wasAdded = true;
                    Console.WriteLine("Added " + newColorNumber.Number + " for " + existingColorSwatch.Name);
                }
            }
            return wasAdded;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite($"Data Source={_dbFile}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ColorSwatch>(builder => {
                builder.HasIndex(s => new {s.Brand, s.Name}).IsUnique();
                builder.Property(s => s.Name).IsRequired();
            });
            modelBuilder.Entity<ColorNumber>(builder => {
                builder.HasIndex(n => new {n.ColorSwatchId, n.Number}).IsUnique();
                builder.Property(n => n.Number).IsRequired();
            });
        }
    }
}
