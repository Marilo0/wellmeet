using Microsoft.EntityFrameworkCore;

namespace Wellmeet.Data
{
    public class WellmeetDbContext : DbContext
    {
        public WellmeetDbContext() 
        {
        }

        public WellmeetDbContext(DbContextOptions<WellmeetDbContext> options) : base(options)
        {
        }


        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Activity> Activities { get; set; } = null!;
        public DbSet<ActivityParticipant> ActivityParticipants { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            // Automatically exclude soft - deleted records from ALL queries - MINE EXTRA 
            // db.Users.ToList() automatically becomes db.Users.Where(u => !u.IsDeleted).ToList()
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Activity>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<ActivityParticipant>().HasQueryFilter(ap => !ap.IsDeleted);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(255).IsRequired();
                entity.Property(u => u.Password).HasMaxLength(255).IsRequired();
                entity.Property(u => u.Firstname).HasMaxLength(50);
                entity.Property(u => u.Lastname).HasMaxLength(50);

                entity.Property(u => u.UserRole).HasConversion<string>().HasMaxLength(20);

                entity.Property(u => u.InsertedAt)
                    .ValueGeneratedOnAdd()
                //.HasDefaultValueSql("GETUTCDATE()");  // SQL Server
                .HasDefaultValueSql("now()"); // PostgreSQL



                entity.Property(u => u.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                //.HasDefaultValueSql("GETUTCDATE()");  // SQL Server
                .HasDefaultValueSql("now()"); // PostgreSQL

                entity.HasIndex(u => u.Username, "IX_Users_Username").IsUnique();
                entity.HasIndex(u => u.Email, "IX_Users_Email").IsUnique();
            });

            // Activity configuration
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("Activities");
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Title).HasMaxLength(100).IsRequired();
                entity.Property(a => a.Description).HasMaxLength(500);
                entity.Property(a => a.City).HasMaxLength(50);
                entity.Property(a => a.Location).HasMaxLength(100);
                entity.Property(a => a.Category).HasMaxLength(50);

                entity.Property(a => a.InsertedAt)
                    .ValueGeneratedOnAdd()
                //.HasDefaultValueSql("GETUTCDATE()");  // SQL Server
                .HasDefaultValueSql("now()"); // PostgreSQL

                entity.Property(a => a.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                //.HasDefaultValueSql("GETUTCDATE()");  // SQL Server
                .HasDefaultValueSql("now()"); // PostgreSQL

                entity.HasOne(a => a.Creator)
                      .WithMany(u => u.CreatedActivities)
                      .HasForeignKey(a => a.CreatorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ActivityParticipant configuration
            modelBuilder.Entity<ActivityParticipant>(entity =>
            {
                entity.ToTable("ActivityParticipants");
                entity.HasKey(ap => ap.Id);

                entity.HasIndex(ap => new { ap.ActivityId, ap.UserId }, "IX_ActivityParticipants_ActivityId_UserId").IsUnique();

                entity.Property(ap => ap.JoinDate) //.HasDefaultValueSql("GETUTCDATE()");  // SQL Server
                .HasDefaultValueSql("now()"); // PostgreSQL
                entity.Property(ap => ap.Status).HasMaxLength(20).IsRequired();

                entity.Property(ap => ap.InsertedAt)
                    .ValueGeneratedOnAdd()
                //.HasDefaultValueSql("GETUTCDATE()");  // SQL Server
                .HasDefaultValueSql("now()"); // PostgreSQL

                entity.Property(ap => ap.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                //.HasDefaultValueSql("GETUTCDATE()");  // SQL Server
                .HasDefaultValueSql("now()"); // PostgreSQL

                entity.HasOne(ap => ap.User)
                      .WithMany(u => u.ActivityParticipants)
                      .HasForeignKey(ap => ap.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ap => ap.Activity)
                      .WithMany(a => a.Participants)
                      .HasForeignKey(ap => ap.ActivityId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // THESE BELONG HERE - They're infrastructure concerns not business logic
        public override int SaveChanges()
        {
            UpdateSoftDeleteTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateSoftDeleteTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Modified &&
                           e.Entity.IsDeleted &&
                           e.Entity.DeletedAt == null);

            foreach (var entityEntry in entries)
            {
                entityEntry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }
    }
}
