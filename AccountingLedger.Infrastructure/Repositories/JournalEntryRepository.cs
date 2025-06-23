using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Entities;
using AccountingLedger.Core.Interfaces;
using AccountingLedger.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace AccountingLedger.Infrastructure.Repositories
{
    public class JournalEntryRepository : IJournalEntryRepository
    {
        private readonly ApplicationDbContext _context;

        public JournalEntryRepository(ApplicationDbContext context)
        {
            _context = context; 
        }
        public async Task<JournalEntry> AddAsync(JournalEntry journalEntry)
        {
            await _context.JournalEntries.AddAsync(journalEntry);
            await _context.SaveChangesAsync();
            return journalEntry;
        }

        public async Task<IEnumerable<JournalEntry>> GetAllAsync()
        {
            return await _context.JournalEntries
                .Include(je => je.JournalEntryLines)
                    .ThenInclude(jel => jel.Account)
                .OrderByDescending(je => je.Date)
                .ToListAsync(); 
        }

        public async Task<JournalEntry?> GetByIdWithLinesAsync(int id)
        {
            return await _context.JournalEntries
                .Include(je => je.JournalEntryLines)
                    .ThenInclude(jel => jel.Account)
                .FirstOrDefaultAsync(je => je.Id == id);
        }

        public async Task<IEnumerable<JournalEntry>> GetFilteredAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.JournalEntries
                .Include(je => je.JournalEntryLines)
                    .ThenInclude(jel => jel.Account)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(je => je.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(je => je.Date <= endDate.Value.AddDays(1).AddTicks(-1));
            }

            return await query
                .OrderByDescending(je => je.Date)
                .ToListAsync();

        }
    }
}
