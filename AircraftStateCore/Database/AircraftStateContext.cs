using AircraftStateCore.DAL.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace AircraftStateCore.Database;

public partial class AircraftStateContext : DbContext
{
	public AircraftStateContext()
	{
	}

	public AircraftStateContext(DbContextOptions<AircraftStateContext> options)
		: base(options)
	{
	}

	public virtual DbSet<ProfileDatum> ProfileData { get; set; }

	public virtual DbSet<ApplicationSettingsDatum> ApplicationSettings { get; set; }

	public virtual DbSet<PlaneDatum> PlaneData { get; set; }

	public virtual DbSet<SettingsDatum> Settings { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseSqlite();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProfileDatum>(entity =>
		{
			entity.HasKey(e => e.ProfileName);

			entity.ToTable("profileData");

			entity.Property(e => e.ProfileName)
				.HasColumnType("VARCHAR(100)")
				.HasColumnName("profileName");
			entity.Property(e => e.Data).HasColumnName("data");
		});

		modelBuilder.Entity<ApplicationSettingsDatum>(entity =>
		{
			entity.HasKey(e => e.DataKey);

			entity.ToTable("applicationSettings");

			entity.Property(e => e.DataKey).HasColumnType("VARCHAR(100)");
			entity.Property(e => e.DataValue)
				.IsRequired()
				.HasColumnType("VARCHAR(100)");
		});

		//Legacy tables for copying data
		modelBuilder.Entity<PlaneDatum>(entity =>
		{
			entity.HasKey(e => e.Plane);

			entity.ToTable("planeData");

			entity.Metadata.SetIsTableExcludedFromMigrations(true);

			entity.Property(e => e.Plane)
				.HasColumnType("VARCHAR(100)")
				.HasColumnName("plane");
			entity.Property(e => e.Data).HasColumnName("data");
		});

		modelBuilder.Entity<SettingsDatum>(entity =>
		{
			entity.HasKey(e => e.DataKey);

			entity.ToTable("settings");

			entity.Metadata.SetIsTableExcludedFromMigrations(true);

			entity.Property(e => e.DataKey).HasColumnType("VARCHAR(100)");
			entity.Property(e => e.DataValue)
				.IsRequired()
				.HasColumnType("VARCHAR(100)");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
