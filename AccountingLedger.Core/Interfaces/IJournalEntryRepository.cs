using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Entities;

namespace AccountingLedger.Core.Interfaces
{
    public interface IJournalEntryRepository
    {
        Task<JournalEntry> AddAsync(JournalEntry journalEntry);
        Task<IEnumerable<JournalEntry>> GetAllAsync();
        Task<JournalEntry?> GetByIdWithLinesAsync(int id);
        Task<IEnumerable<JournalEntry>> GetFilteredAsync(DateTime? startDate, DateTime? endDate);

    }
}
