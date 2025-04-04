using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DataContext
{
    public class MedicalDbContext : DbContext
    {
        public MedicalDbContext() { }
        public MedicalDbContext(DbContextOptions<MedicalDbContext> options) : base(options) { }

        public const string CONNECTION_STRING = @"
         Host=positively-grounded-tenpounder.data-1.euc1.tembo.io;
         Port=5432;
         Username=postgres;
         Password=R2pFPK8q95GlSypK;
         Database=postgres
        ";

        public DbSet<Examination> Examinations { get; set; }
        public DbSet<MedicalFile> MedicalFiles { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(CONNECTION_STRING)
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Examination>()
                .HasOne(e => e.Patient)
                .WithMany(e => e.Examinations)
                .HasForeignKey(e => e.PatientId);

            modelBuilder.Entity<MedicalFile>()
                .HasOne(m => m.Examination)
                .WithMany(m => m.MedicalFiles)
                .HasForeignKey(m => m.ExaminationId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(p => p.PatientId);

            
        }

    }
}
