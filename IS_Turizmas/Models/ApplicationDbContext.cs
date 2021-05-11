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

        /*
         * Add created DB tables here
         */
        public virtual DbSet<PlaceOfInterest> PlaceOfInterest { get; set; }
        public virtual DbSet<ClientRoute> ClientRoute { get; set; }
        public virtual DbSet<ClientRouteState> ClientRouteState { get; set; }
        public virtual DbSet<Route> Route { get; set; }
        public virtual DbSet<Route_PlaceOfInterest> Route_PlaceOfInterest { get; set; }



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

            modelBuilder.Entity<Route_PlaceOfInterest>(entity =>
            {
                entity.ToTable("Route_PlaceOfInterest");

                entity.Property(e => e.Number)
                    .HasColumnName("Number")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");

                entity.HasIndex(e => e.Route_id)
                    .HasName("Route_id");

                entity.Property(e => e.Route_id)
                    .HasColumnName("Route_id")
                    .HasColumnType("int(11)");

                entity.HasIndex(e => e.PlaceOfInterest_id)
                    .HasName("PlaceOfInterest_id");
                

                entity.Property(e => e.PlaceOfInterest_id)
                    .HasColumnName("PlaceOfInterest_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Route_idNavigation)
                    .WithMany(p => p.Route_PlaceOfInterest)
                    .HasForeignKey(d => d.Route_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Route_id");

                entity.HasOne(d => d.PlaceOfInterest_idNavigation)
                    .WithMany(p => p.Route_PlaceOfInterest)
                    .HasForeignKey(d => d.PlaceOfInterest_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PlaceOfInterest_id");

            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("Route");



                entity.Property(e => e.Length)
                    .HasColumnName("Length")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnName("Name")
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnName("Description")
                    .HasMaxLength(5000);

                entity.Property(e => e.Rating)
                    .HasColumnName("Rating");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");

            });


            modelBuilder.Entity<ClientRoute>(entity =>
            {
                entity.ToTable("ClientRoute");

                entity.Property(e => e.Start_date)
                    .HasColumnName("Start_date")
                    .HasColumnType("date");

                entity.Property(e => e.Finish_date)
                    .HasColumnName("Finish_date")
                    .HasColumnType("date");

                entity.HasIndex(e => e.Route_id)
                    .HasName("Route_id");

                entity.Property(e => e.Route_id)
                    .HasColumnName("Route_id")
                    .HasColumnType("int(11)");

                entity.HasIndex(e => e.Item_id)
                    .HasName("Item_id");

                entity.Property(e => e.Item_id)
                    .HasColumnName("Item_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Route_idNavigation)
                    .WithMany(p => p.ClientRoute)
                    .HasForeignKey(d => d.Route_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Route_id");

                entity.HasOne(d => d.Item_idNavigation)
                    .WithMany(p => p.ClientRoute)
                    .HasForeignKey(d => d.Item_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Item_id");

                entity.Property(e => e.State_Id)
                    .HasColumnName("State_Id")
                    .HasColumnType("int(11)");

                entity.HasIndex(e => e.State_Id)
                    .HasName("State_Id");

                entity.HasOne(d => d.State_IdNavigation)
                    .WithMany(p => p.ClientRoute)
                    .HasForeignKey(d => d.State_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("State_Id");

                entity.Property(e => e.Calendar_date)
                    .HasColumnName("Calendar_date")
                    .HasColumnType("date");

                //entity.Property(e => e.Client_id)
                //    .HasColumnName("Client_id")
                //    .HasColumnType("int(11)");

                //entity.HasIndex(e => e.Client_id)
                //    .HasName("Client_id");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");
                

            });

            modelBuilder.Entity<PersonalRouteItem>(entity =>
            {
                entity.ToTable("PersonalRouteItem");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Item)
                    .IsRequired()
                    .HasColumnName("Item")
                    .HasMaxLength(255);
            });



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
