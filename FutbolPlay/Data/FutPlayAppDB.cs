namespace FutbolPlay
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FutPlayAppDB : DbContext
    {
        public FutPlayAppDB()
            : base("name=FutPlayAppDB")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<customer> customer { get; set; }
        public virtual DbSet<days> days { get; set; }
        public virtual DbSet<multi_pitch> multi_pitch { get; set; }
        public virtual DbSet<pitch> pitch { get; set; }
        public virtual DbSet<pitch_type> pitch_type { get; set; }
        public virtual DbSet<place> place { get; set; }
        public virtual DbSet<reservation> reservation { get; set; }
        public virtual DbSet<scheduler> scheduler { get; set; }
        public virtual DbSet<status_type> status_type { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<users_type> users_type { get; set; }
        public virtual DbSet<holidays> holidays { get; set; }
        public virtual DbSet<opening> opening { get; set; }
        public virtual DbSet<users_offline> users_offline { get; set; }
        public virtual DbSet<reservation_multipitch> reservation_multipitch { get; set; }
        public virtual DbSet<cities> cities { get; set; }
        public virtual DbSet<countries> countries { get; set; }
        public virtual DbSet<sports_type> sports_type { get; set; }
        public virtual DbSet<reservation_report> reservation_report { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<reservation_report>()
                .Property(e => e.ingresos)
                .HasPrecision(38, 0);

            modelBuilder.Entity<days>()
                .Property(e => e.name)
                .IsFixedLength();

            modelBuilder.Entity<place>()
                .Property(e => e.latitude)
                .IsUnicode(false);

            modelBuilder.Entity<place>()
                .Property(e => e.longitude)
                .IsUnicode(false);

            modelBuilder.Entity<place>()
                .Property(e => e.format_hour)
                .IsFixedLength();

            modelBuilder.Entity<scheduler>()
                .Property(e => e.value)
                .HasPrecision(18, 0);

            modelBuilder.Entity<status_type>()
                .Property(e => e.name)
                .IsFixedLength();

            modelBuilder.Entity<users>()
                .Property(e => e.id_usersocialred)
                .IsUnicode(false);

            modelBuilder.Entity<users_type>()
                .Property(e => e.name)
                .IsFixedLength();

            modelBuilder.Entity<users_offline>()
                .Property(e => e.name)
                .IsFixedLength();

            modelBuilder.Entity<users_offline>()
                .Property(e => e.phone)
                .IsFixedLength();

            modelBuilder.Entity<cities>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<countries>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<countries>()
                .HasMany(e => e.cities)
                .WithRequired(e => e.countries)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<pitch>()
                .HasMany(e => e.reservation)
                .WithRequired(e => e.pitch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<place>()
                .Property(e => e.profile_img)
                .IsUnicode(false);

            modelBuilder.Entity<place>()
                .HasMany(e => e.reservation)
                .WithRequired(e => e.place)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<reservation>()
                .Property(e => e.value)
                .HasPrecision(18, 0);

            modelBuilder.Entity<reservation>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<sports_type>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<sports_type>()
                .Property(e => e.description)
                .IsUnicode(false);
        }
    }
}
