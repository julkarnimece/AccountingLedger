using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Entities;
using AccountingLedger.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AccountingLedger.Infrastructure.Persistance
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            if (!await context.Accounts.AnyAsync())
            {
                context.Accounts.AddRange(
                    new Account { Name = "Cash", Type = AccountType.Asset },
                    new Account { Name = "Accounts Receivable", Type = AccountType.Asset },
                    new Account { Name = "Supplies", Type = AccountType.Asset },
                    new Account { Name = "Accounts Payable", Type = AccountType.Liability },
                    new Account { Name = "Unearned Revenue", Type = AccountType.Liability },
                    new Account { Name = "Owner's Equity", Type = AccountType.Equity },
                    new Account { Name = "Service Revenue", Type = AccountType.Revenue },
                    new Account { Name = "Rent Expense", Type = AccountType.Expense },
                    new Account { Name = "Salaries Expense", Type = AccountType.Expense }
                );
                await context.SaveChangesAsync();

                // Seed a few journal entries after accounts are available
                var cashAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Cash");
                var serviceRevenueAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Service Revenue");
                var suppliesAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Supplies");
                var accountsPayableAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "Accounts Payable");

                if (cashAccount != null && serviceRevenueAccount != null && suppliesAccount != null && accountsPayableAccount != null)
                {
                    if (!await context.JournalEntries.AnyAsync())
                    {
                        var entry1 = new JournalEntry
                        {
                            Date = DateTime.Parse("2023-01-05"),
                            Description = "Received cash for services rendered",
                            JournalEntryLines = new List<JournalEntryLine>
                            {
                                new JournalEntryLine { AccountId = cashAccount.Id, Debit = 1000, Credit = 0 },
                                new JournalEntryLine { AccountId = serviceRevenueAccount.Id, Debit = 0, Credit = 1000 }
                            }
                        };

                        var entry2 = new JournalEntry
                        {
                            Date = DateTime.Parse("2023-01-10"),
                            Description = "Purchased supplies on account",
                            JournalEntryLines = new List<JournalEntryLine>
                            {
                                new JournalEntryLine { AccountId = suppliesAccount.Id, Debit = 500, Credit = 0 },
                                new JournalEntryLine { AccountId = accountsPayableAccount.Id, Debit = 0, Credit = 500 }
                            }
                        };

                        context.JournalEntries.AddRange(entry1, entry2);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
