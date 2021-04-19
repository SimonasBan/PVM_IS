using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Linq;

namespace IS_Turizmas.Models
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PlaceOfInterest> PlaceOfInterest { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var myJsonString = System.IO.File.ReadAllText("..\\config.json");
                var myJObject = JObject.Parse(myJsonString);
                var conf_string = myJObject.SelectToken("ConfigurationString").Value<string>();
                optionsBuilder.UseMySQL(conf_string);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlaceOfInterest>(entity =>
            {
                entity.ToTable("PlaceOfInterest");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Pavadinimas)
                    .IsRequired()
                    .HasColumnName("Pavadinimas")
                    .HasMaxLength(255);

                entity.Property(e => e.Aprasymas)
                    .IsRequired()
                    .HasColumnName("Aprasymas")
                    .HasMaxLength(5000);

                entity.Property(e => e.Taskai)
                    .HasColumnName("Taskai")
                    .HasColumnType("int(11)");
            });
            

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
