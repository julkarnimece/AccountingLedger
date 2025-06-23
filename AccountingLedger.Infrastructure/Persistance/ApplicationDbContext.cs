using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountingLedger.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }   

        public DbSet<Account> Accounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; } 
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(je => je.JournalEntry)
                .WithMany(jel => jel.JournalEntryLines)
                .HasForeignKey(je => je.JournalEntryId);


            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(je => je.Account)
                .WithMany()
                .HasForeignKey(je => je.AccountId);


            modelBuilder.Entity<JournalEntryLine>()
                .Property(jel => jel.Debit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<JournalEntryLine>()
                .Property(jel => jel.Credit)
                .HasColumnType("decimal(18,2)");

        }



    }
}
