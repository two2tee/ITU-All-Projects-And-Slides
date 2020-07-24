// Context.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using LiftLog.Data.Entities;
using LiftLog.Data.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LiftLog.Data
{
    // This class coordinates EF functionality for the data model 
    public class Context : IdentityDbContext<User, IdentityRole<int>, int>, IContext
    {
        public Context(DbContextOptions options) : base(options)
        {
        }

        //Custom DbSets

        // Entity part of data model represented as a set (table)
        // Each entity is a row in the table 
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Ranking> Rankings { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutEntry> WorkoutEntries { get; set; }
        public DbSet<Challenge> Challenges { get; set; }

        // Identity and Authorization
        //public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }

        /// <summary>
        ///     Use this method to override default behavior (e.g. table names)
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Create singular table names 
            modelBuilder.Entity<Exercise>().ToTable("Exercise");
            modelBuilder.Entity<Ranking>().ToTable("Ranking");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Workout>().ToTable("Workout");
            modelBuilder.Entity<WorkoutEntry>().ToTable("WorkoutEntry");
            modelBuilder.Entity<Challenge>().ToTable("Challenge");

            //Identity tables
            modelBuilder.Entity<IdentityRole<int>>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("AspNetUserClaims");

            // Migration cannot find primary key of IdentityUserRole 
            //modelBuilder.Entity<IdentityUserRole<int>>(x => x.HasKey(k => new { x.RoleId, x.UserId }));
            //modelBuilder.Entity<IdentityUserRole<string>>(e => e.HasKey(x => new { x.RoleId, x.UserId }));
            
            //mapping parent entities into relationships

            //User
            modelBuilder.Entity<User>()
                .HasMany(pt => pt.Workouts)
                .WithOne(pt => pt.User)
                .HasForeignKey(pt => pt.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(pt => pt.Rankings)
                .WithOne(p => p.User)
                .HasForeignKey(pt => pt.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()
                .HasMany(pt => pt.GivenChallenges)
                .WithOne(p => p.ChallengerUser)
                .HasForeignKey(pt => pt.ChallengerId)
                .OnDelete(DeleteBehavior.Restrict); //Nullable because a challengee may stil want to complete challenge even if the challenger is gone. 

            modelBuilder.Entity<User>()
                .HasMany(pt => pt.ReceivedChallenges)
                .WithOne(p => p.ChallengeeUser)
                .HasForeignKey(pt => pt.ChallengeeId)
                .IsRequired();

            //Challenge

            //Exercise
            modelBuilder.Entity<Exercise>()
                .HasMany(p => p.Challenges)
                .WithOne(pt => pt.Exercise)
                .HasForeignKey(p => p.ExerciseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exercise>()
                .HasMany(p => p.WorkoutEntries)
                .WithOne(pt => pt.Exercise)
                .HasForeignKey(pt => pt.ExerciseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            //Workout 
            modelBuilder.Entity<Workout>()
                .HasMany(p => p.WorkoutEntries)
                .WithOne(pt => pt.Workout)
                .HasForeignKey(p => p.WorkoutId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        // Change MaxLength property of primary/foreign keys to 255 instead of 256 
        // MySql only supports 767 bytes at default and default code uses utf-8 where each char is length 3 = 256*3 = 768 bytes
        // Link: https://www.codeproject.com/Articles/1167050/Running-Your-First-ASP-NET-Core-Web-App-with-MySQL
        private void SetupMySqlKeyConstraint(ModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<Exercise>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));

            modelBuilder.Entity<Ranking>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            modelBuilder.Entity<Ranking>(entity => entity.Property(m => m.UserId)
                .HasMaxLength(255));
            modelBuilder.Entity<Ranking>(entity => entity.Property(m => m.ExerciseId)
                .HasMaxLength(255));

            modelBuilder.Entity<User>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            modelBuilder.Entity<User>(entity => entity.Property(m => m.NormalizedEmail)
                .HasMaxLength(255));
            modelBuilder.Entity<User>(entity => entity.Property(m => m.NormalizedUserName)
                .HasMaxLength(255));
            modelBuilder.Entity<User>(entity => entity.Property(m => m.Email)
                .HasMaxLength(255));
            modelBuilder.Entity<User>(entity => entity.Property(m => m.UserName)
                .HasMaxLength(255));

            modelBuilder.Entity<Workout>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            modelBuilder.Entity<Workout>(entity => entity.Property(m => m.UserId)
                .HasMaxLength(255));

            modelBuilder.Entity<WorkoutEntry>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            modelBuilder.Entity<WorkoutEntry>(entity => entity.Property(m => m.ExerciseId)
                .HasMaxLength(255));
            modelBuilder.Entity<WorkoutEntry>(entity => entity.Property(m => m.WorkoutId)
                .HasMaxLength(255));

            modelBuilder.Entity<Challenge>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            modelBuilder.Entity<Challenge>(entity => entity.Property(m => m.ChallengeeId)
                .HasMaxLength(255));
            modelBuilder.Entity<Challenge>(entity => entity.Property(m => m.ChallengerId)
                .HasMaxLength(255));
            modelBuilder.Entity<Challenge>(entity => entity.Property(m => m.ExerciseId)
                .HasMaxLength(255));

            modelBuilder.Entity<IdentityRole<int>>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            modelBuilder.Entity<IdentityRole<int>>(entity => entity.Property(m => m.NormalizedName)
                .HasMaxLength(255));
            modelBuilder.Entity<IdentityRole<int>>(entity => entity.Property(m => m.Name)
                .HasMaxLength(255));

            modelBuilder.Entity<IdentityUserLogin<int>>(entity => entity.Property(m => m.UserId)
                .HasMaxLength(255));
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => entity.Property(m => m.LoginProvider)
                .HasMaxLength(255));
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => entity.Property(m => m.ProviderKey)
                .HasMaxLength(255));

            modelBuilder.Entity<IdentityUserRole<int>>(entity => entity.Property(m => m.UserId)
                .HasMaxLength(255));
            modelBuilder.Entity<IdentityUserRole<int>>(entity => entity.Property(m => m.RoleId)
                .HasMaxLength(255));

            modelBuilder.Entity<IdentityUserClaim<int>>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => entity.Property(m => m.UserId)
                .HasMaxLength(255));
        }
    }
}