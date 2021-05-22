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
        public virtual DbSet<PlaceOfInterestComment> PlaceOfInterestComment { get; set; }
        public virtual DbSet<ClientRoute> ClientRoute { get; set; }
        public virtual DbSet<ClientRouteState> ClientRouteState { get; set; }
        public virtual DbSet<Route> Route { get; set; }
        public virtual DbSet<Route_PlaceOfInterest> Route_PlaceOfInterest { get; set; }
        public virtual DbSet<OrientationGame> OrientationGame { get; set; }
        public virtual DbSet<Riddle> Riddle { get; set; }
        public virtual DbSet<ClientOrientationGame> ClientOrientationGame { get; set; }
        public virtual DbSet<OrientationGame_Riddle> OrientationGame_Riddle { get; set; }
        public virtual DbSet<PersonalRouteItem> PersonalRouteItem { get; set; }



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

                entity.Property(e => e.CurrentNumber)
                    .HasColumnName("CurrentNumber")
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

                entity.HasIndex(e => e.userRoute_id)
                    .HasName("userRoute_id");

                entity.Property(e => e.userRoute_id)
                    .HasColumnName("userRoute_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.userRoute_idNavigation)
                    .WithMany(p => p.PersonalRouteItem)
                    .HasForeignKey(d => d.userRoute_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("userRoute_id");

                //entity.HasOne(d => d.ClientRoute_IdNavigation)
                //     .WithMany(p => p.PersonalRouteItem)
                //     .HasForeignKey(d => d.user_id)
                //     .OnDelete(DeleteBehavior.ClientSetNull)
                //     .HasConstraintName("user_id");
            });

            modelBuilder.Entity<PlaceOfInterestComment>(entity =>
            {
                entity.ToTable("PlaceOfInterestComment");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");
                entity.Property(e => e.Comment)
                    .HasColumnName("Comment")
                    .HasMaxLength(255);
                entity.Property(e => e.Rating)
                    .HasColumnName("Rating")
                    .HasColumnType("int(11)");
                entity.Property(e => e.PlaceOfInterest_Id)
                    .HasColumnName("PlaceOfInterest_Id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Riddle>(entity =>
            {
                entity.ToTable("Riddle");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RiddleQuestion)
                    .HasColumnName("RiddleQuestion")
                    .HasMaxLength(255);

                entity.Property(e => e.Answer)
                    .HasColumnName("Answer")
                    .HasMaxLength(255);

                entity.Property(e => e.PlaceCode)
                    .HasColumnName("PlaceCode")
                    .HasMaxLength(255);

                entity.Property(e => e.PlaceCodeAnswer)
                    .HasColumnName("PlaceCodeAnswer")
                    .HasMaxLength(255);


                entity.HasIndex(e => e.PlaceOfInterest_Id)
                    .HasName("PlaceOfInterest_Id");


                entity.Property(e => e.PlaceOfInterest_Id)
                    .HasColumnName("PlaceOfInterest_Id")
                    .HasColumnType("int(11)");


                entity.HasOne(d => d.PlaceOfInterest_IdNavigation)
                    .WithMany(p => p.Riddle)
                    .HasForeignKey(d => d.PlaceOfInterest_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PlaceOfInterest_Id");

            });

            modelBuilder.Entity<OrientationGame>(entity =>
            {
                entity.ToTable("OrientationGame");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Title)
                    .HasColumnName("Title")
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("Description")
                    .HasMaxLength(255);

                entity.Property(e => e.Points_For_Completion)
                    .HasColumnName("Points_For_Completion")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<OrientationGame_Riddle>(entity =>
            {
                entity.ToTable("OrientationGame_Riddle");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Number)
                    .HasColumnName("Number")
                    .HasColumnType("int(11)");

                entity.HasIndex(e => e.Riddle_Id)
                    .HasName("Route_Id");

                entity.Property(e => e.Riddle_Id)
                    .HasColumnName("Riddle_Id")
                    .HasColumnType("int(11)");

                entity.HasIndex(e => e.OrientationGame_Id)
                    .HasName("OrientationGame_Id");


                entity.Property(e => e.OrientationGame_Id)
                    .HasColumnName("OrientationGame_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Riddle_IdNavigation)
                    .WithMany(p => p.OrientationGame_Riddle)
                    .HasForeignKey(d => d.Riddle_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Riddle_Id");

                entity.HasOne(d => d.OrientationGame_IdNavigation)
                    .WithMany(p => p.OrientationGame_Riddle)
                    .HasForeignKey(d => d.OrientationGame_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("OrientationGame_Id");
            });

            modelBuilder.Entity<ClientOrientationGame>(entity =>
            {
                entity.ToTable("ClientOrientationGame");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.State)
                    .HasColumnName("State")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CurrentNumber)
                    .HasColumnName("CurrentNumber")
                    .HasColumnType("int(11)");

                entity.HasIndex(e => e.OrientationGame_Id)
                    .HasName("OrientationGame_Id");


                entity.Property(e => e.OrientationGame_Id)
                    .HasColumnName("OrientationGame_Id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.OrientationGame_IdNavigation)
                    .WithMany(p => p.ClientOrientationGame)
                    .HasForeignKey(d => d.OrientationGame_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("OrientationGame_Id");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
