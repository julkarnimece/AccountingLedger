using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Entities;
using AccountingLedger.Core.Interfaces;
using AccountingLedger.Infrastructure.Persistance;
using Microsoft.Data.SqlClient;
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
        //public async Task<JournalEntry> AddAsync(JournalEntry journalEntry)
        //{
        //    await _context.JournalEntries.AddAsync(journalEntry);
        //    await _context.SaveChangesAsync();
        //    return journalEntry;
        //}


        public async Task<JournalEntry> AddAsync(JournalEntry journalEntry)
        {
            var journalEntryLinesTable = new DataTable();
            journalEntryLinesTable.Columns.Add("AccountId", typeof(int));
            journalEntryLinesTable.Columns.Add("Debit", typeof(decimal));
            journalEntryLinesTable.Columns.Add("Credit", typeof(decimal));

            foreach (var line in journalEntry.JournalEntryLines)
            {
                journalEntryLinesTable.Rows.Add(line.AccountId, line.Debit, line.Credit);
            }

            var dateParam = new SqlParameter("@Date", SqlDbType.DateTime) { Value = journalEntry.Date };
            var descriptionParam = new SqlParameter("@Description", SqlDbType.NVarChar) { Value = journalEntry.Description };
            var linesParam = new SqlParameter("@JournalEntryLines", SqlDbType.Structured)
            {
                TypeName = "dbo.JournalEntryLineType", 
                Value = journalEntryLinesTable
            };

            var newIdParam = new SqlParameter("@NewJournalEntryId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };


            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_InsertJournalEntry @Date, @Description, @JournalEntryLines, @NewJournalEntryId OUTPUT",
                dateParam, descriptionParam, linesParam, newIdParam);


            journalEntry.Id = (int)newIdParam.Value;

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
