using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team56LMSContext : DbContext
    {
        public Team56LMSContext()
        {
        }

        public Team56LMSContext(DbContextOptions<Team56LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Enrollment> Enrollment { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submissions> Submissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u0831946;Password=zxcvbnm;Database=Team56LMS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .HasColumnName("firstName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .HasColumnName("lastName")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.HasKey(e => e.AcId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.ClassId, e.AcName })
                    .HasName("classID")
                    .IsUnique();

                entity.Property(e => e.AcId).HasColumnName("acID");

                entity.Property(e => e.AcName)
                    .HasColumnName("acName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ClassId).HasColumnName("classID");

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasColumnType("varchar(3)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasKey(e => e.HwId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AcId)
                    .HasName("acID");

                entity.Property(e => e.HwId).HasColumnName("hwID");

                entity.Property(e => e.AcId).HasColumnName("acID");

                entity.Property(e => e.DueDate)
                    .HasColumnName("dueDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.HwName)
                    .HasColumnName("hwName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Instructions)
                    .HasColumnName("instructions")
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.MaxPoints)
                    .HasColumnName("maxPoints")
                    .HasColumnType("smallint(6)");

                entity.HasOne(d => d.Ac)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.AcId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ProfId)
                    .HasName("profID");

                entity.Property(e => e.ClassId).HasColumnName("classID");

                entity.Property(e => e.CatalogId)
                    .IsRequired()
                    .HasColumnName("catalogID")
                    .HasColumnType("char(5)");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ProfId)
                    .HasColumnName("profID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Start)
                    .HasColumnName("start")
                    .HasColumnType("time");

                entity.Property(e => e.Stop)
                    .HasColumnName("stop")
                    .HasColumnType("time");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.Catalog)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CatalogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_2");

                entity.HasOne(d => d.Prof)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.ProfId)
                    .HasConstraintName("Classes_ibfk_1");

                // enum or Seasons
                entity.Property(e => e.Season).HasColumnType("enum");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CatalogId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Abrev)
                    .HasName("Abrev");

                entity.HasIndex(e => new { e.Number, e.Abrev })
                    .HasName("Number")
                    .IsUnique();

                entity.Property(e => e.CatalogId)
                    .HasColumnName("catalogID")
                    .HasColumnType("char(5)");

                entity.Property(e => e.Abrev).HasColumnType("varchar(4)");

                entity.Property(e => e.CName)
                    .HasColumnName("cName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Number).HasColumnType("char(4)");

                entity.HasOne(d => d.AbrevNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Abrev)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.Abrev)
                    .HasName("PRIMARY");

                entity.Property(e => e.Abrev).HasColumnType("varchar(4)");

                entity.Property(e => e.DName)
                    .HasColumnName("dName")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.ClassId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("classID");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.ClassId).HasColumnName("classID");

                entity.Property(e => e.Grade)
                    .HasColumnName("grade")
                    .HasColumnType("varchar(2)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollment_ibfk_2");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Enrollment)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrollment_ibfk_1");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Abrev)
                    .HasName("Abrev");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Abrev)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .HasColumnName("firstName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .HasColumnName("lastName")
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.AbrevNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Abrev)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Abrev)
                    .HasName("Abrev");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Abrev)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .HasColumnName("firstName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .HasColumnName("lastName")
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.AbrevNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Abrev)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submissions>(entity =>
            {
                entity.HasKey(e => new { e.HwId, e.UId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.UId)
                    .HasName("uID");

                entity.Property(e => e.HwId).HasColumnName("hwID");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Contents)
                    .HasColumnName("contents")
                    .HasColumnType("text");

                entity.Property(e => e.STime)
                    .HasColumnName("sTime")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Score).HasColumnName("score");

                entity.HasOne(d => d.Hw)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.HwId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_1");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_2");
            });
        }
    }
}
